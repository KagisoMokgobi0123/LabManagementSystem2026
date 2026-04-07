using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestRequestModel
    {
        [Key]
        public int TestRequestId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string PatientId { get; set; } = String.Empty;
        public RegisterUser? Patient { get; set; }
        [Required]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; } = string.Empty; 
        public string DoctorId { get; set; } = String.Empty;
        public RegisterUser? Doctor { get; set; }

        public ICollection<TestRequestTestModel> Tests { get; set; }
        public ICollection<SampleModel> Samples { get; set; }
        public ICollection<ClinicalNotesModel> ClinicalNotes { get; set; }
    }
}
