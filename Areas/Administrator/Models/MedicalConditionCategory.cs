using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Patient.Models;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class MedicalConditionCategory
    {
        public int MedicalConditionCategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();

    }
}
