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
    public class MedicalConditionCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalConditionCategoriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Administrator/MedicalConditionCategories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.MedicalConditionCategories
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View("~/Areas/Administrator/Views/ConditionCategories/Index.cshtml", categories);
        }

        // GET: Administrator/MedicalConditionCategories/Get/5
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var category =
                await _context.MedicalConditionCategories
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionCategoryId == id);

            if (category == null)
                return NotFound();

            return Json(category);
        }

        // POST: Administrator/MedicalConditionCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalConditionCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.MedicalConditionCategories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medical condition category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }

            category.Status = Status.Active;
            _context.MedicalConditionCategories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medical condition category added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalConditionCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            var existing = await _context.MedicalConditionCategories.FirstOrDefaultAsync(x => x.MedicalConditionCategoryId == category.MedicalConditionCategoryId && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medical condition category could not be found or is inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.MedicalConditionCategories.AnyAsync(x => x.MedicalConditionCategoryId != category.MedicalConditionCategoryId && x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medical condition category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }
            existing.Name = category.Name;
            existing.Description = category.Description;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medical condition category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.MedicalConditionCategories.FirstOrDefaultAsync(x => x.MedicalConditionCategoryId == id && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medical condition category could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.MedicalConditions.AnyAsync(x => x.MedicalConditionCategoryId == id && x.Status == Status.Active))
            {
                TempData["ErrorMessage"] = "This category cannot be deactivated while active records are assigned to it. Reassign or deactivate those records first.";
                return RedirectToAction(nameof(Index));
            }
            existing.Status = Status.Inactive;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medical condition category removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
