using System.Collections.Generic;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class LabManagerDashboardViewModel
    { // Manager profile public string ManagerName { get; set; } = "Laboratory Manager";

        public string ManagerEmail { get; set; } = string.Empty;

        // Dashboard statistics public int TestTypeCount { get; set; }

        public int TechnicianCount { get; set; }

        public int DoctorCount { get; set; }

        public int ConsumableCount { get; set; }

        public int LowStockCount { get; set; }

        public int OpenOrderCount { get; set; }

        public int SupplierCount { get; set; }
        public string ManagerName { get; internal set; }
        public List<TestCategorySummaryViewModel> TestCategorySummaries { get; internal set; }
        public List<ConsumableStockViewModel> Consumables { get; internal set; }
        public int TestTypeCount { get; internal set; }

        // Test types grouped by category public List<TestCategorySummaryViewModel> TestCategorySummaries { get; set; } = new();

        // Consumable stock overview public List<ConsumableStockViewModel> Consumables { get; set; } = new(); }

        public class TestCategorySummaryViewModel
        {
            public string CategoryName { get; set; } = string.Empty;

            public int TestCount { get; set; }
        }

        public class ConsumableStockViewModel
        {
            public int ConsumableId { get; set; }

            public string ConsumableName { get; set; } = string.Empty;

            public string SupplierName { get; set; } = "Not assigned";

            public int Quantity { get; set; }

            public int ReorderLevel { get; set; }

            public string Status { get; set; } = string.Empty;
        }
    }
}