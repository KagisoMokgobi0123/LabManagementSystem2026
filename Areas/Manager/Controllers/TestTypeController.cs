//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NMB_HLabSys.Models.Laboratory;
//using NMB_HLabSys.Services.Interfaces;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//     [Authorize(Roles = "LabManager")]
//    public class TestTypeController : Controller
//    {
//        private readonly ILabManagerService _service;

//        public TestTypeController(ILabManagerService service)
//        {
//            _service = service;
//        }

//        // GET: /Manager/TestType  (testtype.html)
//        public async Task<IActionResult> Index()
//        {
//            ViewBag.Categories = await _service.GetCategoriesAsync();
//            ViewBag.SampleTypes = await _service.GetSampleTypesAsync();

//            var testTypes = await _service.GetTestTypesAsync();
//            return View(testTypes);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(TestType testType)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.CreateTestTypeAsync(testType);
//            TempData["Success"] = $"Test type '{testType.Name}' added successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(TestType testType)
//        {
//            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
//            await _service.UpdateTestTypeAsync(testType);
//            TempData["Success"] = $"Test type '{testType.Name}' updated successfully.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _service.DeleteTestTypeAsync(id);
//            TempData["Success"] = "Test type removed successfully.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
