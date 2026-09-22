using System.Collections.Generic;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalConsumables { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockItems { get; set; }

        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
    }
}
