
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace NMB_HLabSys.Services
{
    public class LaboratoryEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<LaboratoryEmailService> _logger;

        public LaboratoryEmailService(
            IOptionsMonitor<EmailSettings> options,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<LaboratoryEmailService> logger)
        {
            _settings = options.Get("Technician");
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendReportAsync(
            string recipientEmail,
            string subject,
            string message,
            string pdfFileName,
            byte[] pdfBytes,
            CancellationToken cancellationToken = default)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException(
                    "PDF attachment cannot be empty.",
                    nameof(pdfBytes));

            if (string.IsNullOrWhiteSpace(recipientEmail))
                throw new ArgumentException(
                    "Recipient email is required.",
                    nameof(recipientEmail));

            if (string.IsNullOrWhiteSpace(_settings.Host) ||
                string.IsNullOrWhiteSpace(_settings.Username) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail))
            {
                throw new InvalidOperationException(
                    "Technician SMTP configuration is incomplete.");
            }

            string actualRecipient = recipientEmail.Trim();

            if (_environment.IsDevelopment())
            {
                string? testRecipient = _configuration[
                    "LaboratoryReportDelivery:TestRecipientEmail"];

                if (string.IsNullOrWhiteSpace(testRecipient))
                {
                    throw new InvalidOperationException(
                        "Development email delivery blocked: " +
                        "TestRecipientEmail is not configured.");
                }

                actualRecipient = testRecipient.Trim();

                _logger.LogInformation(
                    "Development laboratory report email redirected " +
                    "to the configured test inbox.");
            }

            var email = new MimeMessage();

            email.From.Add(
                MailboxAddress.Parse(_settings.SenderEmail));

            email.To.Add(
                MailboxAddress.Parse(actualRecipient));

            email.Subject = _environment.IsDevelopment()
                ? "[TEST - NMB HLabSys] " + subject
                : subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = _environment.IsDevelopment()
                    ? "DEVELOPMENT TEST EMAIL - NOT FOR CLINICAL USE."
                      + Environment.NewLine
                      + Environment.NewLine
                      + message
                    : message
            };

            bodyBuilder.Attachments.Add(
                pdfFileName,
                pdfBytes,
                ContentType.Parse("application/pdf"));

            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password,
                cancellationToken);

            await smtp.SendAsync(
                email,
                cancellationToken);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);
        }
    }
}
