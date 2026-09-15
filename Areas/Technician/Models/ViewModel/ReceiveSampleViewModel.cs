using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ReceiveSampleViewModel
    {
        public int TestRequestId { get; set; }

        public int SampleID { get; set; }

        public string RequestNumber { get; set; } = string.Empty;

        public string? TechnicianName { get; set; }

        public string? PatientName { get; set; }

        public string? DoctorName { get; set; }

        public string Barcode { get; set; } = string.Empty;
        public string? SampleTypeName { get; set; }
        public int SampleTypeId { get; set; }

        public IEnumerable<SelectListItem>? SampleTypes { get; set; }

        public DateTime DateTimeReceived { get; set; }

        // Verification checkboxes
        public bool VerifyRequestNumber { get; set; }

        public bool VerifyTechnician { get; set; }

        public bool VerifyBarcode { get; set; }

        public bool VerifySampleType { get; set; }

        public bool VerifyDateTime { get; set; }
    }
}