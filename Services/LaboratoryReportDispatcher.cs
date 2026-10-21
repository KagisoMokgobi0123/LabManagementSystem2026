
using Microsoft.EntityFrameworkCore;
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

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            // Wait briefly for application startup.
            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);

                using var timer = new PeriodicTimer(
                    TimeSpan.FromSeconds(15));

                do
                {
                    try
                    {
                        await ProcessPendingAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Laboratory report dispatch cycle failed.");
                    }
                }
                while (await timer.WaitForNextTickAsync(
                    stoppingToken));
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                // Normal application shutdown.
            }
        }

        private async Task ProcessPendingAsync(
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var deliveryService = scope.ServiceProvider
                .GetRequiredService<LaboratoryReportDeliveryService>();

            var emailService = scope.ServiceProvider
                .GetRequiredService<LaboratoryEmailService>();

            var pendingIds = await context.TechnicianSentEmails
                .AsNoTracking()
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.CreatedAt)
                .Select(x => x.SentEmailId)
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var emailId in pendingIds)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var email = await context.TechnicianSentEmails
                        .FirstOrDefaultAsync(
                            x => x.SentEmailId == emailId &&
                                 x.Status == "Pending",
                            cancellationToken);

                    if (email == null)
                        continue;

                    // Generate the report if necessary.
                    if (email.PdfAttachment == null ||
                        email.PdfAttachment.Length == 0)
                    {
                        bool prepared =
                            await deliveryService.PrepareReportAsync(
                                email.TestRequestId);

                        if (!prepared)
                        {
                            _logger.LogWarning(
                                "Report preparation deferred for email {EmailId}.",
                                emailId);

                            continue;
                        }

                        // Reload the saved PDF and recipient.
                        await context.Entry(email).ReloadAsync(
                            cancellationToken);
                    }

                    if (email.PdfAttachment == null ||
                        email.PdfAttachment.Length == 0)
                        continue;

                    // Stop this email from being picked up again
                    // by this dispatcher after a failed attempt.
                    email.Status = "Processing";
                    await context.SaveChangesAsync(cancellationToken);

                    try
                    {
                        await emailService.SendReportAsync(
                            email.RecipientEmail,
                            email.Subject,
                            email.Body,
                            email.PdfFileName,
                            email.PdfAttachment,
                            cancellationToken);

                        email.Status = "Sent";
                        email.SentAt = DateTime.UtcNow;
                        email.ErrorMessage = null;

                        await context.SaveChangesAsync(
                            cancellationToken);

                        _logger.LogInformation(
                            "Laboratory email {EmailId} accepted by SMTP.",
                            emailId);
                    }
                    catch (Exception ex)
                    {
                        // Do not automatically retry an uncertain
                        // SMTP outcome; it could duplicate a report.
                        email.Status = "Failed";
                        email.ErrorMessage =
                            ex.Message.Length > 1000
                                ? ex.Message[..1000]
                                : ex.Message;

                        await context.SaveChangesAsync(
                            CancellationToken.None);

                        _logger.LogError(
                            ex,
                            "Laboratory email {EmailId} failed.",
                            emailId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Could not process laboratory email {EmailId}.",
                        emailId);
                }
            }
        }
    }
}
