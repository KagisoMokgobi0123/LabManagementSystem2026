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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists =
                await _context.MedicationCategories
                    .AnyAsync(x =>
                        x.MedicationCategoryId ==
                        medication.MedicationCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            medication.Status = Status.Active;

            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medication added successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Medication medication)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing =
                await _context.Medications
                    .FirstOrDefaultAsync(x =>
                        x.MedicationId ==
                        medication.MedicationId &&
                        x.Status == Status.Active);

            if (existing == null)
                return NotFound();

            var categoryExists =
                await _context.MedicationCategories
                    .AnyAsync(x =>
                        x.MedicationCategoryId ==
                        medication.MedicationCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            existing.Name = medication.Name;
            existing.Description = medication.Description;
            existing.MedicationCategoryId =
                medication.MedicationCategoryId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medication updated successfully."
            });
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
                return NotFound();

            medication.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medication removed successfully."
            });
        }
    }
}
