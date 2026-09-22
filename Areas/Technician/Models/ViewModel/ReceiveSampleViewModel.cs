using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ReceiveSampleViewModel
    {
        public int TestRequestId { get; set; }

        public int SampleID { get; set; }

        public string RequestNumber { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string TechnicianName { get; set; } = string.Empty;

        // Actual barcode
        public string Barcode { get; set; } = string.Empty;

        // Selected/actual sample type
        public int SampleTypeId { get; set; }

        // Display name e.g. "Blood"
        public string SampleTypeName { get; set; } = string.Empty;

        // Dropdown options
        public List<SelectListItem> SampleTypes { get; set; } = new();

        public DateTime DateTimeReceived { get; set; } = DateTime.Now;

        // Verification checkboxes
        public bool VerifyRequestNumber { get; set; }
        public bool VerifyTechnician { get; set; }
        public bool VerifyBarcode { get; set; }
        public bool VerifySampleType { get; set; }
        public bool VerifyDateTime { get; set; }
    }
}