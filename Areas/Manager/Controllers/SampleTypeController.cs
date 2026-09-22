using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Models.Laboratory;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
     [Authorize(Roles = "LabManager")]
    public class SampleTypeController : Controller
    {
        private readonly ILabManagerService _service;

        public SampleTypeController(ILabManagerService service)
        {
            _service = service;
        }

        // GET: /Manager/SampleType  (sampletype.html)
        public async Task<IActionResult> Index()
        {
            var sampleTypes = await _service.GetSampleTypesAsync();
            return View(sampleTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SampleType sampleType)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _service.CreateSampleTypeAsync(sampleType);
            TempData["Success"] = $"Sample type '{sampleType.Name}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SampleType sampleType)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _service.UpdateSampleTypeAsync(sampleType);
            TempData["Success"] = $"Sample type '{sampleType.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSampleTypeAsync(id);
            TempData["Success"] = "Sample type removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
