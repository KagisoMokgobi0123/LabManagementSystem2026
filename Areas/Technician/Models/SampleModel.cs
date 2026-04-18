using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleModel
    {
        [Key]
        public int SampleId { get; set; }

        [Display(Name = "Request Number")]
        // This property isn't in the DB, it just formats the ID for the UI
        [NotMapped]
        public string RequestNumber => $"NMB-REQ-{TestRequestId:D6}";
        // Result: NMB-REQ-000001
        [Required]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public string SampleType { get; set; } = string.Empty; // e.g., Blood, Plasma

        public DateTime DateReceived { get; set; } = DateTime.Now;

        // Foreign Keys
        public int TestRequestId { get; set; }
        public TestRequestsModel? TestRequest { get; set; }

      
    }
}
