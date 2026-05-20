using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace NMB_HLabSys.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Just log to console or file for development
            System.Diagnostics.Debug.WriteLine($"EMAIL TO: {email}");
            System.Diagnostics.Debug.WriteLine($"SUBJECT: {subject}");
            System.Diagnostics.Debug.WriteLine($"MESSAGE: {htmlMessage}");

            // You could also write to a log file
            return Task.CompletedTask;
        }
    }
}