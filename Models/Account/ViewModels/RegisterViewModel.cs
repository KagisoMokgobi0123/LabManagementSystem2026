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
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    public class RegisterViewModel
    {
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

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password),
            ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must accept the terms and privacy policy.")]
        [Range(typeof(bool), "true", "true",
            ErrorMessage = "You must accept the terms and privacy policy.")]
        public bool AcceptTerms { get; set; }


        // Selected registration role
        [Required(ErrorMessage = "Please select a registration role.")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;


        // Required only when registering as a Doctor
        [Display(Name = "HPCSA Number")]
        public string? HPCSANumber { get; set; }


        // Required only when registering as a Lab Technician
        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }
    }


}