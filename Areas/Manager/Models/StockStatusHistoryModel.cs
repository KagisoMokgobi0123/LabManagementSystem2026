using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class StockStatusHistoryModel
    {
        [Key]
        public int StockStatusHistoryId { get; set; }


        // ============================================================
        // CONSUMABLE
        // ============================================================

        [Required]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }


        // ============================================================
        // STOCK INFORMATION AT TIME OF RECORD
        // ============================================================

        [Required]
        [Display(Name = "Stock Status")]
        public StockStatus StockStatus { get; set; }


        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Quantity On Hand")]
        public int QuantityOnHand { get; set; }


        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }


        // ============================================================
        // DATE RECORDED
        // ============================================================

        [Required]
        [Display(Name = "Date Recorded")]
        public DateTime DateRecorded { get; set; } = DateTime.Now;
    }
}
