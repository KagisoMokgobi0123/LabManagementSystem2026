using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class MedicalCondition
    {
        public int MedicalConditionId { get; set; }

        public int MedicalConditionCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Status Status { get; set; }

        public MedicalConditionCategory Category { get; set; } = null!;

    }
}
