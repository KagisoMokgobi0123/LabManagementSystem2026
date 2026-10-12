using System.Net;
using System.Net.Mail;

namespace NMB_HLabSys.Areas.Doctor.Services;

public sealed class DoctorEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DoctorEmailService> _logger;
    public DoctorEmailService(IConfiguration configuration, ILogger<DoctorEmailService> logger)
    { _configuration = configuration; _logger = logger; }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = _configuration["EmailSettings:Host"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var from = _configuration["EmailSettings:From"] ?? username;
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("Email is not configured. Configure EmailSettings through user-secrets or environment variables.");

        using var message = new MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };
        using var client = new SmtpClient(host, int.TryParse(_configuration["EmailSettings:Port"], out var port) ? port : 587)
        {
            EnableSsl = true, Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
        try { await client.SendMailAsync(message); }
        catch (Exception ex) { _logger.LogError(ex, "Email delivery failed for recipient {Recipient}", to); throw; }
    }
}
