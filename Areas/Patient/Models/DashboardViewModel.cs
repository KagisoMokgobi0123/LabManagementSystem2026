using NMB_HLabSys.Areas.Doctor.Models;
namespace NMB_HLabSys.Areas.Patient.Models

{
    public class DashboardViewModel
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientSurname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int ReleasedRequests { get; set; }
        public bool HasAbnormalResults { get; set; }
        public List<Doctor.Models.TestRequestsModel> RecentRequests { get; set; } = new List<Doctor.Models.TestRequestsModel>();
    }
}