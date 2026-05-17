using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class StockTransactionModel
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int ConsumableId { get; set; }
        public ConsumableModel Consumable { get; set; } = null!;

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int PreviousQuantity { get; set; }

        [Required]
        public int NewQuantity { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(200)]
        public string? Reference { get; set; }

        [MaxLength(100)]
        public string? PerformedBy { get; set; }

    
    }

    public enum TransactionType
    {
        Increase,
        Decrease,
        Set,
        OrderReceived
    }
}

