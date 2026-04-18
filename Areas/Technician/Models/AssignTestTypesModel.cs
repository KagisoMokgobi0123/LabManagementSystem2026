using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class AssignTestTypesModel
    {
        [Key]
        public int AssignTestId { get; set; }

        [Required]
        public int TestRequestId { get; set; }
        public TestRequestsModel? TestRequest { get; set; }

        [Required]
        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; }

        public string? TechnicianId { get; set; }
        public RegisterUser? Technician { get; set; }

        public string ManagerId { get; set; } = string.Empty;
        public RegisterUser? Manager { get; set; }

        // Set only when test starts
        public DateTime? StartDateTime { get; set; }

        // Set only when completed
        public DateTime? CompletionDateTime { get; set; }

        [Required]
        public TestStatus Status { get; set; } = TestStatus.InProgress;
    }
    public enum TestStatus
    {
        
        Submitted,
        InProgress,
        Completed,
        SampleReceived
    }
}
