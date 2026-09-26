//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class CategoriesController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public CategoriesController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/Categories  (categories.html)
//        public async Task<IActionResult> Index()
//        {
//            var categories = await _service.GetCategoriesAsync();
//            return View(categories);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Category category)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.CreateCategoryAsync(category);
//            TempData["Success"] = $"Category '{category.Name}' added successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Category category)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.UpdateCategoryAsync(category);
//            TempData["Success"] = $"Category '{category.Name}' updated successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _service.DeleteCategoryAsync(id);
//            TempData["Success"] = "Category removed successfully.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
