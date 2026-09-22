using System.Threading.Tasks;
using NMB_HLabSys.Areas.Manager.Models;

namespace NMB_HLabSys.Services.Interfaces
{
    /// <summary>
    /// Builds the consumable/order reporting data shown on reports.html,
    /// including the category breakdown chart and the date-filtered table.
    /// </summary>
    public interface IReportService
    {
        Task<ReportFilterViewModel> GetOrderReportAsync(System.DateTime? startDate, System.DateTime? endDate);
    }
}
