using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class Medication
    {
        public int MedicationId { get; set; }

        public int MedicationCategoryId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public Status Status { get; set; }

        public MedicationCategory Category { get; set; } = null!;
    }
}
