using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeModel
    {
        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int TestTypeId { get; set; }


        // ============================================================
        // TEST NAME
        // Examples:
        // Full Blood Count (FBC)
        // Prothrombin Time (PT)
        // Glucose (Fasting)
        // ============================================================

        [Required]
        [StringLength(150)]
        [Display(Name = "Test Name")]
        public string Name { get; set; } = string.Empty;


        // ============================================================
        // TEST CATEGORY
        // ============================================================

        [Required]
        [Display(Name = "Test Category")]
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))]
        public virtual TestCategoryModel? TestCategory { get; set; }


        // ============================================================
        // REQUIRED SAMPLE TYPE
        // ============================================================

        [Required]
        [Display(Name = "Required Sample Type")]
        public int SampleTypeId { get; set; }

        [ForeignKey(nameof(SampleTypeId))]
        public virtual SampleTypeModel? SampleType { get; set; }


        // ============================================================
        // MEASUREMENT UNIT
        // ============================================================

        [Required]
        [Display(Name = "Measurement Unit")]
        public int MeasurementUnit { get; set; }

        [ForeignKey(nameof(MeasurementUnit))]
        public virtual MeasurementUnitModel? MeasurementUnitDetails
        {
            get;
            set;
        }


        // ============================================================
        // NORMAL RANGE
        // ============================================================

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Minimum")]
        public decimal NormalMin { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Maximum")]
        public decimal NormalMax { get; set; }


        // ============================================================
        // TURNAROUND TIME
        // ============================================================

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Turnaround Time (Hours)")]
        public int TurnAroundTimeHours { get; set; }


        // ============================================================
        // EXISTING PRIMARY CONSUMABLE
        //
        // Keep this because your current seed data still supplies
        // ConsumableId directly on TestTypeModel.
        // ============================================================

        [Required]
        [Display(Name = "Consumable")]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }


        // ============================================================
        // TEST REQUESTS
        //
        // A test type can appear in many test requests.
        // ============================================================

        public virtual ICollection<TestTypeModel>
            TestTypes
        { get; set; }
            = new List<TestTypeModel>();


        // ============================================================
        // TECHNICIAN ASSIGNMENTS
        //
        // Multiple technicians can be assigned to perform this test.
        // ============================================================

        public virtual ICollection<TechnicianTestTypeModel>
            TechnicianTestTypes
        { get; set; }
            = new List<TechnicianTestTypeModel>();


        // ============================================================
        // CONSUMABLE REQUIREMENTS
        //
        // A test can use multiple consumables.
        // ============================================================

        public virtual ICollection<TestTypeConsumableModel>
            TestTypeConsumables
        { get; set; }
            = new List<TestTypeConsumableModel>();
    }
}