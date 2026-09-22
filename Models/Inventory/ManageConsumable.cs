using System;
using NMB_HLabSys.Models.Inventory.Enums;

namespace NMB_HLabSys.Models.Inventory
{
    public class ManageConsumable
    {
        public int ManageConsumableId { get; set; }


        public int? ConsumableId { get; set; }

        public Consumable? Consumable { get; set; } = null!;


        public int? SupplierId { get; set; }

        public Supplier? Supplier { get; set; }


        public int ReorderLevel { get; set; }

        public int QuantityOnHand { get; set; }


        public ManageConsumableStatus Status { get; set; }
            = ManageConsumableStatus.Empty;


        public ConsumableStatus IsActive { get; set; }
            = ConsumableStatus.Active;


        public DateTime DateUpdated { get; set; }
            = DateTime.UtcNow;


        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}