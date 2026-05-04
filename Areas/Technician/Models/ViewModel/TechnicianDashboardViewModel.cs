namespace NMB_HLabSys.Areas.Technician.Models.ViewModel
{
    public class TechnicianDashboardViewModel
    {
        // 🟢 Tests currently being worked on
        public List<TestRequestTestTypeModel> SelectedTests { get; set; }

        // 🟡 Unassigned / waiting to be picked up
        public List<TestRequestTestTypeModel> PendingSelection { get; set; }

        // 🔵 Waiting for verification (completed tests)
        public List<TestRequestTestTypeModel> PendingVerification { get; set; }

        // 🟠 Returned for rework by verifier
        public List<TestRequestTestTypeModel> ReturnedTests { get; set; }

        // 🔴 Urgent (STAT) tests
        public List<TestRequestTestTypeModel> UrgentTests { get; set; }

        // ⚫ Overdue tests
        public List<TestRequestTestTypeModel> OverdueTests { get; set; }

        // 🟣 Nearly due tests (approaching turnaround limit)
        public List<TestRequestTestTypeModel> NearDueTests { get; set; }

        // 📌 Optional filters (for UI controls)
        public string? UrgencyFilter { get; set; }
        public string? CategoryFilter { get; set; }
        public string? RequestNumber { get; set; }
        public DateTime? DueBefore { get; set; }
    }
}
