using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}