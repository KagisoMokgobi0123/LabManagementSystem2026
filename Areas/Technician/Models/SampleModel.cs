using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleModel
    {
        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int SampleId { get; set; }


        // ============================================================
        // TEST REQUEST
        // ============================================================

        [Required]
        public int TestRequestId { get; set; }

        [ForeignKey(nameof(TestRequestId))]
        public virtual TestRequestsModel? TestRequest { get; set; }


        // ============================================================
        // SAMPLE BARCODE
        // ============================================================

        [Required]
        [StringLength(100)]
        [Display(Name = "Sample Barcode")]
        public string Barcode { get; set; } = string.Empty;


        // ============================================================
        // SAMPLE TYPE
        // ============================================================

        [Required]
        [Display(Name = "Sample Type")]
        public int SampleTypeId { get; set; }

        [ForeignKey(nameof(SampleTypeId))]
        public virtual SampleTypeModel? SampleType { get; set; }


        // ============================================================
        // RECEIVING TECHNICIAN
        //
        // Null until the physical sample is received.
        // ============================================================

        [Display(Name = "Technician Receiving Sample")]
        public string? TechnicianId { get; set; }

        [ForeignKey(nameof(TechnicianId))]
        public virtual ApplicationUser? Technician { get; set; }


        // ============================================================
        // DATE / TIME RECEIVED
        //
        // Null while the sample is still waiting for reception.
        // ============================================================

        [Display(Name = "Date / Time Received")]
        public DateTime? DateReceived { get; set; }


        // ============================================================
        // SAMPLE STATUS
        //
        // Examples:
        // Submitted
        // Samples Received
        // ============================================================

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Submitted";


        // ============================================================
        // RECEPTION STATE
        // ============================================================

        [Display(Name = "Received")]
        public bool IsReceived { get; set; } = false;


        // ============================================================
        // SOFT DELETE
        // ============================================================

        public bool IsDeleted { get; set; } = false;
    }
}