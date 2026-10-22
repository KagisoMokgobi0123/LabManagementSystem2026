using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeModel
    {

        public enum RecordStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Key]
        public int TestTypeId { get; set; }


        [Required]
        [StringLength(150)]
        [Display(Name = "Test Name")]
        public string Name { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Test Category")]
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))]
        public virtual TestCategoryModel? TestCategory { get; set; }


        [Required]
        [Display(Name = "Required Sample Type")]
        public int SampleTypeId { get; set; }

        [ForeignKey(nameof(SampleTypeId))]
        public virtual SampleTypeModel? SampleType { get; set; }


        [Required]
        [Display(Name = "Measurement Unit")]
        public int MeasurementUnit { get; set; }

        [ForeignKey(nameof(MeasurementUnit))]
        public virtual MeasurementUnitModel? MeasurementUnitDetails
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Minimum")]
        public decimal NormalMin { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Maximum")]
        public decimal NormalMax { get; set; }


        [Required]
        [Range(1, 10000)]
        [Display(Name = "Turnaround Time (Minutes)")]
        public int TurnAroundTimeHours { get; set; }


        [Required]
        [Display(Name = "Status")]
        public RecordStatus IsActive { get; set; }
            = RecordStatus.Active;


        public virtual ICollection<TechnicianTestTypeModel>
            TechnicianTestTypes
        { get; set; }
            = new List<TechnicianTestTypeModel>();


        public virtual ICollection<TestTypeMeasurementModel>
            TestTypeMeasurements
        { get; set; }
                = new List<TestTypeMeasurementModel>();
        public virtual ICollection<TestTypeConsumableModel>
            TestTypeConsumables
        { get; set; }
            = new List<TestTypeConsumableModel>();
    }
}
