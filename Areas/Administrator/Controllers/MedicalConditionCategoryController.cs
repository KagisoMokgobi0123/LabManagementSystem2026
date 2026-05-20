using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.ViewModels.Administrator;
using NMB_HLabSys.Data;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicalConditionCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalConditionCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.MedicalConditionCategories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create() => View(new CategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var exists = await _context.MedicalConditionCategories
                .AnyAsync(c => c.CategoryName == vm.CategoryName);
            if (exists)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists.");
                return View(vm);
            }

            _context.MedicalConditionCategories.Add(new MedicalConditionCategory
            {
                CategoryName = vm.CategoryName,
                Description = vm.Description
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _context.MedicalConditionCategories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(new CategoryViewModel
            {
                Id = cat.MedicalConditionCategoryID,
                CategoryName = cat.CategoryName,
                Description = cat.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var exists = await _context.MedicalConditionCategories
                .AnyAsync(c => c.CategoryName == vm.CategoryName
                            && c.MedicalConditionCategoryID != vm.Id);
            if (exists)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists.");
                return View(vm);
            }

            var cat = await _context.MedicalConditionCategories.FindAsync(vm.Id);
            if (cat == null) return NotFound();

            cat.CategoryName = vm.CategoryName;
            cat.Description = vm.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _context.MedicalConditionCategories
                .Include(c => c.MedicalConditions)
                .FirstOrDefaultAsync(c => c.MedicalConditionCategoryID == id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await _context.MedicalConditionCategories
                .Include(c => c.MedicalConditions)
                .FirstOrDefaultAsync(c => c.MedicalConditionCategoryID == id);

            if (cat.MedicalConditions.Any())
            {
                ModelState.AddModelError("",
                    "Cannot delete category with existing conditions.");
                return View(cat);
            }

            _context.MedicalConditionCategories.Remove(cat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
