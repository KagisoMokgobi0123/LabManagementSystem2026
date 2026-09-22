using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Laboratory;
using NMB_HLabSys.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // ============================================================
        // SHARED PERSONAL PROFILE
        // ============================================================

        /*
         * Every registered user has a UserProfile.
         *
         * The UserProfile is created during registration with the
         * information available at that point, such as:
         *
         * - FirstName
         * - LastName
         *
         * The remaining profile fields are completed later.
         */
        public virtual UserProfile? UserProfile { get; set; }


        // ============================================================
        // ROLE-SPECIFIC ONE-TO-ONE PROFILES
        // ============================================================

        public virtual Doctor? Doctor { get; set; }

        public virtual TechnicianProfile? TechnicianProfile { get; set; }

        public virtual ManagerProfile? ManagerProfile { get; set; }

        public virtual AdministratorProfile? AdministratorProfile { get; set; }


        // ============================================================
        // CONVENIENCE PROPERTIES
        // ============================================================

        /*
         * FirstName and LastName are stored in UserProfile rather
         * than directly on ApplicationUser.
         *
         * These convenience properties make them easily accessible
         * throughout the application.
         */

        [NotMapped]
        public string Name =>
            UserProfile?.FirstName ?? string.Empty;

        [NotMapped]
        public string Surname =>
            UserProfile?.LastName ?? string.Empty;

        [NotMapped]
        public string FullName =>
            $"{Name} {Surname}".Trim();


        // ============================================================
        // PASSWORD RESET / OTP HISTORY
        // ============================================================

        public virtual ICollection<PasswordResetOtp> PasswordResetOtps
        {
            get;
            set;
        } = new List<PasswordResetOtp>();


        // ============================================================
        // REGISTRATION / ACCOUNT STATE
        // ============================================================

        /*
         * ------------------------------------------------------------
         * SELF-REGISTERED PATIENTS
         * ------------------------------------------------------------
         *
         * IsActive = true
         * MustChangePassword = false
         * IsProfileCompleted = true
         *
         * Patients provide their own password during registration and
         * use the separate Patient profile workflow.
         *
         * Their shared UserProfile may contain their registration
         * information, but they do not use /Profile/Complete.
         *
         *
         * ------------------------------------------------------------
         * STAFF USERS
         * ------------------------------------------------------------
         *
         * IsActive = false
         * MustChangePassword = true
         * IsProfileCompleted = false
         *
         * Staff users receive a temporary password and must activate
         * their account.
         *
         * Their UserProfile is nevertheless created immediately during
         * registration with at least:
         *
         * - FirstName
         * - LastName
         *
         * The remaining profile fields are completed after activation.
         *
         *
         * ------------------------------------------------------------
         * PATIENTS REGISTERED BY A DOCTOR
         * ------------------------------------------------------------
         *
         * IsActive = false
         * MustChangePassword = true
         * IsProfileCompleted = true
         *
         * Doctor-created patients use the separate Patient entity and
         * Patient workflow rather than the shared staff UserProfile
         * completion process.
         */

        public bool IsActive { get; set; } = false;

        public bool MustChangePassword { get; set; } = true;

        /*
         * Indicates whether the user's required profile information
         * has been completed.
         *
         * IMPORTANT:
         *
         * IsProfileCompleted == false does NOT mean that UserProfile
         * does not exist.
         *
         * A UserProfile is created during registration and may initially
         * contain only FirstName and LastName.
         */
        public bool IsProfileCompleted { get; set; } = false;


        // ============================================================
        // AUDIT INFORMATION
        // ============================================================

        /*
         * Date and time when the Identity account was created.
         */
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /*
         * Date and time when the user last successfully logged in.
         */
        public DateTime? LastLoginDate { get; set; }
    }
}
