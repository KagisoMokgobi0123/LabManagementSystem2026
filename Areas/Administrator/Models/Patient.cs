using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class Patient
    {
        [Key]
        public int PatientID { get; set; } 

        // =========================================================
        // ASP.NET IDENTITY ACCOUNT
        // =========================================================

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;


        // =========================================================
        // PATIENT PERSONAL INFORMATION
        // =========================================================

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;


        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string Surname { get; set; } = string.Empty;


        [Required]
        [StringLength(13, MinimumLength = 13)]
        [Display(Name = "South African ID Number")]
        public string IDNumber { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Cellphone Number")]
        public string CellPhone { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [StringLength(300)]
        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; } = string.Empty;


        // =========================================================
        // PATIENT REGISTRATION
        // =========================================================

        // Populated only when an administrator/staff member
        // creates a patient account on behalf of the patient.
        public string? RegisterUserId { get; set; }

        [ForeignKey(nameof(RegisterUserId))]
        public virtual ApplicationUser? RegisterUser { get; set; }


        // =========================================================
        // PATIENT MEDICAL HISTORY
        // =========================================================

        public virtual ICollection<PatientCondition> PatientConditions
        {
            get;
            set;
        } = new List<PatientCondition>();
        public virtual ICollection<PatientAllergy> PatientAllergies
        {
            get;
            set;
        } = new List<PatientAllergy>();

        public virtual ICollection<PatientMedication> PatientMedications
        {
            get;
            set;
        } = new List<PatientMedication>();

        public virtual ICollection<PatientDoctorConsent> PatientDoctorConsents { get; set; }= new List<PatientDoctorConsent>();


        // =========================================================
        // OPTIONAL CONVENIENCE PROPERTY
        // =========================================================
        //
        // This is NOT stored in the database.
        // It replaces the old Patient.FullName usage.
        //

        [NotMapped]
        public string FullName =>
            $"{Name} {Surname}".Trim();
    }
}
