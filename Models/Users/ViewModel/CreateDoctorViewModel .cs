using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Users.ViewModel
{
    public class CreateDoctorViewModel
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

        // PROFESSIONAL INFORMATION

        [Required(ErrorMessage = "HPCSA number is required.")]
        [Display(Name = "HPCSA Number")]
        [StringLength(50)]
        public string HPCSANumber { get; set; } = string.Empty;

        // CONTACT INFORMATION

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}