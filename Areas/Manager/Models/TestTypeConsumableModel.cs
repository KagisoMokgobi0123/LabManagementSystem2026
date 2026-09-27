using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Account;
 

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeConsumableModel
    {
        [Key]
        public int TestTypeConsumableId { get; set; }


        // ============================================================
        // TEST TYPE
        // ============================================================

        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))]
        public virtual TestTypeModel? TestType { get; set; }


        // ============================================================
        // CONSUMABLE
        // ============================================================

        [Required]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }


        // ============================================================
        // QUANTITY REQUIRED FOR ONE TEST
        // ============================================================

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity Required")]
        public int QuantityRequired { get; set; } = 1;
    }
}