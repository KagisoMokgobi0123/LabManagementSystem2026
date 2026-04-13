using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class TestRequestsModel
    {
        [Key]
        public int TestRequestId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string PatientId { get; set; } = String.Empty;
        public RegisterUser? Patient { get; set; }
        [Required]
        [Display(Name = "Request Number")]
        // This property isn't in the DB, it just formats the ID for the UI
        [NotMapped]
        public string RequestNumber => $"NMB-REQ-{TestRequestId:D6}";
        // Result: NMB-REQ-000001
        public string DoctorId { get; set; } = String.Empty;
        public RegisterUser? Doctor { get; set; }

        public ICollection<TestRequestTestModel> Tests { get; set; } = new List<TestRequestTestModel>();
        public ICollection<AssignTestTypes> TestAssignments { get; set; } = new List<AssignTestTypes>();
        public ICollection<SampleModel> Samples { get; set; } = new List<SampleModel>();
        public ICollection<ClinicalNotesModel> ClinicalNotes { get; set; } = new List<ClinicalNotesModel>();
    }
}
