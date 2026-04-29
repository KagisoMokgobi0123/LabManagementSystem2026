using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.ViewModels
{
    public class ReceiveSampleViewModel
    {
        [Required]
        public string RequestNumber { get; set; } = string.Empty;

        [Required]
        public string Barcode { get; set; } = string.Empty;
        public int RequestId { get; set; }

        public string SampleType { get; set; } = string.Empty;

        public DateTime ReceivedDateTime { get; set; } = DateTime.Now;

        public string TechnicianId { get; set; } = string.Empty;
    }
}
