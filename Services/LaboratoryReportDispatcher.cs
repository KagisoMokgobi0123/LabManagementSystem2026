
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
            _logger.LogInformation(
                "Laboratory Report Dispatcher started.");

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
                    catch (OperationCanceledException)
                        when (stoppingToken.IsCancellationRequested)
                    {
                        break;
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
                // Application is shutting down normally.
            }

            _logger.LogInformation(
                "Laboratory Report Dispatcher stopped.");
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
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // Load pending email.
                    var email = await context.TechnicianSentEmails
                        .FirstOrDefaultAsync(
                            x => x.SentEmailId == emailId &&
                                 x.Status == "Pending",
                            cancellationToken);

                    if (email == null)
                        continue;

                    // Never trust a previously stored PDF on a Pending email.
                    // It may have been generated before results or recipient details changed.
                    // Hold it for review rather than risk sending stale clinical data.
                    if (email.PdfAttachment is { Length: > 0 })
                    {
                        email.Status = "Failed";
                        email.ErrorMessage =
                            "Existing PDF requires review before delivery; " +
                            "a previously generated attachment was not revalidated.";
                        await context.SaveChangesAsync(cancellationToken);
                        _logger.LogWarning(
                            "Held email {EmailId} because its PDF was already attached.",
                            emailId);
                        continue;
                    }

                    // Prepare and validate a fresh report from the current records.
                    bool prepared = await deliveryService.PrepareReportForEmailAsync(
                        email.SentEmailId, cancellationToken);

                    if (!prepared)
                    {
                        email.Status = "Failed";
                        email.ErrorMessage =
                            "Report preparation blocked: check the result records, " +
                            "verification, request status, and doctor's address.";
                        await context.SaveChangesAsync(cancellationToken);
                        _logger.LogWarning("Preparation blocked for email {EmailId}.", emailId);
                        continue;
                    }

                    await context.Entry(email).ReloadAsync(cancellationToken);
                    if (email.PdfAttachment == null || email.PdfAttachment.Length == 0)
                    {
                        email.Status = "Failed";
                        email.ErrorMessage = "Generated PDF attachment is missing.";
                        await context.SaveChangesAsync(cancellationToken);
                        continue;
                    }

                    // Atomically claim the email for delivery.
                    int claimed = await context.TechnicianSentEmails
                        .Where(x =>
                            x.SentEmailId == emailId &&
                            x.Status == "Pending")
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(
                                x => x.Status,
                                "Processing"),
                            cancellationToken);

                    if (claimed != 1)
                    {
                        context.Entry(email).State =
                            EntityState.Detached;

                        continue;
                    }

                    await context.Entry(email).ReloadAsync(
                        cancellationToken);

                    // Ensure the recipient is valid.
                    if (string.IsNullOrWhiteSpace(email.RecipientEmail) ||
                        email.RecipientEmail ==
                            "Pending recipient resolution")
                    {
                        email.Status = "Failed";
                        email.ErrorMessage =
                            "Requesting doctor's email is missing.";

                        await context.SaveChangesAsync(
                            cancellationToken);

                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(email.Subject) ||
                        string.IsNullOrWhiteSpace(email.PdfFileName))
                    {
                        email.Status = "Failed";
                        email.ErrorMessage =
                            "Email subject or PDF filename is missing.";

                        await context.SaveChangesAsync(
                            cancellationToken);

                        continue;
                    }

                    // Automatically send PDF to the doctor.
                    await emailService.SendReportAsync(
                        email.RecipientEmail,
                        email.Subject,
                        email.Body ?? string.Empty,
                        email.PdfFileName,
                        email.PdfAttachment!,
                        cancellationToken);

                    // Mark as successfully submitted to SMTP.
                    email.Status = "Sent";
                    email.SentAt = DateTime.UtcNow;
                    email.ErrorMessage = null;

                    await context.SaveChangesAsync(
                        cancellationToken);

                    _logger.LogInformation(
                        "Laboratory report email {EmailId} accepted by SMTP.",
                        emailId);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Laboratory email {EmailId} processing failed.",
                        emailId);

                    try
                    {
                        context.ChangeTracker.Clear();

                        string errorMessage =
                            ex.Message.Length > 1000
                                ? ex.Message[..1000]
                                : ex.Message;

                        await context.TechnicianSentEmails
                            .Where(x =>
                                x.SentEmailId == emailId &&
                                (x.Status == "Pending" ||
                                 x.Status == "Processing"))
                            .ExecuteUpdateAsync(
                                setters => setters
                                    .SetProperty(
                                        x => x.Status,
                                        "Failed")
                                    .SetProperty(
                                        x => x.ErrorMessage,
                                        errorMessage),
                                CancellationToken.None);
                    }
                    catch (Exception updateException)
                    {
                        _logger.LogError(
                            updateException,
                            "Unable to record failure for email {EmailId}.",
                            emailId);
                    }
                }
            }
        }
    }
}

