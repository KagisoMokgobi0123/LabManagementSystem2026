using NMB_HLabSys.Areas.Administrator.Models;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class MedicalCondition
    {
        public int MedicalConditionId { get; set; }

        [Required]
        [StringLength(150)]
        public string ConditionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // FK
        public int MedicalConditionCategoryId { get; set; }
        public MedicalConditionCategory MedicalConditionCategory { get; set; } = null;

        // Many-to-many with Patient
        public ICollection<PatientCondition> PatientConditions { get; set; } = new List<PatientCondition>();

    }
}
