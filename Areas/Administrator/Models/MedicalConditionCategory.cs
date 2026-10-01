using NMB_HLabSys.Areas.Administrator.Models.Enums;
using System.Net.NetworkInformation;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class MedicalConditionCategory
    {
        public int MedicalConditionCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Status Status { get; set; }

        public ICollection<MedicalCondition> MedicalConditions { get; set; }
            = new List<MedicalCondition>();

    }
}
