using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ConsumableModel
    {
        [Key]
        public int ConsumableId { get; set; }
        public int QuantityAvailable { get; set; } 
    }
}
