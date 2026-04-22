using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.ViewModels
{
    public class ReceiveSampleViewModel
    {
        public int TestRequestId { get; set; }

        [Display(Name = "Request Number")]
        public string? RequestNumber { get; set; }

        [Required(ErrorMessage = "Please scan or enter the sample barcode.")]
        [Display(Name = "Sample Barcode")]
        public string Barcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a sample type.")]
        [Display(Name = "Sample Type")]
        public string SampleType { get; set; } = string.Empty;
    }
}
