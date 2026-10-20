
using System.Net;
using System.Net.Mail;

namespace NMB_HLabSys.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailWithPdfAsync(
            string recipientEmail,
            string subject,
            string body,
            byte[] pdfBytes,
            string pdfFileName)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
                throw new ArgumentException(
                    "Recipient email is required.");

            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException(
                    "PDF attachment cannot be empty.");

            var host = _configuration["EmailSettings:SmtpHost"]
                ?? throw new InvalidOperationException(
                    "SMTP host is not configured.");

            var port = int.Parse(
                _configuration["EmailSettings:SmtpPort"]
                ?? throw new InvalidOperationException(
                    "SMTP port is not configured."));

            var senderEmail = _configuration["EmailSettings:SenderEmail"]
                ?? throw new InvalidOperationException(
                    "Sender email is not configured.");

            var password = _configuration["EmailSettings:Password"]
                ?? throw new InvalidOperationException(
                    "SMTP password is not configured.");

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(recipientEmail);

            var attachmentStream = new MemoryStream(pdfBytes);

            message.Attachments.Add(
                new Attachment(
                    attachmentStream,
                    pdfFileName,
                    "application/pdf"));

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(
                    senderEmail,
                    password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            try
            {
                await smtp.SendMailAsync(message);

                _logger.LogInformation(
                    "Laboratory report email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send laboratory report email.");

                throw;
            }
        }
    }
}
