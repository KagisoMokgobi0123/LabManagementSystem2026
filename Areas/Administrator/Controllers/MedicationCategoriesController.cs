using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Administrator.Models.Enums;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class MedicationCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationCategoriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.MedicationCategories
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var category =
                await _context.MedicationCategories
                    .FirstOrDefaultAsync(x =>
                        x.MedicationCategoryId == id);

            if (category == null)
                return NotFound();

            return Json(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicationCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.MedicationCategories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medication category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }

            category.Status = Status.Active;
            _context.MedicationCategories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medication category added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicationCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            var existing = await _context.MedicationCategories.FirstOrDefaultAsync(x => x.MedicationCategoryId == category.MedicationCategoryId && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medication category could not be found or is inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.MedicationCategories.AnyAsync(x => x.MedicationCategoryId != category.MedicationCategoryId && x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medication category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }
            existing.Name = category.Name;
            existing.Description = category.Description;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medication category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.MedicationCategories.FirstOrDefaultAsync(x => x.MedicationCategoryId == id && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medication category could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.Medications.AnyAsync(x => x.MedicationCategoryId == id && x.Status == Status.Active))
            {
                TempData["ErrorMessage"] = "This category cannot be deactivated while active records are assigned to it. Reassign or deactivate those records first.";
                return RedirectToAction(nameof(Index));
            }
            existing.Status = Status.Inactive;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medication category removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
