//using System.ComponentModel.DataAnnotations;

//namespace NMB_HLabSys.Models.Account
//{
//    public class RegisterViewModel
//    {
//        [Required]
//        [Display(Name = "First Name")]
//        public string FirstName { get; set; } = string.Empty;

//        [Required]
//        [Display(Name = "Last Name")]
//        public string LastName { get; set; } = string.Empty;

//        [Required]
//        [EmailAddress]
//        [Display(Name = "Email Address")]
//        public string Email { get; set; } = string.Empty;

//        [Required]
//        [DataType(DataType.Password)]
//        [StringLength(100,
//            MinimumLength = 8,
//            ErrorMessage = "Password must be at least 8 characters.")]
//        public string Password { get; set; } = string.Empty;

//        [Required]
//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm Password")]
//        [Compare(nameof(Password),
//            ErrorMessage = "The passwords do not match.")]
//        public string ConfirmPassword { get; set; } = string.Empty;

//        [Required(ErrorMessage = "You must accept the terms and privacy policy.")]
//        [Range(typeof(bool), "true", "true",
//            ErrorMessage = "You must accept the terms and privacy policy.")]
//        public bool AcceptTerms { get; set; }
//    }
//}
using System;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class RegisterViewModel
    {
        // ================================
        // COMMON REGISTRATION FIELDS
        // ================================

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;


        // ================================
        // PATIENT FIELDS
        // ================================

        [Required(ErrorMessage = "South African ID number is required.")]
        [Display(Name = "South African ID Number")]
        [StringLength(
            13,
            MinimumLength = 13,
            ErrorMessage = "South African ID number must contain exactly 13 digits."
        )]
        [RegularExpression(
            @"^\d{13}$",
            ErrorMessage = "South African ID number must contain only 13 digits."
        )]
        public string? IdNumber { get; set; }


        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Cellphone number is required.")]
        [Phone]
        [Display(Name = "Cellphone Number")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Home address is required.")]
        [Display(Name = "Home Address")]
        public string? HomeAddress { get; set; }


        // ================================
        // PASSWORD
        // ================================

        [Required]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters."
        )]
        public string Password { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(
            nameof(Password),
            ErrorMessage = "The passwords do not match."
        )]
        public string ConfirmPassword { get; set; } = string.Empty;


        // ================================
        // TERMS
        // ================================

        [Required(ErrorMessage = "You must accept the terms and privacy policy.")]
        [Range(
            typeof(bool),
            "true",
            "true",
            ErrorMessage = "You must accept the terms and privacy policy."
        )]
        public bool AcceptTerms { get; set; }


        // ================================
        // REGISTRATION ROLE
        // ================================

        [Required(ErrorMessage = "Please select a registration role.")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;


        // ================================
        // DOCTOR
        // ================================

        [Display(Name = "HPCSA Number")]
        public string? HPCSANumber { get; set; }


        // ================================
        // LAB TECHNICIAN
        // ================================

        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }
    }


}