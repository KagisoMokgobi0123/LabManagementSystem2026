using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Administrator.Models;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class MedicationModel
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(150)]
        public string MedicationName { get; set; }

        public string Description { get; set; }

        // FK
        public int MedicationCategoryId { get; set; }
        public MedicationCategory MedicationCategory { get; set; }

        // Many-to-many with Patient
        public ICollection<PatientMedicationModel> PatientMedications { get; set; }
    }
}
