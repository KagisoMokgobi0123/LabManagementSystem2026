using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class MedicalCondition
    {
        [Key]
        public int ConditionId { get; set; }
    }
}
