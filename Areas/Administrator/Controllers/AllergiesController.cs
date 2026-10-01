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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists =
                await _context.AllergyCategories
                    .AnyAsync(x =>
                        x.AllergyCategoryId ==
                        allergy.AllergyCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            allergy.Status = Status.Active;

            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy added successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Allergy allergy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing =
                await _context.Allergies
                    .FirstOrDefaultAsync(x =>
                        x.AllergyId == allergy.AllergyId &&
                        x.Status == Status.Active);

            if (existing == null)
                return NotFound();

            var categoryExists =
                await _context.AllergyCategories
                    .AnyAsync(x =>
                        x.AllergyCategoryId ==
                        allergy.AllergyCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            existing.Name = allergy.Name;
            existing.Description = allergy.Description;
            existing.AllergyCategoryId =
                allergy.AllergyCategoryId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy updated successfully."
            });
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
                return NotFound();

            allergy.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy removed successfully."
            });
        }
    }
}
