namespace NMB_HLabSys.Patterns.Observer
{
    public interface ITestRequestObserver
    {
        void OnStatusChanged(TestRequestStatusChangedEvent e);
    }

    // Event object passed to all observers when status changes
    public class TestRequestStatusChangedEvent
    {
        public int TestRequestId { get; set; }
        public string PatientEmail { get; set; }
        public string PatientName { get; set; }
        public string DoctorEmail { get; set; }
        public string DoctorName { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public byte[] ResultPdfBytes { get; set; } // populated only when status = Verified
    }
}
