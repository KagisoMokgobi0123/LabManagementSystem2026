using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class CreateOrderViewModel
    {
        // ============================================================
        // ORDER
        // ============================================================

        [Required(ErrorMessage = "Order number is required.")]
        [StringLength(
            50,
            MinimumLength = 1,
            ErrorMessage = "Order number must be between 1 and 50 characters.")]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;


        // ============================================================
        // SUPPLIER
        // ============================================================

        [Required(ErrorMessage = "Please select a supplier.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a valid supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }


        // ============================================================
        // ORDER ITEMS
        // ============================================================

        [Required(ErrorMessage = "Please add at least one consumable.")]
        [MinLength(
            1,
            ErrorMessage = "Please add at least one consumable.")]
        public List<CreateOrderItemViewModel> Items { get; set; }
            = new List<CreateOrderItemViewModel>();
    }


    public class CreateOrderItemViewModel
    {
        // ============================================================
        // CONSUMABLE
        // ============================================================

        [Required]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a valid consumable.")]
        public int ConsumableId { get; set; }


        // ============================================================
        // QUANTITY
        // ============================================================

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Quantity must be at least 1.")]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }
    }
}
