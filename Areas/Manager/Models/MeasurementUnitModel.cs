using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class MeasurementUnitModel
    {
        [Key]
        public int MeasurementUnitId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        // One measurement unit can be used by many test types.
        public virtual ICollection<TestTypeModel> TestTypes { get; set; }
            = new List<TestTypeModel>();
    }
}