using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientMedicationModel
    {
        [Key]
        public int PatientMedicationId { get; set; }

        public string PatientId { get; set; } = string.Empty;

        [ForeignKey("PatientId")]
        public virtual PatientModel? Patient { get; set; }

        public int MedicationId { get; set; }

        [ForeignKey("MedicationId")]
        public virtual MedicationModel? Medication { get; set; }

        public string? Dosage { get; set; }

        public string? Frequency { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }
    }
}