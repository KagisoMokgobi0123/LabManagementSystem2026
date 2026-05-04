using System.Net.Mail;
namespace NMB_HLabSys.Areas.Technician.Data
{
    public class EmailService : IEmailService
    {
   
        public async Task SendEmailWithAttachmentAsync(
            string to,
            string subject,
            string body,
            byte[] attachment,
            string attachmentName)
        {
            var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var stream = new MemoryStream(attachment);
            message.Attachments.Add(new Attachment(stream, attachmentName));

            using var smtp = new SmtpClient("smtp.yourserver.com");
            await smtp.SendMailAsync(message);
        }
    }
}

