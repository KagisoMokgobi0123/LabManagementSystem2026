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
            var conditions = await _context.MedicalConditions
                .Include(x => x.Category)
                .Where(x => x.Status == Status.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(conditions);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists =
                await _context.MedicalConditionCategories
                    .AnyAsync(x =>
                        x.MedicalConditionCategoryId ==
                        condition.MedicalConditionCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            condition.Status = Status.Active;

            _context.MedicalConditions.Add(condition);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition added successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            MedicalCondition condition)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing =
                await _context.MedicalConditions
                    .FirstOrDefaultAsync(x =>
                        x.MedicalConditionId ==
                        condition.MedicalConditionId &&
                        x.Status == Status.Active);

            if (existing == null)
                return NotFound();

            var categoryExists =
                await _context.MedicalConditionCategories
                    .AnyAsync(x =>
                        x.MedicalConditionCategoryId ==
                        condition.MedicalConditionCategoryId &&
                        x.Status == Status.Active);

            if (!categoryExists)
                return BadRequest("Invalid category.");

            existing.Name = condition.Name;
            existing.Description = condition.Description;
            existing.MedicalConditionCategoryId =
                condition.MedicalConditionCategoryId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition updated successfully."
            });
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
                return NotFound();

            condition.Status = Status.Inactive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Medical condition removed successfully."
            });
        }
    }
}
