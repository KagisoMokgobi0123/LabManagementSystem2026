using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Doctor
{
    public class CreatePatientViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "Surname is required")]
        [StringLength(100)]
        [Display(Name = "Surname")]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "SA ID number is required")]
        [StringLength(
            13,
            MinimumLength = 13,
            ErrorMessage = "SA ID number must be exactly 13 digits")]
        [RegularExpression(
            @"^\d{13}$",
            ErrorMessage = "SA ID number must contain only digits")]
        [Display(Name = "South African ID Number")]
        public string IDNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Cellphone number is required")]
        [Phone(ErrorMessage = "Invalid cellphone number")]
        [StringLength(20)]
        [Display(Name = "Cellphone Number")]
        public string CellPhone { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256)]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Home address is required")]
        [StringLength(300)]
        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; } = string.Empty;


        // =========================================================
        // MEDICAL HISTORY
        // =========================================================
        //
        // The doctor may capture these details when registering
        // a new patient.
        //
        // These are optional because a patient may have no known
        // conditions, allergies, or medications.
        //

        public List<int> SelectedConditionIds { get; set; } = new();

        public List<int> SelectedAllergyIds { get; set; } = new();

        public List<int> SelectedMedicationIds { get; set; } = new();


        // =========================================================
        // DROPDOWN DATA
        // =========================================================
        //
        // These are populated by the controller and are not
        // submitted as patient data.
        //

        public List<SelectListItem> AvailableConditions { get; set; }
            = new();

        public List<SelectListItem> AvailableAllergies { get; set; }
            = new();

        public List<SelectListItem> AvailableMedications { get; set; }
            = new();
    }
}
