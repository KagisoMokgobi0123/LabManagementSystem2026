namespace NMB_HLabSys.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(
            string userId);

        Task<bool> ValidateOtpAsync(
            string userId,
            string otp);


        Task MarkOtpAsUsedAsync(
            string userId,
            string otp);
    }
}