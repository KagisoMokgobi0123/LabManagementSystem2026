namespace NMB_HLabSys.Areas.Technician.Models.ViewModel
{

    public class TechnicianDashboardViewModel
    {
        public List<TestRequestTestTypeModel> SelectedTests { get; set; } = new();
        public List<TestRequestTestTypeModel> PendingSelection { get; set; } = new();
        public List<TestRequestTestTypeModel> PendingVerification { get; set; } = new();
        public List<TestRequestTestTypeModel> ReturnedTests { get; set; } = new();
        public List<TestRequestTestTypeModel> UrgentTests { get; set; } = new();
        public List<TestRequestTestTypeModel> OverdueTests { get; set; } = new();
        public List<TestRequestTestTypeModel> NearDueTests { get; set; } = new();

        public string? UrgencyFilter { get; set; }
        public string? RequestNumber { get; set; }

        public int TotalAssigned { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalVerified { get; set; }
    }
}
