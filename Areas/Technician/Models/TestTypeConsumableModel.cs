
using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Models.Laboratory;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestTypeConsumableModel
    {
        [Key]
        public int TestTypeConsumableId { get; set; }
        public int TestTypeId { get; set; }
        public TestType? TestType { get; set; }
        public int ConsumableId { get; set; }
        public Consumable Consumable { get; set; } = null!;
        public DateTime VerificationDate { get; set; } = DateTime.Now;
        public int QuantityRequired { get; set; } 

    }
}
