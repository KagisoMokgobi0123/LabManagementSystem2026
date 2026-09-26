//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NMB_HLabSys.Models.Inventory;
//using NMB_HLabSys.Services.Interfaces;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class ConsumablesController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public ConsumablesController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/Consumables  (consumables.html)
//        public async Task<IActionResult> Index()
//        {
//            ViewBag.Categories = await _service.GetCategoriesAsync();
//            var consumables = await _service.GetConsumablesAsync();
//            return View(consumables);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Consumable consumable)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.CreateConsumableAsync(consumable);
//            TempData["Success"] = $"Consumable '{consumable.Name}' added successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Consumable consumable)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.UpdateConsumableAsync(consumable);
//            TempData["Success"] = $"Consumable '{consumable.Name}' updated successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _service.DeleteConsumableAsync(id);
//            TempData["Success"] = "Consumable removed successfully.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
