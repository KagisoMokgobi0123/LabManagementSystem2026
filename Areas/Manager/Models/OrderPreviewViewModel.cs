using System.Collections.Generic;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class OrderPreviewViewModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public List<OrderPreviewItem> Items { get; set; } = new List<OrderPreviewItem>();
    }

    public class OrderPreviewItem
    {
        public int ConsumableId { get; set; }
        public string ConsumableName { get; set; }
        public int Quantity { get; set; }
    }
}
