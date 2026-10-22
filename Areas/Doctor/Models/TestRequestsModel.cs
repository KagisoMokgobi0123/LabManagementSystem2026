
using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class TestRequestsModel
    {
        [Key]
        public int TestRequestId { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestNumber { get; set; } = string.Empty;

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [ForeignKey(nameof(PatientId))]
        public virtual ApplicationUser? Patient { get; set; }

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [ForeignKey(nameof(DoctorId))]
        public virtual ApplicationUser? Doctor { get; set; }

        [Required]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Urgency")]
        public Urgency Urgency { get; set; } = Urgency.Routine;

        [StringLength(2000)]
        [Display(Name = "Clinical Notes")]
        public string? ClinicalNotes { get; set; }



        [Required]
        [StringLength(50)]
        public string Status { get; set; } =
            TestRequestStatus.Submitted.ToString();

        // Doctor's decision and information recorded when releasing results.

        [StringLength(1000)]
        [Display(Name = "Cancellation Reason")]
        public string? CancellationReason { get; set; }

        [StringLength(2000)]
        [Display(Name = "Doctor's Release Note")]
        public string? DoctorReleaseNote { get; set; }

        [Display(Name = "Results Released At")]
        public DateTime? ResultsReleasedAt { get; set; }

        public string? ReleasedByDoctorId { get; set; }

        [ForeignKey(nameof(ReleasedByDoctorId))]
        public virtual ApplicationUser? ReleasedByDoctor { get; set; }

        [StringLength(30)]
        [Display(Name = "Patient Notification Method")]
        public string? PatientNotificationMethod { get; set; }

        // Samples associated with this test request.
        public virtual ICollection<SampleModel> Samples { get; set; }
            = new List<SampleModel>();

        // Legacy/direct test-type relationship, retained for compatibility.
        public virtual ICollection<TestTypeModel> TestTypes { get; set; }
            = new List<TestTypeModel>();

        // Individual test types requested for this request.
        public virtual ICollection<TestRequestTestTypeModel> RequestedTestTypes
        {
            get;
            set;
        } = new List<TestRequestTestTypeModel>();

        // Uses the individual requested test entries when available.
        // Falls back to the legacy TestTypes collection.
        [NotMapped]
        public IReadOnlyCollection<TestTypeModel> EffectiveTestTypes =>
            RequestedTestTypes != null && RequestedTestTypes.Count > 0
                ? RequestedTestTypes
                    .Where(x => x.TestType != null)
                    .Select(x => x.TestType!)
                    .ToList()
                : TestTypes.ToList();
    }

    public enum Urgency
    {
        Routine = 1,
        Urgent = 2,
        Stat = 3
    }

    public enum TestRequestStatus
    {
        Submitted = 1,
        SamplesReceived = 2,
        InProgress = 3,
        Completed = 4,
        Released = 5,
        Cancelled = 6,
        ReleasedByDoctor = 7
    }
}
