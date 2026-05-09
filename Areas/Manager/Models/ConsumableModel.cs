using NMB_HLabSys.Areas.Technician.Models;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class ConsumableModel
    {
        [Key]
        public int ConsumableId { get; set; }
        public string Name { get; set; } = null!;
        public int ReorderLevel { get; set; }
        public int QuantityOnHand { get; set; }
        public ICollection<TestTypeConsumableModel> TestTypeConsumables { get; set; }
    = new List<TestTypeConsumableModel>();
    }
}
