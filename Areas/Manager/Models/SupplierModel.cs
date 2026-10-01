using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class SupplierModel
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;


        // ============================================================
        // SOFT DELETE
        // ============================================================

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;


        // ============================================================
        // CONSUMABLES
        // ============================================================

        // One supplier can supply many consumables.
        public virtual ICollection<ConsumableModel>
            Consumables
        { get; set; }
            = new List<ConsumableModel>();


        // ============================================================
        // ORDERS
        // ============================================================

        // One supplier can have many orders.
        public virtual ICollection<OrderModel>
            Orders
        { get; set; }
            = new List<OrderModel>();
    }
}
