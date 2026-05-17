namespace NMB_HLabSys.Areas.Technician.Models.ViewModel
{
    public class SampleListViewModel
    {   
        public int SampleId { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string SampleTypeName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string TechnicianId { get; set; } = string.Empty;
        public DateTime DateReceived { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsReceived { get; set; }
        public string Urgency { get; set; } = string.Empty;
    }
}
