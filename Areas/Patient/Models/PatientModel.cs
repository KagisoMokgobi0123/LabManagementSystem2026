using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientModel
    {
        [Key]
        public string PatientId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }//KG CHANGED FROM RegisterVeiwModel to ApplicationUser

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Surname { get; set; } = string.Empty;

        public string FullName => $"{Name} {Surname}";

        [Required]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "SA ID must be 13 digits")]
        public string IDNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;    // Street address

        public string Suburb { get; set; } = string.Empty;    // Optional

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Province { get; set; } = string.Empty;

        [StringLength(4)]
        public string PostalCode { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedDate { get; set; }

        // Navigation Properties
        public ICollection<PatientConditionModel> PatientConditions { get; set; }
            = new List<PatientConditionModel>();
        public ICollection<PatientAllergyModel> PatientAllergies { get; set; }
            = new List<PatientAllergyModel>();
        public ICollection<PatientMedicationModel> PatientMedications { get; set; }
            = new List<PatientMedicationModel>();
        public ICollection<TestRequestsModel> TestRequests { get; set; }
            = new List<TestRequestsModel>();
        public ICollection<PatientDoctorConsentModel> PatientDoctorConsents { get; set; }
            = new List<PatientDoctorConsentModel>();
    }
}