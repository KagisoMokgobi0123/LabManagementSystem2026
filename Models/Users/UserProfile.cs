using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users.Enums;

namespace NMB_HLabSys.Models.Users
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }


        // ============================================================
        // IDENTITY USER
        // ============================================================

        /*
         * Every registered user has a UserProfile.
         *
         * Registration creates this record immediately with the
         * information already available at registration time:
         *
         * - FirstName
         * - LastName
         *
         * The remaining profile information is completed later.
         */
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;


        // ============================================================
        // PERSONAL INFORMATION
        // ============================================================

        /*
         * First Name and Last Name are collected during registration
         * and are therefore required when the UserProfile is created.
         */
        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;


        // ============================================================
        // PROFILE INFORMATION
        // ============================================================

        /*
         * These fields are intentionally nullable because the
         * UserProfile is created during registration before the user
         * has completed the full profile.
         *
         * They are populated later through:
         *
         * /Profile/Complete
         *
         * and/or:
         *
         * /Profile/Edit
         */

        [Display(Name = "Title")]
        public Title? Title { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "ID Number")]
        [StringLength(20)]
        public string? IDNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Ethnicity")]
        public Ethnicity? Ethnicity { get; set; }


        // ============================================================
        // ADDRESS INFORMATION
        // ============================================================

        /*
         * Address information is completed after registration.
         */

        [StringLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Province")]
        public int? ProvinceId { get; set; }

        public virtual Province? Province { get; set; }

        [Display(Name = "City")]
        public int? CityId { get; set; }

        public virtual City? City { get; set; }

        [Display(Name = "Address Line 1")]
        [StringLength(250)]
        public string? Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(250)]
        public string? Address2 { get; set; }

        [Display(Name = "Postal Code")]
        [StringLength(20)]
        public string? PostalCode { get; set; }


        // ============================================================
        // PROFILE IMAGE
        // ============================================================

        [StringLength(300)]
        [Display(Name = "Profile Image")]
        public string? ProfileImagePath { get; set; }


        // ============================================================
        // AUDIT INFORMATION
        // ============================================================

        /*
         * Set when the UserProfile is initially created during
         * registration.
         */
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /*
         * Updated whenever profile information is changed.
         */
        public DateTime? UpdatedDate { get; set; }
    }
}
