using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class TestRequestsModel
    {
        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int TestRequestId { get; set; }


        // ============================================================
        // REQUEST NUMBER
        // ============================================================

        [Required]
        [StringLength(50)]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; } = string.Empty;


        // ============================================================
        // PATIENT
        // ============================================================

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [ForeignKey(nameof(PatientId))]
        public virtual ApplicationUser? Patient { get; set; }


        // ============================================================
        // DOCTOR
        // ============================================================

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [ForeignKey(nameof(DoctorId))]
        public virtual ApplicationUser? Doctor { get; set; }


        // ============================================================
        // REQUEST DATE
        // ============================================================

        [Required]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;


        // ============================================================
        // URGENCY
        // Routine / Urgent / Stat
        // ============================================================

        [Required]
        [Display(Name = "Urgency")]
        public Urgency Urgency { get; set; } = Urgency.Routine;


        // ============================================================
        // CLINICAL NOTES
        // ============================================================

        [StringLength(2000)]
        [Display(Name = "Clinical Notes")]
        public string? ClinicalNotes { get; set; }


        // ============================================================
        // REQUEST STATUS
        //
        // Submitted
        // Samples Received
        // InProgress
        // Completed
        // Released
        // Cancelled
        // ============================================================

        [Required]
        [StringLength(50)]
        public string Status { get; set; } =
            TestRequestStatus.Submitted.ToString();


        // ============================================================
        // SAMPLES
        //
        // One test request can contain one or more samples.
        // ============================================================

        public virtual ICollection<SampleModel> Samples { get; set; }
            = new List<SampleModel>();


        // ============================================================
        // REQUESTED TEST TYPES
        //
        // One request can contain multiple test types.
        // ============================================================

        public virtual ICollection<TestTypeModel> TestTypes
        {
            get;
            set;
        } = new List<TestTypeModel>();
    }


    // ================================================================
    // URGENCY
    // ================================================================

    public enum Urgency
    {
        Routine = 1,
        Urgent = 2,
        Stat = 3
    }


    // ================================================================
    // TEST REQUEST STATUS
    // ================================================================

    public enum TestRequestStatus
    {
        Submitted = 1,

        SamplesReceived = 2,

        InProgress = 3,

        Completed = 4,

        Released = 5,

        Cancelled = 6
    }
}