using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeModel
    {
        // ============================================================
        // RECORD STATUS
        // ============================================================

        public enum RecordStatus
        {
            Inactive = 0,
            Active = 1
        }


        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int TestTypeId { get; set; }


        // ============================================================
        // TEST NAME
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
        // Property name remains TurnAroundTimeHours for compatibility
        // with existing subsystems.
        //
        // VALUE IS NOW STORED IN MINUTES.
        // ============================================================

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Turnaround Time (Minutes)")]
        public int TurnAroundTimeHours { get; set; }


        // ============================================================
        // SOFT DELETE / RECORD STATUS
        // ============================================================

        [Required]
        [Display(Name = "Status")]
        public RecordStatus IsActive { get; set; }
            = RecordStatus.Active;


        // ============================================================
        // TECHNICIAN ASSIGNMENTS
        // ============================================================

        public virtual ICollection<TechnicianTestTypeModel>
            TechnicianTestTypes
        { get; set; }
            = new List<TechnicianTestTypeModel>();


        // ============================================================
        // CONSUMABLE REQUIREMENTS
        // ============================================================
        // Many-to-many relationship:
        //
        // TestType -> TestTypeConsumable -> Consumable
        //
        // A test can have many consumables.
        // A consumable can be used by many tests.
        //
        // ConsumableId is intentionally NOT stored directly on
        // TestTypeModel.
        // ============================================================

        public virtual ICollection<TestTypeConsumableModel>
            TestTypeConsumables
        { get; set; }
            = new List<TestTypeConsumableModel>();
    }
}
