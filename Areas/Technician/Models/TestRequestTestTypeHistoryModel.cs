namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestRequestTestTypeHistoryModel
    {
        public int Id { get; set; }

        public int TestRequestTestTypeId { get; set; }

        public string ResultValue { get; set; }
        public string Notes { get; set; }

        public string ActionType { get; set; } // e.g. Captured, Returned, Reworked

        public string ActionByTechnicianId { get; set; }

        public DateTime ActionDateTime { get; set; }

        // optional navigation
        public TestRequestTestTypeModel TestRequestTestType { get; set; }
    }
}

