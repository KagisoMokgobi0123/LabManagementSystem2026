using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Manager.Models;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TestConsumableUsageModel
    {
        [Key]
        public int TestConsumableUsageId { get; set; }

        [Required]
        public int TestRequestTestTypeId { get; set; }

        [ForeignKey(nameof(TestRequestTestTypeId))]
        public virtual TestRequestTestTypeModel? TestRequestTestType
        {
            get;
            set;
        }

        [Required]
        public int ConsumableId { get; set; }

        [ForeignKey(nameof(ConsumableId))]
        public virtual ConsumableModel? Consumable { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityUsed { get; set; }

        [Required]
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UsedByTechnicianId { get; set; } = string.Empty;
    }
}