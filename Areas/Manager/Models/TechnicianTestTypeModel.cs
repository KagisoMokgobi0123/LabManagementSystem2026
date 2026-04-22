using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.LabManager.Models
{
    public class TechnicianTestTypeModel
    {
        [Key]
        public int TechnicianTestTypeId { get; set; }
        public int TestTypeId { get; set; }
        public TestTypeModel TestType { get; set; } = null!;
        public string TechnicianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; }
        public string ManagerId { get; set; } = string.Empty;
        public RegisterUser? Manager { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime AssignmentDate { get; set; } = DateTime.Now;
    }
}
