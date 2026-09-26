using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;

        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ReportFilterViewModel> GetOrderReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _db.Orders
                .Include(o => o.Consumable).ThenInclude(c => c.Category)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderedDate <= endDate.Value);

            var orders = await query.OrderByDescending(o => o.OrderedDate).ToListAsync();

            var result = new ReportFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Rows = orders.Select(o => new ReportRow
                {
                    ConsumableName = o.Consumable.Name,
                    CategoryName = o.Consumable.Category != null ? o.Consumable.Category.Name : "Uncategorised",
                    QuantityOrdered = o.QtyOrdered,
                    QuantityDelivered = o.QtyDelivered ?? 0,
                    Date = o.OrderedDate,
                    Status = o.Status.ToString()
                }).ToList()
            };

            result.CategoryTotals = orders
                .GroupBy(o => o.Consumable.Category != null ? o.Consumable.Category.Name : "Uncategorised")
                .ToDictionary(g => g.Key, g => g.Sum(o => o.QtyOrdered));

            return result;
        }
    }
}
