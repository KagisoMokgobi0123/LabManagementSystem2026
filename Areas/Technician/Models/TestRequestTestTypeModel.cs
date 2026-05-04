using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestRequestTestTypeModel
    {
        [Key]
        public int TestRequestTestTypeId { get; set; }

        public int TestRequestId { get; set; }
        public TestRequestsModel TestRequest { get; set; } = null!;

        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; } 

        public string? TechnicianId { get; set; }
        public RegisterUser? Technician { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime StartDateTime { get; set; } 
        public DateTime CompletionDateTime { get; set; } 

        public DateTime? DueDate { get; set; }
        public bool IsOverdue =>
        DueDate < DateTime.Now && Status != "Completed";

        // Result capture
        public string? ResultValue { get; set; }
        public string? Notes { get; set; }

        // Verification
        public string? VerifiedByTechnicianId { get; set; }
        public RegisterUser? VerifyTechnician { get; set; }
        public string? VerificationNote { get; set; }
        public DateTime? VerificationDateTime { get; set; }

        public ICollection<ResultModel> Results { get; set; } = new List<ResultModel>();
    }
}
