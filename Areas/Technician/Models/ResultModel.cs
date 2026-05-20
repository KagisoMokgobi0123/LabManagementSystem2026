using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Areas.Doctor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ResultModel
    {
        [Key]
        public int ResultId { get; set; }

        public DateTime CapturedDateTime { get; set; }

        public DateTime IssuedDate { get; set; } = DateTime.Now;

        public string TechnicianId { get; set; } = string.Empty;

        public RegisterUser Technician { get; set; } = null!;

        public string ResultValues { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public bool IsAbnormal { get; set; } = false;

        // Foreign key to TestRequestItem
        public int TestRequestItemId { get; set; }

        [ForeignKey(nameof(TestRequestItemId))]
        public TestRequestItem TestRequestItem { get; set; } = null!;
    }
}