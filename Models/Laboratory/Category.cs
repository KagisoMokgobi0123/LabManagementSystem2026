using System;
using System.Collections.Generic;
using NMB_HLabSys.Models.Laboratory.Enums;

namespace NMB_HLabSys.Models.Laboratory
{
    /// <summary>
    /// General-purpose category used both to group consumables (categories.html)
    /// and to classify test types (testtype.html "Test Category" dropdown).
    /// </summary>
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public CategoryStatus Status { get; set; } = CategoryStatus.Active;

        public ICollection<TestType> TestTypes { get; set; } = new List<TestType>();
        public ICollection<Inventory.Consumable> Consumables { get; set; } = new List<Inventory.Consumable>();
    }
}
