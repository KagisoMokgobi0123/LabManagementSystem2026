namespace NMB_HLabSys.Patterns.Strategy
{
    public interface INotificationStrategy
    {
        void Send(string to, string subject, string body);

        void SendWithAttachment(string to, string subject, string body,
                                byte[] attachment, string filename);
    }

    // ----------------------------------------------------------
    // Concrete Strategy A: SMTP e-mail (production)
    // ----------------------------------------------------------
    public class EmailNotificationStrategy : INotificationStrategy
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _fromAddress;

        public EmailNotificationStrategy(string smtpHost, int smtpPort, string fromAddress)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _fromAddress = fromAddress;
        }

        public void Send(string to, string subject, string body)
        {
            // Wire up System.Net.Mail.SmtpClient here
            // using var client = new SmtpClient(_smtpHost, _smtpPort);
            // var msg = new MailMessage(_fromAddress, to, subject, body);
            // client.Send(msg);
            Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
        }

        public void SendWithAttachment(string to, string subject, string body,
                                       byte[] attachment, string filename)
        {
            // Attach the PDF byte array via MailMessage.Attachments
            Console.WriteLine($"[EMAIL+PDF] To: {to} | Subject: {subject} | File: {filename}");
        }
    }

    // ----------------------------------------------------------
    // Concrete Strategy B: Console logger (development/testing)
    // ----------------------------------------------------------
    public class ConsoleNotificationStrategy : INotificationStrategy
    {
        public void Send(string to, string subject, string body)
            => Console.WriteLine($"[CONSOLE NOTIFY] → {to}\n  {subject}\n  {body}\n");

        public void SendWithAttachment(string to, string subject, string body,
                                       byte[] attachment, string filename)
            => Console.WriteLine($"[CONSOLE NOTIFY+PDF] → {to}\n  {subject}\n  PDF: {filename} ({attachment?.Length} bytes)\n");
    }
}
