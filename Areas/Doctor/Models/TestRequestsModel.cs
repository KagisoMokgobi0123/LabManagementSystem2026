using Microsoft.Extensions.Diagnostics.HealthChecks;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Patterns.Observer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public enum Urgency
    {
        Routine,
        Urgent,
        Stat
    }
    public enum TestRequestStatus
    {
        Submitted,
        SamplesReceived,
        InProgress,
        Completed,
        ReleasedByDoctor,
        Cancelled
    }
    public class TestRequestsModel
    {
        [Key]
        public int TestRequestId { get; set; }

        public string PatientId { get; set; } = string.Empty;
        public RegisterUser Patient { get; set; } = null!;

        [Required]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; } = String.Empty;


        [Required]
        [ForeignKey("TestType")]
        public int TestTypeId { get; set; }
        public TestTypeModel TestType { get; set; }


        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public Urgency Urgency { get; set; }

        public string ClinicalNotes { get; set; } = String.Empty;

        [Required]
        public string Status { get; set; } = String.Empty;

        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; }

        //public ICollection<SampleModel> Samples { get; set; } = new List<SampleModel>(); // uncomment after migration
        // public List<TestRequestTestTypeModel> TechnicianTests { get; set; } = new(); // uncomment after migration

        public virtual ICollection<ConsentRequestItem> ConsentRequestItems { get; set; } = new List<ConsentRequestItem>();
        public virtual ICollection<TestRequestItem> TestRequestItems { get; set; } = new List<TestRequestItem>();
        public ICollection<SampleModel> Samples { get; set; }
     = new List<SampleModel>();

        public List<TestRequestTestTypeModel> TechnicianTests { get; set; } = new();

        public string Barcode { get; set; } = String.Empty;
        public int? ResultReleaseId { get; set; }
        public ResultRelease? ResultRelease { get; set; }

        // ── Doctor foreign key (string to match ASP.NET Identity) ──
        [Display(Name = "Doctor")]
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; } = string.Empty;
        public RegisterUser Doctor { get; set; } = null!;

        // --- Observer list ---
        private readonly List<ITestRequestObserver> _observers = new();

        public void AddObserver(ITestRequestObserver observer)
            => _observers.Add(observer);

        public void RemoveObserver(ITestRequestObserver observer)
            => _observers.Remove(observer);

        // --- The one method that drives all notifications ---
        public void ChangeStatus(TestRequestStatus newStatus, byte[] resultPdf = null)
        {
            var e = new TestRequestStatusChangedEvent
            {
                TestRequestId = TestRequestId,
                PatientEmail = Patient.Email,
                PatientName = Patient.FullName,
                OldStatus = Status.ToString(),
                NewStatus = newStatus.ToString(),
                ResultPdfBytes = resultPdf
            };

            Status = newStatus.ToString();

            foreach (var observer in _observers)
                observer.OnStatusChanged(e);
        }
    }
}