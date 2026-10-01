using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class MedicationCategory
    {
        public int MedicationCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Status Status { get; set; }

        public ICollection<Medication> Medications { get; set; }
            = new List<Medication>();
    }
}
