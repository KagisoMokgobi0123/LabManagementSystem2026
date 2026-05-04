namespace NMB_HLabSys.Areas.Technician.Data
{
    public interface IEmailService
    {
      
            Task SendEmailWithAttachmentAsync(
                string to,
                string subject,
                string body,
                byte[] attachment,
                string attachmentName);
        
    }
}
