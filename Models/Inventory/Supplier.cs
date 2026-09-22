using System;
using System.Collections.Generic;
using NMB_HLabSys.Models.Inventory.Enums;

namespace NMB_HLabSys.Models.Inventory
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public SupplierStatus Status { get; set; } = SupplierStatus.Active;

        public ICollection<ManageConsumable> ManagedConsumables { get; set; } = new List<ManageConsumable>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
