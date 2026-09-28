using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
