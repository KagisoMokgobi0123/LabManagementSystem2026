using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace NMB_HLabSys.Models.Account
{
    public class RegisterUser : IdentityUser
    {
       
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 100 characters")]
        public string FullName { get; set; } = String.Empty;

        // Role-based navigation


    }
}
