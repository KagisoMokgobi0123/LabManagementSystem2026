using System;
using System.Collections.Generic;
using NMB_HLabSys.Models.Inventory.Enums;
using NMB_HLabSys.Models.Laboratory;

namespace NMB_HLabSys.Models.Inventory
{
    /// <summary>
    /// Master catalogue entry for a consumable item (see consumables.html).
    /// Stock-level tracking for a consumable lives in <see cref="ManageConsumable"/>.
    /// </summary>
    public class Consumable
    {
        public int ConsumableId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public DateTime DateAdded { get; set; }

        public ConsumableStatus Status { get; set; }

        public ManageConsumable? Inventory { get; set; }
    }
}
