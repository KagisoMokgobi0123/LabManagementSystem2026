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
    public class AllergyCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllergyCategoriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.AllergyCategories
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _context.AllergyCategories
                .FirstOrDefaultAsync(x =>
                    x.AllergyCategoryId == id);

            if (category == null)
                return NotFound();

            return Json(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AllergyCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.AllergyCategories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A allergy category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }

            category.Status = Status.Active;
            _context.AllergyCategories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Allergy category added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AllergyCategory category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;
            var existing = await _context.AllergyCategories.FirstOrDefaultAsync(x => x.AllergyCategoryId == category.AllergyCategoryId && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The allergy category could not be found or is inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.AllergyCategories.AnyAsync(x => x.AllergyCategoryId != category.AllergyCategoryId && x.Name.ToLower() == category.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A allergy category with this name already exists.";
                return RedirectToAction(nameof(Index));
            }
            existing.Name = category.Name;
            existing.Description = category.Description;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Allergy category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.AllergyCategories.FirstOrDefaultAsync(x => x.AllergyCategoryId == id && x.Status == Status.Active);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "The allergy category could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }
            if (await _context.Allergies.AnyAsync(x => x.AllergyCategoryId == id && x.Status == Status.Active))
            {
                TempData["ErrorMessage"] = "This category cannot be deactivated while active records are assigned to it. Reassign or deactivate those records first.";
                return RedirectToAction(nameof(Index));
            }
            existing.Status = Status.Inactive;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Allergy category removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
