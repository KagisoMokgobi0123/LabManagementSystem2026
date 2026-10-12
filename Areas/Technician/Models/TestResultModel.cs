
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestResultModel
    {
        [Key]
        public int TestResultId { get; set; }

        // -----------------------------------------
        // Requested test
        // -----------------------------------------

        [Required]
        public int TestRequestTestTypeId { get; set; }

        [ForeignKey(nameof(TestRequestTestTypeId))]
        public virtual TestRequestTestTypeModel?
            TestRequestTestType
        { get; set; }

        // -----------------------------------------
        // Result information
        // -----------------------------------------

        [Required]
        [StringLength(200)]
        [Display(Name = "Result Name")]
        public string ResultName { get; set; } =
            string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Result Value")]
        public string ResultValue { get; set; } =
            string.Empty;

        [StringLength(100)]
        [Display(Name = "Measurement Unit")]
        public string? MeasurementUnit { get; set; }

        // -----------------------------------------
        // Reference range
        // -----------------------------------------

        [Display(Name = "Minimum Reference Value")]
        public decimal? ReferenceMin { get; set; }

        [Display(Name = "Maximum Reference Value")]
        public decimal? ReferenceMax { get; set; }

        // -----------------------------------------
        // Abnormal result detection
        // -----------------------------------------

        [Display(Name = "Abnormal Result")]
        public bool IsAbnormal { get; set; }

        [StringLength(20)]
        [Display(Name = "Result Flag")]
        public string ResultFlag { get; set; } =
            "Normal";

        // Normal, Low, High, or Not Evaluated

        // -----------------------------------------
        // Technician information
        // -----------------------------------------

        [Required]
        public string TechnicianId { get; set; } =
            string.Empty;

        [ForeignKey(nameof(TechnicianId))]
        public virtual ApplicationUser?
            Technician
        { get; set; }

        [StringLength(2000)]
        [Display(Name = "Technician Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Captured Date")]
        public DateTime CapturedDateTime { get; set; } =
            DateTime.Now;

        // -----------------------------------------
        // Verification information
        // -----------------------------------------

        public string? VerifiedByTechnicianId { get; set; }

        [ForeignKey(nameof(VerifiedByTechnicianId))]
        public virtual ApplicationUser?
            VerifiedByTechnician
        { get; set; }

        public DateTime? VerifiedDateTime { get; set; }

        [StringLength(2000)]
        [Display(Name = "Verification Notes")]
        public string? VerificationNotes { get; set; }

        // -----------------------------------------
        // Review information
        // -----------------------------------------

        [Display(Name = "Returned For Review")]
        public bool RequiresReview { get; set; } = false;

        [StringLength(2000)]
        [Display(Name = "Review Notes")]
        public string? ReviewNotes { get; set; }

        [StringLength(100)]
        public string? PreviousMeasurementUnit { get; set; }

        [StringLength(100)]
        public string? NewMeasurementUnit { get; set; }

        public decimal? PreviousReferenceMin { get; set; }
        public decimal? NewReferenceMin { get; set; }

        public decimal? PreviousReferenceMax { get; set; }
        public decimal? NewReferenceMax { get; set; }

        public bool? PreviousIsAbnormal { get; set; }
        public bool? NewIsAbnormal { get; set; }

        [StringLength(20)]
        public string? PreviousResultFlag { get; set; }

        [StringLength(20)]
        public string? NewResultFlag { get; set; }

        [StringLength(2000)]
        public string? PreviousResultNotes { get; set; }

        [StringLength(2000)]
        public string? NewResultNotes { get; set; }

        // ==========================================
        // MANAGER-DEFINED MEASUREMENT
        // ==========================================

        // Nullable initially to preserve existing results.
        public int? TestTypeMeasurementId { get; set; }

        [ForeignKey(nameof(TestTypeMeasurementId))]
        public virtual TestTypeMeasurementModel? TestTypeMeasurement
        {
            get;
            set;
        }
    }
}
