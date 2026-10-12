
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestResultVerificationHistoryModel
    {
        [Key]
        public int TestResultVerificationHistoryId { get; set; }

        // ==========================================
        // TEST RESULT RELATIONSHIP
        // ==========================================

        [Required]
        public int TestResultId { get; set; }

        [ForeignKey(nameof(TestResultId))]
        public TestResultModel? TestResult { get; set; }

        // ==========================================
        // TECHNICIAN WHO PERFORMED THE ACTION
        // ==========================================

        [Required]
        public string ActionByTechnicianId { get; set; }
            = string.Empty;

        [ForeignKey(nameof(ActionByTechnicianId))]
        public ApplicationUser? ActionByTechnician { get; set; }

        // ==========================================
        // ACTION INFORMATION
        // ==========================================

        [Required]
        [StringLength(30)]
        public string Action { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Notes { get; set; }

        public DateTime ActionDateTime { get; set; }
            = DateTime.UtcNow;

        // ==========================================
        // PREVIOUS AND NEW RESULT VALUES
        // ==========================================

        [StringLength(200)]
        public string? PreviousResultValue { get; set; }

        [StringLength(200)]
        public string? NewResultValue { get; set; }

        // ==========================================
        // PREVIOUS AND NEW MEASUREMENT UNITS
        // ==========================================

        [StringLength(100)]
        public string? PreviousMeasurementUnit { get; set; }

        [StringLength(100)]
        public string? NewMeasurementUnit { get; set; }

        // ==========================================
        // PREVIOUS AND NEW REFERENCE RANGES
        // ==========================================

        public decimal? PreviousReferenceMin { get; set; }

        public decimal? NewReferenceMin { get; set; }

        public decimal? PreviousReferenceMax { get; set; }

        public decimal? NewReferenceMax { get; set; }

        // ==========================================
        // PREVIOUS AND NEW ABNORMAL FLAGS
        // ==========================================

        public bool? PreviousIsAbnormal { get; set; }

        public bool? NewIsAbnormal { get; set; }

        // ==========================================
        // PREVIOUS AND NEW RESULT FLAGS
        // ==========================================

        [StringLength(20)]
        public string? PreviousResultFlag { get; set; }

        [StringLength(20)]
        public string? NewResultFlag { get; set; }

        // ==========================================
        // PREVIOUS AND NEW RESULT NOTES
        // ==========================================

        [StringLength(2000)]
        public string? PreviousResultNotes { get; set; }

        [StringLength(2000)]
        public string? NewResultNotes { get; set; }
    }
}
