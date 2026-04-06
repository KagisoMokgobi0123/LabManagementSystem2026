using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleModel
    {
        [Key]
        public int SampleId { get; set; }
        public DateTime RecievedDateTime { get; set; } = DateTime.Now;
        public TestRequestModel? Request { get; set; }
        public string TechcianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; }
        public int BarcodeNumber { get; set; } 
        public string SampleType { get; set; } = string.Empty;
    }
}
