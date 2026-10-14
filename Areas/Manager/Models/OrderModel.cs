using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class OrderModel
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;


        [Required]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public virtual SupplierModel? Supplier { get; set; }

        [Required]
        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Ordered;

        [Display(Name = "Date Complete")]
        public DateTime? DateComplete { get; set; }

        [StringLength(500)]
        [Display(Name = "Cancellation Reason")]
        public string? CancellationReason { get; set; }

        [Display(Name = "Date Cancelled")]
        public DateTime? DateCancelled { get; set; }

        // One order contains many order items.
        public virtual ICollection<OrderItemModel>
            OrderItems
        { get; set; }
            = new List<OrderItemModel>();
    }


    public enum OrderStatus
    {
        Ordered,
        PartiallyComplete,
        Complete,
        Cancelled
    }
}
