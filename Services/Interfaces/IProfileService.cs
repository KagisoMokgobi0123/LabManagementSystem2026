using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Services.Interfaces
{
    public interface IProfileService
    {
        // ============================================================
        // PROFILE COMPLETION
        // ============================================================

        Task<bool>
            IsProfileCompletedAsync(
                string userId);

        Task<CompleteProfileViewModel?>
            GetProfileAsync(
                string userId);

        Task<bool>
            CompleteProfileAsync(
                string userId,
                CompleteProfileViewModel model);


        // ============================================================
        // PROFILE EDIT
        // ============================================================

        Task<EditProfileViewModel?>
            GetEditProfileAsync(
                string userId);

        Task<bool>
            UpdateAccountProfileAsync(
                string userId,
                EditProfileViewModel model);
    }

}
