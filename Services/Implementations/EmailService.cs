using System.Net;
using System.Net.Mail;
using System.Text;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Services
{
    public class EmailService : IEmailService
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


        // ============================================================
        // TEMPORARY PASSWORD EMAIL
        // ============================================================

        public async Task SendTemporaryPasswordEmailAsync(
            string email,
            string firstName,
            string temporaryPassword,
            string activationLink)
        {
            var subject =
                "Account Registration - Activate Your Account";

            var body =
                $"""
                Hello {firstName},

                Your account has been created successfully.

                Your temporary password is:

                {temporaryPassword}

                Please activate your account using the link below:

                {activationLink}

                You must activate your account and create a new
                password before you can log in.

                If you did not register for this account, please
                contact System Administration.

                Regards,
                System Administration
                """;

            await SendEmailAsync(
                email,
                subject,
                body);
        }


        // ============================================================
        // PASSWORD RESET OTP EMAIL
        // ============================================================

        public async Task SendPasswordResetOtpEmailAsync(
            string email,
            string firstName,
            string otp)
        {
            var subject =
                "Password Reset OTP";

            var body =
                $"""
                Hello {firstName},

                Your password reset OTP is:

                {otp}

                This OTP will expire shortly.

                If you did not request this password reset,
                please ignore this email.

                Regards,
                System Administration
                """;

            await SendEmailAsync(
                email,
                subject,
                body);
        }


        // ============================================================
        // SEND EMAIL
        // ============================================================

        public async Task SendEmailAsync(
            string email,
            string subject,
            string body)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Recipient email address is required.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException(
                    "Email subject is required.",
                    nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException(
                    "Email body is required.",
                    nameof(body));
            }


            // --------------------------------------------------------
            // Read SMTP configuration
            // --------------------------------------------------------

            var host =
                _configuration["EmailSettings:Host"];

            var portValue =
                _configuration["EmailSettings:Port"];

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            var fromEmail =
                _configuration["EmailSettings:From"];


            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Host is missing.");
            }

            if (string.IsNullOrWhiteSpace(portValue))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port is missing.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Username is missing.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Password is missing.");
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings:From is missing.");
            }


            // --------------------------------------------------------
            // Parse SMTP port
            // --------------------------------------------------------

            if (!int.TryParse(
                    portValue,
                    out var port))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port must be a valid number.");
            }


            // --------------------------------------------------------
            // Create SMTP client
            // --------------------------------------------------------

            using var client =
                new SmtpClient(
                    host,
                    port)
                {
                    EnableSsl = true,

                    UseDefaultCredentials = false,

                    Credentials =
                        new NetworkCredential(
                            username,
                            password),

                    DeliveryMethod =
                        SmtpDeliveryMethod.Network,

                    Timeout = 30000
                };


            // --------------------------------------------------------
            // Create email
            // --------------------------------------------------------

            using var mail =
                new MailMessage
                {
                    From =
                        new MailAddress(
                            fromEmail),

                    Subject =
                        subject,

                    Body =
                        body,

                    IsBodyHtml =
                        false,

                    BodyEncoding =
                        Encoding.UTF8,

                    SubjectEncoding =
                        Encoding.UTF8
                };


            mail.To.Add(
                new MailAddress(email));


            // --------------------------------------------------------
            // Send
            // --------------------------------------------------------

            try
            {
                _logger.LogInformation(
                    "Sending email to {Email} using SMTP host {Host}:{Port}.",
                    email,
                    host,
                    port);

                await client.SendMailAsync(mail);

                _logger.LogInformation(
                    "Email successfully sent to {Email}.",
                    email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP error while sending email to {Email}. " +
                    "SMTP status code: {StatusCode}.",
                    email,
                    ex.StatusCode);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while sending email to {Email}.",
                    email);

                throw;
            }
        }
    }
}
