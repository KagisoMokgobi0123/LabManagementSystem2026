using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Services
{
    public class LaboratoryReportDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LaboratoryReportDispatcher> _logger;

        public LaboratoryReportDispatcher(
            IServiceScopeFactory scopeFactory,
            ILogger<LaboratoryReportDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Laboratory Report Dispatcher started.");
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
                do
                {
                    try
                    {
                        await ProcessPendingAsync(stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Laboratory report dispatch cycle failed.");
                    }
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            _logger.LogInformation("Laboratory Report Dispatcher stopped.");
        }

        private async Task ProcessPendingAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deliveryService = scope.ServiceProvider.GetRequiredService<LaboratoryReportDeliveryService>();
            var emailService = scope.ServiceProvider.GetRequiredService<LaboratoryEmailService>();

            // Only final verified reports are automatically delivered.
            // Preliminary reports remain available through the existing manual workflow.
            var pendingIds = await context.TechnicianSentEmails
                .AsNoTracking()
                .Where(x => x.Status == "Pending" && x.ReportType == "Final")
                .OrderBy(x => x.CreatedAt)
                .Select(x => x.SentEmailId)
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var emailId in pendingIds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                bool smtpAttemptStarted = false;

                try
                {
                    // Claim before preparing the report, not afterward.
                    int claimed = await context.TechnicianSentEmails
                        .Where(x => x.SentEmailId == emailId &&
                                    x.Status == "Pending" &&
                                    x.ReportType == "Final")
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(x => x.Status, "Processing"),
                            cancellationToken);

                    if (claimed != 1)
                        continue;

                    // Always regenerate from the current database state.
                    // Preparation also resolves the current requesting doctor's address.
                    bool prepared = await deliveryService.PrepareReportForEmailAsync(
                        emailId, cancellationToken);

                    if (!prepared)
                    {
                        await SetStatusAsync(context, emailId, "Failed",
                            "Final report preparation blocked: request or verification state is invalid.",
                            cancellationToken);
                        continue;
                    }

                    // Read current saved report after preparation.
                    context.ChangeTracker.Clear();
                    var email = await context.TechnicianSentEmails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.SentEmailId == emailId &&
                                                  x.Status == "Processing" &&
                                                  x.ReportType == "Final",
                            cancellationToken);

                    if (email == null)
                        continue;

                    if (email.PdfAttachment is not { Length: > 0 } ||
                        string.IsNullOrWhiteSpace(email.RecipientEmail) ||
                        email.RecipientEmail == "Pending recipient resolution" ||
                        string.IsNullOrWhiteSpace(email.Subject) ||
                        string.IsNullOrWhiteSpace(email.PdfFileName))
                    {
                        await SetStatusAsync(context, emailId, "Failed",
                            "Prepared report is missing an attachment or required email fields.",
                            cancellationToken);
                        continue;
                    }

                    // From this point on, a connection failure or crash may mean
                    // SMTP accepted the message. Do not retry automatically.
                    smtpAttemptStarted = true;
                    await emailService.SendReportAsync(
                        email.RecipientEmail,
                        email.Subject,
                        email.Body ?? string.Empty,
                        email.PdfFileName,
                        email.PdfAttachment,
                        cancellationToken);

                    // Record successful SMTP submission (not inbox delivery).
                    await context.TechnicianSentEmails
                        .Where(x => x.SentEmailId == emailId && x.Status == "Processing")
                        .ExecuteUpdateAsync(
                            setters => setters
                                .SetProperty(x => x.Status, "Sent")
                                .SetProperty(x => x.SentAt, DateTime.UtcNow)
                                .SetProperty(x => x.ErrorMessage, (string?)null),
                            CancellationToken.None);

                    _logger.LogInformation(
                        "Final laboratory report email {EmailId} accepted by SMTP.", emailId);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    if (smtpAttemptStarted)
                    {
                        await TrySetStatusAsync(context, emailId, "DeliveryUncertain",
                            "Shutdown occurred during SMTP submission. Check the mailbox and logs before any manual retry.");
                    }
                    // If preparation was interrupted, Processing stays held for review.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Laboratory email {EmailId} processing failed.", emailId);
                    string error = ex.Message.Length > 800 ? ex.Message[..800] : ex.Message;
                    await TrySetStatusAsync(context, emailId,
                        smtpAttemptStarted ? "DeliveryUncertain" : "Failed",
                        smtpAttemptStarted
                            ? "SMTP outcome uncertain; do not automatically retry. " + error
                            : "Report preparation failed. " + error);
                }
            }
        }

        private static async Task SetStatusAsync(
            ApplicationDbContext context,
            int emailId,
            string status,
            string message,
            CancellationToken cancellationToken)
        {
            context.ChangeTracker.Clear();
            await context.TechnicianSentEmails
                .Where(x => x.SentEmailId == emailId && x.Status == "Processing")
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.Status, status)
                        .SetProperty(x => x.ErrorMessage, message),
                    cancellationToken);
        }

        private async Task TrySetStatusAsync(
            ApplicationDbContext context,
            int emailId,
            string status,
            string message)
        {
            try
            {
                await SetStatusAsync(context, emailId, status, message, CancellationToken.None);
            }
            catch (Exception updateException)
            {
                _logger.LogError(updateException,
                    "Could not record {Status} for email {EmailId}.", status, emailId);
            }
        }
    }
}
