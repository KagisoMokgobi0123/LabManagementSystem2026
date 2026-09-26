//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NMB_HLabSys.Models.Inventory.Enums;
//using NMB_HLabSys.Services.Interfaces;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class OrdersController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public OrdersController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/Orders  (orders.html)
//        public async Task<IActionResult> Index()
//        {
//            var orders = await _service.GetOrdersAsync();
//            return View(orders);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
//        {
//            await _service.UpdateOrderStatusAsync(orderId, status);
//            TempData["Success"] = "Order status updated successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Cancel(int orderId, string cancelReason)
//        {
//            await _service.CancelOrderAsync(orderId, cancelReason);
//            TempData["Success"] = "Order cancelled.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
