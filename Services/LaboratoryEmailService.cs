
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace NMB_HLabSys.Services
{
    public class LaboratoryEmailService
    {
        private readonly EmailSettings _settings;

        public LaboratoryEmailService(
            IOptionsMonitor<EmailSettings> options)
        {
            _settings = options.Get("Technician");
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

            if (string.IsNullOrWhiteSpace(_settings.Host) ||
                string.IsNullOrWhiteSpace(_settings.Username) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail))
            {
                throw new InvalidOperationException(
                    "Technician SMTP configuration is incomplete.");
            }

            var email = new MimeMessage();

            email.From.Add(
                MailboxAddress.Parse(_settings.SenderEmail));

            email.To.Add(
                MailboxAddress.Parse(recipientEmail));

            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = message
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
