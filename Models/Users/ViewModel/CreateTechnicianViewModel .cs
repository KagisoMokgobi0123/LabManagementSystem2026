using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Users.ViewModel
{
    public class CreateTechnicianViewModel
    {

        // PERSONAL INFORMATION

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // IDENTIFICATION

        [Required(ErrorMessage = "South African ID number is required.")]
        [Display(Name = "South African ID Number")]
        [StringLength(13,MinimumLength = 13,ErrorMessage = "South African ID number must contain exactly 13 digits.")]
        [RegularExpression(@"^\d{13}$",ErrorMessage = "South African ID number must contain only 13 digits.")]
        public string IdNumber { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Employee Number")]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        // CONTACT INFORMATION

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        // TEST TYPES

        [Required(ErrorMessage = "At least one test type must be assigned.")]
        [Display(Name = "Test Types")]
        public List<int> TestTypeIds { get; set; } = new List<int>();
    }
}