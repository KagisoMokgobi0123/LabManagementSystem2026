using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestTypeModel
    {
        [Key]
        public int TestTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TechnicianId { get; set; } = string.Empty;
        public RegisterUser? Technician { get; set; } 
        public DateTime TurnAroundTime { get; set; } = DateTime.Now;
    }
}
