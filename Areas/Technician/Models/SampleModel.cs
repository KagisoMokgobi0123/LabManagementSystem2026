using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleModel
    {
        [Key]
        public int SampleId { get; set; }
        public DateTime RecievedDateTime { get; set; } = DateTime.Now;
        public int TestRequestId { get; set; }
        public TestRequestModel? TestRequest { get; set; }
        public string TechnicianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; }
        public string BarcodeNumber { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
    }
}
