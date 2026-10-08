using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Users;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Identity
{
    public class ApplicationUser : IdentityUser
    { // PERSONAL INFORMATION 

        [Required][StringLength(100)] public string FirstName { get; set; } = string.Empty;

        [Required][StringLength(100)] public string LastName { get; set; } = string.Empty;

        // CONVENIENCE PROPERTY 

        public string FullName => $"{FirstName} {LastName}".Trim();

       // PASSWORD RESET OTP 

        public virtual ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();

   // ACCOUNT STATE 

        public bool IsActive { get; set; } = false;

        public bool MustChangePassword { get; set; } = true;

        public bool IsProfileCompleted { get; set; } = false;

        //AUDIT INFORMATION 

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        // USER PROFILES 

        public virtual DoctorProfile? DoctorProfile { get; set; }

        public virtual TechnicianProfile? TechnicianProfile { get; set; }
    }
}