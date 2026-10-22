
using System.Net;
using System.Net.Mail;

namespace NMB_HLabSys.Areas.Doctor.Services;

public sealed class DoctorEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DoctorEmailService> _logger;

    public DoctorEmailService(
        IConfiguration configuration,
        ILogger<DoctorEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody)
    {
        // Support both existing configuration naming conventions.
        var host = _configuration["EmailSettings:Host"]
            ?? _configuration["EmailSettings:SmtpHost"];

        var portValue = _configuration["EmailSettings:Port"]
            ?? _configuration["EmailSettings:SmtpPort"];

        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        var from = _configuration["EmailSettings:From"]
            ?? _configuration["EmailSettings:FromEmail"]
            ?? username;

        var fromName = _configuration["EmailSettings:FromName"]
            ?? "NMB Health Laboratory System";

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException(
                "Email configuration is incomplete. Check the " +
                "EmailSettings host, username, password and sender address.");
        }

        if (!int.TryParse(portValue ?? "587", out var port) ||
            port is < 1 or > 65535)
        {
            throw new InvalidOperationException(
                "EmailSettings:Port or EmailSettings:SmtpPort must be a valid port.");
        }

        if (!MailAddress.TryCreate(to, out _) ||
            !MailAddress.TryCreate(from, out _))
        {
            throw new InvalidOperationException(
                "The recipient or sender email address is invalid.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(from, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(to);

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        try
        {
            await client.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(
                ex,
                "SMTP delivery failed. Host: {Host}, Port: {Port}",
                host,
                port);

            throw;
        }
    }
}
