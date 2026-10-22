
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NMB_HLabSys.Services
{
    public sealed class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IConfiguration configuration,
            ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Recipient email is required.", nameof(email));

            var host = _configuration["EmailSettings:SmtpHost"];
            var portValue = _configuration["EmailSettings:SmtpPort"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"]
                           ?? "NMB Health Laboratory System";

            if (string.IsNullOrWhiteSpace(host) ||
                !int.TryParse(portValue, out var port) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "Email configuration is incomplete. Check " +
                    "EmailSettings:SmtpHost, SmtpPort, Username, " +
                    "Password and FromEmail.");
            }

            if (!MailAddress.TryCreate(email, out _) ||
                !MailAddress.TryCreate(fromEmail, out _))
            {
                throw new InvalidOperationException(
                    "The sender or recipient email address is invalid.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            try
            {
                await smtp.SendMailAsync(message);
                _logger.LogInformation(
                    "Email sent successfully to {Recipient}.", email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex, "SMTP failed to send an email to {Recipient}.", email);
                throw;
            }
        }
    }
}