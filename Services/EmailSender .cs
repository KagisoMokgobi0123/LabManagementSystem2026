using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace NMB_HLabSys.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            var smtpHost =
                _configuration["EmailSettings:SmtpHost"];

            var smtpPort =
                int.Parse(
                    _configuration["EmailSettings:SmtpPort"]
                    ?? "587");

            var smtpUsername =
                _configuration["EmailSettings:Username"];

            var smtpPassword =
                _configuration["EmailSettings:Password"];

            var fromEmail =
                _configuration["EmailSettings:FromEmail"];

            var fromName =
                _configuration["EmailSettings:FromName"]
                ?? "NMB Health Laboratory System";


            using var message =
                new MailMessage();

            message.From =
                new MailAddress(
                    fromEmail!,
                    fromName);

            message.To.Add(email);

            message.Subject =
                subject;

            message.Body =
                htmlMessage;

            message.IsBodyHtml =
                true;


            using var smtp =
                new SmtpClient(
                    smtpHost,
                    smtpPort);

            smtp.EnableSsl =
                true;

            smtp.Credentials =
                new NetworkCredential(
                    smtpUsername,
                    smtpPassword);


            await smtp.SendMailAsync(
                message);
        }
    }
}