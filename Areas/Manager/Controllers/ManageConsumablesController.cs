//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NMB_HLabSys.Models.Inventory;
//using NMB_HLabSys.Areas.Manager.Models;
//using NMB_HLabSys.Services.Interfaces;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class ManageConsumablesController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public ManageConsumablesController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/ManageConsumables  (manageconsumables.html)
//        [HttpGet]
//        public async Task<IActionResult> GetConsumables(int supplierId)
//        {
//            var items = await _service.GetConsumablesBySupplierAsync(supplierId);

//            return Json(items.Select(x => new
//            {
//                id = x.ConsumableId,
//                name = x.Consumable.Name
//            }));
//        }
//        public async Task<IActionResult> Index()
//        {
//            ViewBag.Consumables = await _service.GetConsumablesAsync();
//            ViewBag.Suppliers = await _service.GetSuppliersAsync();

//            var managed = await _service.GetManagedConsumablesAsync();
//            return View(managed);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(ManageConsumable managed)
//        {
//            if (!ModelState.IsValid)
//            {
//                foreach (var state in ModelState)
//                {
//                    foreach (var error in state.Value.Errors)
//                    {
//                        Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
//                    }
//                }

//                return RedirectToAction(nameof(Index));
//            }

//            try
//            {
//                await _service.CreateManagedConsumableAsync(managed);
//                TempData["Success"] = "Consumable is now being managed.";
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
//            }

//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(ManageConsumable managed)
//        {
//            if (!ModelState.IsValid)
//                return RedirectToAction(nameof(Index));

//            await _service.UpdateManagedConsumableAsync(managed);

//            TempData["Success"] = "Consumable stock updated successfully.";

//            return RedirectToAction(nameof(Index));
//        }

//        // Builds/submits the multi-item order preview (order cart) for one supplier
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SubmitOrder(OrderPreviewViewModel preview)
//        {
//            if (preview.Items == null || preview.Items.Count == 0)
//            {
//                TempData["Error"] = "Add at least one item to the order before submitting.";
//                return RedirectToAction(nameof(Index));
//            }


//            await _service.CreateOrderPreviewAsync(preview);
//            TempData["Success"] = $"Order submitted to {preview.SupplierName} successfully.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
