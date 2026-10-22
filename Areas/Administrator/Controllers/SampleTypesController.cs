using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class SampleTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SampleTypesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index() => View(await GetActiveSampleTypes());

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sampleType = await _context.SampleTypes.Include(x => x.Samples).FirstOrDefaultAsync(x => x.SampleTypeId == id);
            if (sampleType == null) return NotFound();
            return Json(new { success = true, sampleType = new { sampleType.SampleTypeId, sampleType.Name, SampleCount = sampleType.Samples.Count, sampleType.IsActive } });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SampleTypeModel model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;
            Validate(model);
            if (await _context.SampleTypes.AnyAsync(x => x.Name.ToLower() == model.Name.ToLower()))
                ModelState.AddModelError(nameof(model.Name), "A sample type with this name already exists.");
            if (!ModelState.IsValid) { TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the sample type details."; return RedirectToAction(nameof(Index)); }
            model.SampleTypeId = 0; model.IsActive = true;
            _context.SampleTypes.Add(model); await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Sample type '{model.Name}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SampleTypeModel model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;
            var existing = await _context.SampleTypes.FirstOrDefaultAsync(x => x.SampleTypeId == model.SampleTypeId && x.IsActive);
            if (existing == null) { TempData["ErrorMessage"] = "The sample type could not be found or is inactive."; return RedirectToAction(nameof(Index)); }
            Validate(model);
            if (await _context.SampleTypes.AnyAsync(x => x.SampleTypeId != model.SampleTypeId && x.Name.ToLower() == model.Name.ToLower()))
                ModelState.AddModelError(nameof(model.Name), "A sample type with this name already exists.");
            if (!ModelState.IsValid) { TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the sample type details."; return RedirectToAction(nameof(Index)); }
            existing.Name = model.Name; await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Sample type '{existing.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SampleTypes.FirstOrDefaultAsync(x => x.SampleTypeId == id && x.IsActive);
            if (item == null) { TempData["ErrorMessage"] = "The sample type could not be found or is already inactive."; return RedirectToAction(nameof(Index)); }
            if (await _context.Samples.AnyAsync(x => x.SampleTypeId == id)) { TempData["ErrorMessage"] = "This sample type cannot be deactivated because it is linked to one or more samples."; return RedirectToAction(nameof(Index)); }
            item.IsActive = false; await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Sample type '{item.Name}' deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void Validate(SampleTypeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) ModelState.AddModelError(nameof(model.Name), "Sample type name is required.");
            if (model.Name.Length > 100) ModelState.AddModelError(nameof(model.Name), "Sample type name cannot exceed 100 characters.");
        }

        private Task<List<SampleTypeModel>> GetActiveSampleTypes() => _context.SampleTypes.Include(x => x.Samples).Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();
    }
}
