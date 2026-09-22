namespace NMB_HLabSys.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendTemporaryPasswordEmailAsync(
            string email,
            string firstName,
            string temporaryPassword,
            string activationLink);

        Task SendPasswordResetOtpEmailAsync(
            string email,
            string firstName,
            string otp);

        Task SendEmailAsync(
            string email,
            string subject,
            string body);
    }
}