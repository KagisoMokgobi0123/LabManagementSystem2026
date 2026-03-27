using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.LabTechnician.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
