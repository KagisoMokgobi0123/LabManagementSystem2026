using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using NMB_HLabSys.Models.Users.Enums;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class EditProfileViewModel
    {
        // ============================================================
        // APPLICATION USER
        // ============================================================

        // ------------------------------------------------------------
        // Email is displayed but should not be changed through this
        // workflow unless email-change functionality is deliberately
        // added later.
        // ------------------------------------------------------------

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;


        // ============================================================
        // USER PROFILE
        // ============================================================

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [StringLength(
            100,
            ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        [StringLength(
            100,
            ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a title.")]
        public Title Title { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a gender.")]
        public Gender Gender { get; set; }


        [Required(ErrorMessage = "ID Number is required.")]
        [Display(Name = "ID Number")]
        [StringLength(20)]
        public string IDNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select an ethnicity.")]
        public Ethnicity Ethnicity { get; set; }


        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a province.")]
        [Display(Name = "Province")]
        public int ProvinceId { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a city.")]
        [Display(Name = "City")]
        public int CityId { get; set; }


        [Required(ErrorMessage = "Address Line 1 is required.")]
        [Display(Name = "Address Line 1")]
        [StringLength(250)]
        public string Address1 { get; set; } = string.Empty;


        [Display(Name = "Address Line 2")]
        [StringLength(250)]
        public string? Address2 { get; set; }


        [Required(ErrorMessage = "Postal Code is required.")]
        [Display(Name = "Postal Code")]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;


        // ============================================================
        // PROFILE IMAGE
        // ============================================================

        public string? ExistingProfileImagePath { get; set; }


        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }


        // ============================================================
        // PASSWORD
        //
        // These fields are specifically for the ApplicationUser
        // password used for login.
        //
        // Password changes are optional.
        // ============================================================

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }


        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "Password must be between 8 and 100 characters.")]
        public string? NewPassword { get; set; }


        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(NewPassword),
            ErrorMessage =
                "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }


        // ============================================================
        // DROPDOWN OPTIONS
        // ============================================================

        public IEnumerable<SelectListItem> Provinces { get; set; }
            = new List<SelectListItem>();


        public IEnumerable<SelectListItem> Cities { get; set; }
            = new List<SelectListItem>();
    }
}
