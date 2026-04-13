using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.LabManager.Models
{
    public class AssignTestTypes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TestRequestId { get; set; }
        public TestRequestsModel? TestRequest { get; set; }

        [Required]
        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; }

        [Required]
        public string TechnicianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; }

        // Set only when test starts
        public DateTime? StartDateTime { get; set; }

        // Set only when completed
        public DateTime? CompletionDateTime { get; set; }

        [Required]
        public TestStatus Status { get; set; } = TestStatus.Pending;
    }
    public enum TestStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
