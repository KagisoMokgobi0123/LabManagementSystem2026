using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Patient.Models;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientReportViewModel
    {
        // Patient details
        public PatientModel Patient { get; set; }

        // Date range the report covers
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        // All test requests in that date range
        public List<TestRequestsModel> TestRequests { get; set; }
            = new List<TestRequestsModel>();

        // Summary counts
        public int TotalTests { get; set; }
        public int CompletedTests { get; set; }
        public int AbnormalResults { get; set; }

        // Report generated date
        public DateTime GeneratedOn { get; set; } = DateTime.Now;
    }
}