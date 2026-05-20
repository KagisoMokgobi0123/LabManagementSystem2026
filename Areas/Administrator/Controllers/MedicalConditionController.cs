using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.ViewModels.Administrator;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicalConditionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalConditionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var conditions = await _context.MedicalConditions
                .Include(c => c.MedicalConditionCategory)
                .ToListAsync();
            return View(conditions);
        }

        private async Task PopulateCategories(MedicalConditionViewModel vm)
        {
            vm.Categories = await _context.MedicalConditionCategories
                .Select(c => new SelectListItem
                {
                    Value = c.MedicalConditionCategoryID.ToString(),
                    Text = c.CategoryName
                }).ToListAsync();
        }

        public async Task<IActionResult> Create()
        {
            var vm = new MedicalConditionViewModel();
            await PopulateCategories(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalConditionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(vm);
                return View(vm);
            }

            _context.MedicalConditions.Add(new MedicalCondition
            {
                ConditionName = vm.ConditionName,
                Description = vm.Description,
                MedicalConditionCategoryId = vm.MedicalConditionCategoryId
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var condition = await _context.MedicalConditions.FindAsync(id);
            if (condition == null) return NotFound();

            var vm = new MedicalConditionViewModel
            {
                MedicalConditionId = condition.MedicalConditionId,
                ConditionName = condition.ConditionName,
                Description = condition.Description,
                MedicalConditionCategoryId = condition.MedicalConditionCategoryId
            };
            await PopulateCategories(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalConditionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(vm);
                return View(vm);
            }

            var condition = await _context.MedicalConditions.FindAsync(vm.MedicalConditionId);
            if (condition == null) return NotFound();

            condition.ConditionName = vm.ConditionName;
            condition.Description = vm.Description;
            condition.MedicalConditionCategoryId = vm.MedicalConditionCategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var condition = await _context.MedicalConditions
                .Include(c => c.MedicalConditionCategory)
                .FirstOrDefaultAsync(c => c.MedicalConditionId == id);
            if (condition == null) return NotFound();
            return View(condition);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condition = await _context.MedicalConditions.FindAsync(id);
            _context.MedicalConditions.Remove(condition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
