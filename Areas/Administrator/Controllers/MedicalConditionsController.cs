using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Administrator.Models.Enums;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class MedicalConditionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalConditionsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ConditionCategories = await _context.MedicalConditionCategories
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var conditions = await _context.MedicalConditions
                .Include(x => x.Category)
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View("~/Areas/Administrator/Views/Conditions/Index.cshtml", conditions);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var condition = await _context.MedicalConditions
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.MedicalConditionId == id);

            if (condition == null)
                return NotFound();

            return Json(condition);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories =
                await _context.MedicalConditionCategories
                    .Where(x => x.Status == Status.Active)
                    .OrderBy(x => x.Name)
                    .Select(x => new
                    {
                        x.MedicalConditionCategoryId,
                        x.Name
                    })
                    .ToListAsync();

            return Json(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            MedicalCondition condition)
        {
            condition.Name = condition.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the medical condition. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.MedicalConditionCategories
                    .AnyAsync(x =>
                        x.MedicalConditionCategoryId ==
                        condition.MedicalConditionCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.MedicalConditions.AnyAsync(x => x.Status == Status.Active && x.MedicalConditionCategoryId == condition.MedicalConditionCategoryId && x.Name.ToLower() == condition.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medical condition with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            condition.Status = Status.Active;

            _context.MedicalConditions.Add(condition);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical condition added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            MedicalCondition condition)
        {
            condition.Name = condition.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the medical condition. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var existing =
                await _context.MedicalConditions
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionId ==
                        condition.MedicalConditionId &&
                        x.Status == Status.Active);

            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medical condition could not be found or is no longer active.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.MedicalConditionCategories
                    .AnyAsync(x =>
                        x.MedicalConditionCategoryId ==
                        condition.MedicalConditionCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.MedicalConditions.AnyAsync(x => x.Status == Status.Active && x.MedicalConditionId != condition.MedicalConditionId && x.MedicalConditionCategoryId == condition.MedicalConditionCategoryId && x.Name.ToLower() == condition.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medical condition with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            existing.Name = condition.Name;
            existing.Description = condition.Description;
            existing.MedicalConditionCategoryId =
                condition.MedicalConditionCategoryId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical condition updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var condition =
                await _context.MedicalConditions
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionId == id &&
                        x.Status == Status.Active);

            if (condition == null)
            {
                TempData["ErrorMessage"] = "The medical condition could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }

            condition.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical condition removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
