using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
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

        [Required(ErrorMessage ="Sample type is required. Please select test type.")]
        public int SampleTypeId { get; set; }// e.g., Blood, Plasma
        public  SampleTypeModel SampleType {  get; set; } = null!;

        public string? TechnicianId { get; set; } = string.Empty;// e.g., Blood, Plasma
        public RegisterUser Technician { get; set; } = null!;

        public DateTime DateReceived { get; set; } = DateTime.Now;

        public string Status { get; set; } = string.Empty;

        public int TestRequestId { get; set; }
        public TestRequestsModel TestRequest { get; set; } = null!;

        public bool IsReceived { get; set; }

        // Soft Delete Properties
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? RestoredAt { get; set; }
        public string? RestoredBy { get; set; }

    }
}
