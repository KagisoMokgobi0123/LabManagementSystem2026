using NMB_HLabSys.Areas.Manager.Models;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestTypeConsumableModel
    {
        [Key]
        public int TestTypeConsumableId { get; set; }
        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; }
        public int ConsumableId { get; set; }
        public ConsumableModel Consumable { get; set; } = null!;
        public DateTime VerificationDate { get; set; } = DateTime.Now;
        public int QuantityRequired { get; set; } 

    }
}
