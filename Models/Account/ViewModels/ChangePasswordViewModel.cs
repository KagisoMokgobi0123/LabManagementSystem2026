using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(
            100,
            ErrorMessage = "Password must be at least {2} characters.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare(
            "NewPassword",
            ErrorMessage = "The new passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}