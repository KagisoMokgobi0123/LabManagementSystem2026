using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class LoginUser : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
