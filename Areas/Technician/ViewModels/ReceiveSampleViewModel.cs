using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ReceiveSampleViewModel
    {
        // ============================================================
        // TEST REQUEST
        // ============================================================

        [Required]
        public int TestRequestId { get; set; }

        [Required(ErrorMessage = "Request number is required.")]
        [StringLength(50)]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; } = string.Empty;


        // ============================================================
        // PATIENT / DOCTOR INFORMATION
        // Display only
        // ============================================================

        [Display(Name = "Patient")]
        public string PatientName { get; set; } = string.Empty;

        [Display(Name = "Doctor")]
        public string DoctorName { get; set; } = string.Empty;


        // ============================================================
        // SAMPLE
        // ============================================================

        [Display(Name = "Sample ID")]
        public int SampleID { get; set; }

        [Required(ErrorMessage = "Sample barcode is required.")]
        [StringLength(100)]
        [Display(Name = "Sample Barcode")]
        public string Barcode { get; set; } = string.Empty;


        // ============================================================
        // SAMPLE TYPE
        // ============================================================

        [Required]
        [Range(1, int.MaxValue,
            ErrorMessage = "A valid sample type is required.")]
        [Display(Name = "Sample Type")]
        public int SampleTypeId { get; set; }

        [Display(Name = "Sample Type")]
        public string SampleTypeName { get; set; } = string.Empty;


        // ============================================================
        // RECEIVING TECHNICIAN
        // Display only - actual technician comes from logged-in user.
        // ============================================================

        [Display(Name = "Receiving Technician")]
        public string TechnicianName { get; set; } = string.Empty;


        // ============================================================
        // DATE / TIME RECEIVED
        // ============================================================

        [Required]
        [Display(Name = "Date / Time Received")]
        [DataType(DataType.DateTime)]
        public DateTime DateTimeReceived { get; set; } = DateTime.Now;


        // ============================================================
        // VERIFICATION CHECKLIST
        // All five must be checked before the sample can be received.
        // ============================================================

        [Display(Name = "Verify Request Number")]
        public bool VerifyRequestNumber { get; set; }

        [Display(Name = "Verify Technician")]
        public bool VerifyTechnician { get; set; }

        [Display(Name = "Verify Barcode")]
        public bool VerifyBarcode { get; set; }

        [Display(Name = "Verify Sample Type")]
        public bool VerifySampleType { get; set; }

        [Display(Name = "Verify Date / Time")]
        public bool VerifyDateTime { get; set; }
    }
}