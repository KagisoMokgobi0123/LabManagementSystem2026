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
        public async Task<IActionResult> Create(
            AllergyCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            category.Status = Status.Active;

            _context.AllergyCategories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy category added successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            AllergyCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing =
                await _context.AllergyCategories
                    .FirstOrDefaultAsync(x =>
                        x.AllergyCategoryId ==
                        category.AllergyCategoryId &&
                        x.Status == Status.Active);

            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy category updated successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category =
                await _context.AllergyCategories
                    .FirstOrDefaultAsync(x =>
                        x.AllergyCategoryId == id &&
                        x.Status == Status.Active);

            if (category == null)
                return NotFound();

            category.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Allergy category removed successfully."
            });
        }
    }
}
