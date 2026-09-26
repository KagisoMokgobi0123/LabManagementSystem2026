using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class ActivateAccountViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Temporary password is required.")]
        [Display(Name = "Temporary Password")]
        [DataType(DataType.Password)]
        public string TemporaryPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(NewPassword),
            ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
