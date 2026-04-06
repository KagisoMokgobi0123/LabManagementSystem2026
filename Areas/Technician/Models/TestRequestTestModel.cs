using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestRequestTestModel
    {
        [Key]
        public int TestRequestTestId { get; set; }

        public int TestRequestId { get; set; }
        public TestRequestModel? TestRequest { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; } 

        public string? TechnicianId { get; set; }
        public RegisterUser? Technician { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime StartDateTime { get; set; } = DateTime.Now;
        public DateTime? CompletionDateTime { get; set; } =  DateTime.Now;
        public ICollection<ResultModel> Results { get; set; } = new List<ResultModel>();
        public ICollection<VerificationModel> Verifications { get; set; } = new List<VerificationModel>();
    }
}
