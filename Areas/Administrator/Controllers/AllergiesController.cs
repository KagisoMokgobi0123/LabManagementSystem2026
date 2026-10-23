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
    public class AllergiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllergiesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.AllergyCategories = await _context.AllergyCategories
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var allergies = await _context.Allergies
                .Include(x => x.Category)
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(allergies);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var allergy = await _context.Allergies
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.AllergyId == id);

            if (allergy == null)
                return NotFound();

            return Json(allergy);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories =
                await _context.AllergyCategories
                    .Where(x => x.Status == Status.Active)
                    .OrderBy(x => x.Name)
                    .Select(x => new
                    {
                        x.AllergyCategoryId,
                        x.Name
                    })
                    .ToListAsync();

            return Json(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Allergy allergy)
        {
            allergy.Name = allergy.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the allergy. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.AllergyCategories
                    .AnyAsync(x =>
                        x.AllergyCategoryId ==
                        allergy.AllergyCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Allergies.AnyAsync(x => x.Status == Status.Active && x.AllergyCategoryId == allergy.AllergyCategoryId && x.Name.ToLower() == allergy.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "An allergy with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            allergy.Status = Status.Active;

            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Allergy allergy)
        {
            allergy.Name = allergy.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the allergy. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var existing =
                await _context.Allergies
                    .FirstOrDefaultAsync(x =>
                        x.AllergyId == allergy.AllergyId &&
                        x.Status == Status.Active);

            if (existing == null)
            {
                TempData["ErrorMessage"] = "The allergy could not be found or is no longer active.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.AllergyCategories
                    .AnyAsync(x =>
                        x.AllergyCategoryId ==
                        allergy.AllergyCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Allergies.AnyAsync(x => x.Status == Status.Active && x.AllergyId != allergy.AllergyId && x.AllergyCategoryId == allergy.AllergyCategoryId && x.Name.ToLower() == allergy.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "An allergy with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            existing.Name = allergy.Name;
            existing.Description = allergy.Description;
            existing.AllergyCategoryId =
                allergy.AllergyCategoryId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var allergy = await _context.Allergies
                .FirstOrDefaultAsync(x =>
                    x.AllergyId == id &&
                    x.Status == Status.Active);

            if (allergy == null)
            {
                TempData["ErrorMessage"] = "The allergy could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }

            allergy.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
