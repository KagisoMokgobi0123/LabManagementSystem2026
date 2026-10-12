namespace NMB_HLabSys.Areas.Technician.ViewModels
{
    public class TechnicianAssignmentRowViewModel
    {
        public int TestTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? SampleType { get; set; }
        public bool IsActive { get; set; }
    }
}

