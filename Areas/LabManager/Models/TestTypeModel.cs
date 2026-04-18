using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.LabManager.Models
{
    public class TestTypeModel
    {
        [Key]
        public int TestTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TurnAroundTime { get; set; } = DateTime.Now;
    }
}
