//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NMB_HLabSys.Services.Interfaces;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class CompletedOrdersController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public CompletedOrdersController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/CompletedOrders  (completedorders.html)
//        public async Task<IActionResult> Index()
//        {
//            var orders = await _service.GetOrdersAsync(completedOnly: true);
//            return View(orders);
//        }
//    }
//}
