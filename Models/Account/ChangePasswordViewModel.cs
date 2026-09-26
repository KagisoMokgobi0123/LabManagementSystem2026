using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
