using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class OrderItemModel
    {
        [Key]
        public int OrderItemId { get; set; }


        // ============================================================
        // ORDER
        // ============================================================

        [Required]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual OrderModel? Order { get; set; }


        // ============================================================
        // CONSUMABLE
        // ============================================================

        [Required]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }


        // ============================================================
        // QUANTITY
        // ============================================================

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }


        // ============================================================
        // ITEM STATUS
        // ============================================================

        [Required]
        [Display(Name = "Item Status")]
        public OrderItemStatus Status { get; set; } = OrderItemStatus.Ordered;


        // ============================================================
        // RECEIVED
        // ============================================================

        [Display(Name = "Date Received")]
        public DateTime? DateReceived { get; set; }


        // ============================================================
        // CANCELLATION
        // ============================================================

        [StringLength(500)]
        [Display(Name = "Cancellation Reason")]
        public string? CancellationReason { get; set; }

        [Display(Name = "Date Cancelled")]
        public DateTime? DateCancelled { get; set; }
    }


    public enum OrderItemStatus
    {
        Ordered = 1,
        Received = 2,
        Cancelled = 3
    }
}
