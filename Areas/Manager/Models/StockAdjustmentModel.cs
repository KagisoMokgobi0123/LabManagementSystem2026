using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class StockAdjustmentModel
    {
        [Key]
        public int StockAdjustmentId { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }

        [Required]
        [Display(Name = "Adjustment Type")]
        public StockAdjustmentType AdjustmentType { get; set; }


        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }


        [Required]
        [Display(Name = "Adjustment Date")]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    }


    public enum StockAdjustmentType
    {
        Increase = 1,
        Decrease = 2,
        Set = 3
    }
}
