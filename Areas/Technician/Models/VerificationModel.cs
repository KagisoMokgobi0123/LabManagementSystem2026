using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class VerificationModel
    {
        [Key]
        public int VerificationId { get; set; }
        public int RequestId { get; set; }
        public TestRequestModel? Request { get;set;  }
        public DateTime VerificationDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }
}
