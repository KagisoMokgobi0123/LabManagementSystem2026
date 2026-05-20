using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Doctor
{
    public class CreatePatientViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "SA ID number is required")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "SA ID must be exactly 13 digits")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "SA ID must contain only digits")]
        public string IDNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? CellPhone { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        public string? HomeAddress { get; set; }

        // Medical history — selected from Admin-maintained lists
        public List<int> SelectedConditionIds { get; set; } = new();
        public List<int> SelectedAllergyIds { get; set; } = new();
        public List<int> SelectedMedicationIds { get; set; } = new();

        // Populate dropdowns
        public List<SelectListItem> AvailableConditions { get; set; } = new();
        public List<SelectListItem> AvailableAllergies { get; set; } = new();
        public List<SelectListItem> AvailableMedications { get; set; } = new();
    }
}