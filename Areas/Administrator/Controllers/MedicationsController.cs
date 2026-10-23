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
    public class MedicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.MedicationCategories = await _context.MedicationCategories
                .Where(x => x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var medications = await _context.Medications
                .Include(x => x.Category)
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(medications);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var medication = await _context.Medications
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.MedicationId == id);

            if (medication == null)
                return NotFound();

            return Json(medication);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories =
                await _context.MedicationCategories
                    .Where(x => x.Status == Status.Active)
                    .OrderBy(x => x.Name)
                    .Select(x => new
                    {
                        x.MedicationCategoryId,
                        x.Name
                    })
                    .ToListAsync();

            return Json(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Medication medication)
        {
            medication.Name = medication.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the medication. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.MedicationCategories
                    .AnyAsync(x =>
                        x.MedicationCategoryId ==
                        medication.MedicationCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Medications.AnyAsync(x => x.Status == Status.Active && x.MedicationCategoryId == medication.MedicationCategoryId && x.Name.ToLower() == medication.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medication with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            medication.Status = Status.Active;

            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Medication medication)
        {
            medication.Name = medication.Name?.Trim() ?? string.Empty;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Unable to save the medication. Check the submitted details and try again.";
                return RedirectToAction(nameof(Index));
            }

            var existing =
                await _context.Medications
                    .FirstOrDefaultAsync(x =>
                        x.MedicationId ==
                        medication.MedicationId &&
                        x.Status == Status.Active);

            if (existing == null)
            {
                TempData["ErrorMessage"] = "The medication could not be found or is no longer active.";
                return RedirectToAction(nameof(Index));
            }

            var categoryExists =
                await _context.MedicationCategories
                    .AnyAsync(x =>
                        x.MedicationCategoryId ==
                        medication.MedicationCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Select a valid active category before saving.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Medications.AnyAsync(x => x.Status == Status.Active && x.MedicationId != medication.MedicationId && x.MedicationCategoryId == medication.MedicationCategoryId && x.Name.ToLower() == medication.Name.ToLower()))
            {
                TempData["ErrorMessage"] = "A medication with this name already exists in the selected category.";
                return RedirectToAction(nameof(Index));
            }

            existing.Name = medication.Name;
            existing.Description = medication.Description;
            existing.MedicationCategoryId =
                medication.MedicationCategoryId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var medication =
                await _context.Medications
                    .FirstOrDefaultAsync(x =>
                        x.MedicationId == id &&
                        x.Status == Status.Active);

            if (medication == null)
            {
                TempData["ErrorMessage"] = "The medication could not be found or is already inactive.";
                return RedirectToAction(nameof(Index));
            }

            medication.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
