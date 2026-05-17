using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class MeasurementUnitModel
    {
        [Key]
        public int MeasurementUnitId { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Unit Symbol")]
        public string Unit { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<TestTypeModel> TestTypes { get; set; } = new List<TestTypeModel>();
    }
}
