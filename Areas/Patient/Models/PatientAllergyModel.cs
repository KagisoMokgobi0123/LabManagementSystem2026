using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientAllergyModel
    {
        [Key]
        public int PatientAllergyId { get; set; }

        public string PatientId { get; set; } = string.Empty;

        [ForeignKey("PatientId")]
        public virtual PatientModel? Patient { get; set; }

        public int AllergyId { get; set; }

        [ForeignKey("AllergyId")]
        public virtual AllergyModel? Allergy { get; set; }

        public string? Severity { get; set; }

        public string? Notes { get; set; }
    }
}