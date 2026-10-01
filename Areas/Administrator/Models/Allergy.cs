using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class Allergy
    {
        public int AllergyId { get; set; }

        public int AllergyCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Status Status { get; set; }

        public AllergyCategory Category { get; set; } = null!;
    }
}
