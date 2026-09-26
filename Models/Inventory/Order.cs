using System;
using NMB_HLabSys.Models.Inventory.Enums;

namespace NMB_HLabSys.Models.Inventory
{
    /// <summary>
    /// A purchase order line raised against a supplier for a consumable
    /// (see orders.html / manageconsumables.html order preview / completedorders.html).
    /// </summary>
    public class Order
    {
        public int OrderId { get; set; }

        public int ConsumableId { get; set; }
        public Consumable Consumable { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredDate { get; set; }

        public int QtyOrdered { get; set; }
        public int? QtyDelivered { get; set; }
        public int ManageConsumableId { get; set; }

        public ManageConsumable ManageConsumable { get; set; } = null!;

        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.AwaitingDelivery;

        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
