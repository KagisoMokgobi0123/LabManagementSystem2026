
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeMeasurementModel
    {
        // ==========================================
        // PRIMARY KEY
        // ==========================================

        [Key]
        public int TestTypeMeasurementId { get; set; }

        // ==========================================
        // TEST TYPE RELATIONSHIP
        // ==========================================

        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))]
        public virtual TestTypeModel? TestType { get; set; }

        // ==========================================
        // MEASUREMENT NAME
        // ==========================================

        [Required]
        [StringLength(200)]
        [Display(Name = "Measurement Name")]
        public string MeasurementName { get; set; }
            = string.Empty;

        // ==========================================
        // MEASUREMENT UNIT RELATIONSHIP
        // ==========================================

        [Required]
        [Display(Name = "Measurement Unit")]
        public int MeasurementUnitId { get; set; }

        [ForeignKey(nameof(MeasurementUnitId))]
        public virtual MeasurementUnitModel? MeasurementUnit
        {
            get;
            set;
        }

        // ==========================================
        // NORMAL REFERENCE RANGE
        // ==========================================

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Minimum")]
        public decimal? NormalMin { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Normal Maximum")]
        public decimal? NormalMax { get; set; }

        // ==========================================
        // REQUIRED MEASUREMENT
        // ==========================================

        [Display(Name = "Required Measurement")]
        public bool IsRequired { get; set; } = true;

        // ==========================================
        // DISPLAY ORDER
        // ==========================================

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 0;
    }
}
