using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeModel
    {
        [Key]
        public int TestTypeId { get; set; }

        public int SampleTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int TestCategoryId { get; set; }

        public decimal NormalMin { get; set; }

        public decimal NormalMax { get; set; }

        public int ConsumableId { get; set; }
        // public ConsumableModel Consumable { get; set; } = null!; // uncomment after migration

        public int TurnAroundTimeHours { get; set; }

        // Removed ICollection<TestRequestsModel> — was causing shadow TestTypeModelTestTypeId column
        // public ICollection<TestTypeConsumableModel> TestTypeConsumables { get; set; } // uncomment after migration
        // public List<TechnicianTestTypeModel> TechnicianTestTypes { get; set; } = new(); // uncomment after migration

        [StringLength(20)]
        public int MeasurementUnit { get; set; }

        // Navigation properties
        [ForeignKey("SampleTypeId")]
        public virtual SampleTypeModel SampleType { get; set; }

        [ForeignKey("TestCategoryId")]
        public TestCategoryModel TestCategory { get; set; } = null!;
    }
}