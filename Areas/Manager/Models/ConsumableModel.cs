using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public enum StockStatus
    {
        Sufficient = 1,
        ReorderNow = 2,
        OutOfStock = 3
    }

    public class ConsumableModel
    {
        [Key]
        public int ConsumableId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Consumable Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Quantity On Hand")]
        public int QuantityOnHand { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        // ============================================================
        // CURRENT STOCK STATUS
        // ============================================================

        [Required]
        [Display(Name = "Stock Status")]
        public StockStatus StockStatus { get; set; }
            = StockStatus.Sufficient;


        // ============================================================
        // SOFT DELETE
        // ============================================================

        public bool IsDeleted { get; set; } = false;


        // ============================================================
        // SUPPLIER
        // ============================================================

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public virtual SupplierModel? Supplier { get; set; }


        // ============================================================
        // TEST TYPES
        // ============================================================

        public virtual ICollection<TestTypeConsumableModel>
            TestTypeConsumables
        { get; set; }
            = new List<TestTypeConsumableModel>();


        // ============================================================
        // ORDER ITEMS
        // ============================================================

        public virtual ICollection<OrderItemModel>
            OrderItems
        { get; set; }
            = new List<OrderItemModel>();


        // ============================================================
        // STOCK ADJUSTMENTS
        // ============================================================

        public virtual ICollection<StockAdjustmentModel>
            StockAdjustments
        { get; set; }
            = new List<StockAdjustmentModel>();


        // ============================================================
        // STOCK STATUS HISTORY
        // ============================================================

        public virtual ICollection<StockStatusHistoryModel>
            StockStatusHistories
        { get; set; }
            = new List<StockStatusHistoryModel>();
    }
}
