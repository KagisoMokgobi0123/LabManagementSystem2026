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

        public string Status { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public RegisterUser Patient { get; set; } = null!;


        [Required]
        [Display(Name = "Request Number")]
        // This property isn't in the DB, it just formats the ID for the UI
        [NotMapped]
        public string RequestNumber => $"NMB-REQ-{TestRequestId:D6}";
        // Result: NMB-REQ-000001

        public string DoctorId { get; set; } = String.Empty;
        public RegisterUser Doctor { get; set; } = null!;


        [Required(ErrorMessage = "Please select the level of urgency.")]
        public string Urgency { get; set; } = "Routine"; // Default value


        [Display(Name = "Clinical Notes")]
        [DataType(DataType.MultilineText)]
        public string? ClinicalNotes { get; set; }


        public ICollection<SampleModel> Samples { get; set; } = new List<SampleModel>();
        public ICollection<TestTypeModel> TestTypes { get; set; } = new List<TestTypeModel>(); public ICollection<TestRequestsModel> TestRequests { get; set; } = new List<TestRequestsModel>();
    }
}
