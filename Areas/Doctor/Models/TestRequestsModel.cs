using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public enum Urgency
    {
        Routine,
        Urgent,
        STAT
    }
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
        public Urgency Urgency { get; set; } = Urgency.Routine;

        [Display(Name = "Clinical Notes")]
        [DataType(DataType.MultilineText)]
        public string? ClinicalNotes { get; set; }


        public ICollection<SampleModel> Samples { get; set; } = new List<SampleModel>();
        public List<TestRequestTestTypeModel> TestTypes { get; set; } = new();
    }
}
