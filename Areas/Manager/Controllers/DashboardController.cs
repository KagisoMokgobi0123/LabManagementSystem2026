using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "LabManager")]
    public class DashboardController : Controller
    {
        private readonly ILabManagerService _labManagerService;

        public DashboardController(ILabManagerService labManagerService)
        {
            _labManagerService = labManagerService;
        }

        // GET: /Manager/Dashboard  (index.html)
        public async Task<IActionResult> Index()
        {
            var model = await _labManagerService.GetDashboardAsync();
            return View(model);
        }
    }
}
