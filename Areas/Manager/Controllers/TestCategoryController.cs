using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class TestCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await GetActiveCategories();

            return View(categories);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.TestCategories
                .Include(c => c.TestTypes)
                .FirstOrDefaultAsync(c =>
                    c.TestCategoryId == id);

            if (category == null)
                return NotFound();

            return Json(new
            {
                success = true,
                category = new
                {
                    category.TestCategoryId,
                    category.Description,

                    TestTypeCount =
                        category.TestTypes.Count
                }
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TestCategoryModel model)
        {
            model.Description =
                model.Description?.Trim() ?? string.Empty;


            await ValidateCategory(model);


            // DUPLICATE CATEGORY CHECK

            bool duplicateCategory =
                await _context.TestCategories
                    .AnyAsync(c =>
                        c.Description.ToLower() ==
                        model.Description.ToLower());

            if (duplicateCategory)
            {
                ModelState.AddModelError(
                    nameof(model.Description),
                    "A test category with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                var categories =
                    await GetActiveCategories();

                return View("Index", categories);
            }



            model.TestCategoryId = 0;

            // New categories are active by default.
            model.IsActive =
                TestCategoryModel.RecordStatus.Active;

            _context.TestCategories.Add(model);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test category '{model.Description}' added successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            TestCategoryModel model)
        {
            model.Description =
                model.Description?.Trim() ?? string.Empty;

            // FIND EXISTING CATEGORY

            var existing =
                await _context.TestCategories
                    .FirstOrDefaultAsync(c =>
                        c.TestCategoryId ==
                        model.TestCategoryId);

            if (existing == null)
                return NotFound();


            await ValidateCategory(model);

            // DUPLICATE CATEGORY CHECK

            bool duplicateCategory =
                await _context.TestCategories
                    .AnyAsync(c =>
                        c.TestCategoryId !=
                        model.TestCategoryId &&
                        c.Description.ToLower() ==
                        model.Description.ToLower());


            if (duplicateCategory)
            {
                ModelState.AddModelError(
                    nameof(model.Description),
                    "A test category with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                var categories =
                    await GetActiveCategories();

                return View("Index", categories);
            }

            existing.Description =
                model.Description;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test category '{existing.Description}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }

        //soft delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var category =
                await _context.TestCategories
                    .Include(c => c.TestTypes)
                    .FirstOrDefaultAsync(c =>
                        c.TestCategoryId == id);


            if (category == null)
            {
                TempData["ErrorMessage"] =
                    "Test category could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }



            bool hasTestTypes =
                await _context.TestTypes
                    .AnyAsync(t =>
                        t.TestCategoryId == id);


            if (hasTestTypes)
            {
                TempData["ErrorMessage"] =
                    "This test category cannot be deleted because it is linked to one or more test types.";

                return RedirectToAction(
                    nameof(Index));
            }



            category.IsActive =
                TestCategoryModel.RecordStatus.Inactive;

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test category '{category.Description}' deleted successfully.";


            return RedirectToAction(
                nameof(Index));
        }



        private async Task ValidateCategory(
            TestCategoryModel model)
        {


            if (string.IsNullOrWhiteSpace(
                model.Description))
            {
                ModelState.AddModelError(
                    nameof(model.Description),
                    "Test category name is required.");
            }


            // Max length
            if (model.Description.Length > 100)
            {
                ModelState.AddModelError(
                    nameof(model.Description),
                    "Test category name cannot exceed 100 characters.");
            }
        }

        private async Task<List<TestCategoryModel>> GetActiveCategories()
        {
            return await _context.TestCategories
                .Where(c =>
                    c.IsActive == TestCategoryModel.RecordStatus.Active)
                .OrderBy(c => c.Description)
                .Select(c => new TestCategoryModel
                {
                    TestCategoryId = c.TestCategoryId,
                    Description = c.Description,
                    IsActive = c.IsActive,

                    TestTypes = c.TestTypes
                        .Where(t =>
                            t.IsActive == TestTypeModel.RecordStatus.Active)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
