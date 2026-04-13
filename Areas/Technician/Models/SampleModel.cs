using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleModel
    {
        [Key]
        public int SampleId { get; set; }

        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; } = string.Empty;// Read-only display

        [Required]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public string SampleType { get; set; } = string.Empty; // e.g., Blood, Plasma

        public DateTime DateReceived { get; set; } = DateTime.Now;

        // Foreign Keys
        public int TestRequestId { get; set; }
        public TestRequestsModel? TestRequest { get; set; }

        public string TechnicianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; }
    }
}
