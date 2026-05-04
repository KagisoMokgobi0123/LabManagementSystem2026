namespace NMB_HLabSys.Areas.Technician.Models
{
    public class Dashboard
    {
       
            public List<TestRequestTestTypeModel> SelectedTests { get; set; }
            public List<TestRequestTestTypeModel> PendingSelection { get; set; }
            public List<TestRequestTestTypeModel> PendingVerification { get; set; }
            public List<TestRequestTestTypeModel> ReturnedTests { get; set; }
            public List<TestRequestTestTypeModel> UrgentTests { get; set; }
            public List<TestRequestTestTypeModel> OverdueTests { get; set; }
            public List<TestRequestTestTypeModel> NearDueTests { get; set; }

            // optional filters
            public string UrgencyFilter { get; set; }
            public int? CategoryId { get; set; }
            public string RequestNumber { get; set; }
        
    }
}
