using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
     [Authorize(Roles = "LabManager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: /Manager/Reports  (reports.html)
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var model = await _reportService.GetOrderReportAsync(startDate, endDate);
            return View(model);
        }
    }
}
