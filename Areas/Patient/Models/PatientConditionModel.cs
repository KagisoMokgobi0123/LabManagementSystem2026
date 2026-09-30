using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientConditionModel
    {
        [Key]
        public int PatientConditionId { get; set; }

        public string PatientId { get; set; } = string.Empty;

        [ForeignKey("PatientId")]
        public virtual PatientModel? Patient { get; set; }

        public int MedicalConditionId { get; set; }

        [ForeignKey("MedicalConditionId")]
        public virtual MedicalConditionModel? MedicalCondition { get; set; }

        public DateTime DateDiagnosed { get; set; } = DateTime.Now;

        public string? Notes { get; set; }
    }
}
