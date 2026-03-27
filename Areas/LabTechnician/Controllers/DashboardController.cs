using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.LabTechnician.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
