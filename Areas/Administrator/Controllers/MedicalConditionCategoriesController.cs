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

            return View(categories);
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
        public async Task<IActionResult> Create(
            MedicalConditionCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            category.Status = Status.Active;

            _context.MedicalConditionCategories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition category added successfully."
            });
        }

        // POST: Administrator/MedicalConditionCategories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            MedicalConditionCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing =
                await _context.MedicalConditionCategories
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionCategoryId ==
                        category.MedicalConditionCategoryId &&
                        x.Status == Status.Active);

            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition category updated successfully."
            });
        }

        // POST: Administrator/MedicalConditionCategories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category =
                await _context.MedicalConditionCategories
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionCategoryId == id &&
                        x.Status == Status.Active);

            if (category == null)
                return NotFound();

            category.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition category removed successfully."
            });
        }
    }
}