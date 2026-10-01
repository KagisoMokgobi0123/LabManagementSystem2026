using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class AllergyCategory
    {
        public int AllergyCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Status Status { get; set; }

        public ICollection<Allergy> Allergies { get; set; }
            = new List<Allergy>();
    }
}
