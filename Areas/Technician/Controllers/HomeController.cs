using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
