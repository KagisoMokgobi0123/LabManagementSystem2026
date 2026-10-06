using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class TestTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestTypeController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var testTypes =
                await GetActiveTestTypes();

            await LoadDropdowns();

            return View(testTypes);
        }


        // ============================================================
        // DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var testType =
                await _context.TestTypes
                    .AsNoTracking()

                    .Include(t => t.TestCategory)

                    .Include(t => t.SampleType)

                    .Include(t => t.MeasurementUnitDetails)

                    .Include(t => t.TechnicianTestTypes)

                    .Include(t => t.TestTypeConsumables)

                    .FirstOrDefaultAsync(t =>
                        t.TestTypeId == id);


            if (testType == null)
                return NotFound();


            return Json(new
            {
                success = true,

                testType = new
                {
                    testType.TestTypeId,

                    testType.Name,

                    TestCategory =
                        testType.TestCategory?.Description,

                    SampleType =
                        testType.SampleType?.Name,

                    MeasurementUnit =
                        testType.MeasurementUnitDetails?.Unit,

                    testType.NormalMin,

                    testType.NormalMax,

                    testType.TurnAroundTimeHours,

                    Status =
                        testType.IsActive.ToString(),

                    TechnicianCount =
                        testType.TechnicianTestTypes?.Count ?? 0,

                    ConsumableCount =
                        testType.TestTypeConsumables?.Count ?? 0
                }
            });
        }


        // ============================================================
        // CREATE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TestTypeModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;


            await ValidateTestType(model);


            // --------------------------------------------------------
            // DUPLICATE TEST TYPE CHECK
            // --------------------------------------------------------

            bool duplicateTestType =
                await _context.TestTypes
                    .AnyAsync(t =>
                        t.Name.ToLower() ==
                        model.Name.ToLower());

            if (duplicateTestType)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A test type with this name already exists.");
            }


            // --------------------------------------------------------
            // VALIDATION
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);

                var testTypes =
                    await GetActiveTestTypes();

                return View("Index", testTypes);
            }


            // --------------------------------------------------------
            // NEW ENTITY
            // --------------------------------------------------------

            model.TestTypeId = 0;

            model.IsActive =
                TestTypeModel.RecordStatus.Active;


            _context.TestTypes.Add(model);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test type '{model.Name}' added successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // EDIT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            TestTypeModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;


            // --------------------------------------------------------
            // FIND EXISTING TEST TYPE
            // --------------------------------------------------------

            var existing =
                await _context.TestTypes
                    .FirstOrDefaultAsync(t =>
                        t.TestTypeId ==
                        model.TestTypeId);


            if (existing == null)
                return NotFound();


            await ValidateTestType(model);


            // --------------------------------------------------------
            // DUPLICATE TEST TYPE CHECK
            // --------------------------------------------------------

            bool duplicateTestType =
                await _context.TestTypes
                    .AnyAsync(t =>
                        t.TestTypeId !=
                        model.TestTypeId &&

                        t.Name.ToLower() ==
                        model.Name.ToLower());


            if (duplicateTestType)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A test type with this name already exists.");
            }


            // --------------------------------------------------------
            // VALIDATION
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);

                var testTypes =
                    await GetActiveTestTypes();

                return View("Index", testTypes);
            }


            // --------------------------------------------------------
            // UPDATE EDITABLE FIELDS
            // --------------------------------------------------------

            existing.Name =
                model.Name;

            existing.TestCategoryId =
                model.TestCategoryId;

            existing.SampleTypeId =
                model.SampleTypeId;

            existing.MeasurementUnit =
                model.MeasurementUnit;

            existing.NormalMin =
                model.NormalMin;

            existing.NormalMax =
                model.NormalMax;

            existing.TurnAroundTimeHours =
                model.TurnAroundTimeHours;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test type '{existing.Name}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // DELETE / SOFT DELETE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var testType =
                await _context.TestTypes
                    .FirstOrDefaultAsync(t =>
                        t.TestTypeId == id);


            if (testType == null)
            {
                TempData["ErrorMessage"] =
                    "Test type could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // TECHNICIAN ASSIGNMENT CHECK
            // --------------------------------------------------------

            bool hasTechnicians =
                await _context.TechnicianTestTypes
                    .AnyAsync(t =>
                        t.TestTypeId == id);


            if (hasTechnicians)
            {
                TempData["ErrorMessage"] =
                    "This test type cannot be deactivated because it is assigned to one or more technicians.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // SOFT DELETE
            // --------------------------------------------------------

            testType.IsActive =
                TestTypeModel.RecordStatus.Inactive;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Test type '{testType.Name}' deactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // VALIDATION
        // ============================================================

        private async Task ValidateTestType(
            TestTypeModel model)
        {
            // --------------------------------------------------------
            // TEST NAME
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Test name is required.");
            }


            if (model.Name.Length > 150)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Test name cannot exceed 150 characters.");
            }


            // --------------------------------------------------------
            // TEST CATEGORY
            // --------------------------------------------------------

            if (model.TestCategoryId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.TestCategoryId),
                    "Test category is required.");
            }
            else
            {
                bool categoryExists =
                    await _context.TestCategories
                        .AnyAsync(c =>
                            c.TestCategoryId ==
                            model.TestCategoryId &&

                            c.IsActive ==
                            TestCategoryModel.RecordStatus.Active);


                if (!categoryExists)
                {
                    ModelState.AddModelError(
                        nameof(model.TestCategoryId),
                        "The selected test category is invalid.");
                }
            }


            // --------------------------------------------------------
            // SAMPLE TYPE
            // --------------------------------------------------------

            if (model.SampleTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.SampleTypeId),
                    "Required sample type is required.");
            }
            else
            {
                bool sampleTypeExists =
                    await _context.SampleTypes
                        .AnyAsync(s =>
                            s.SampleTypeId ==
                            model.SampleTypeId &&

                            s.IsActive);


                if (!sampleTypeExists)
                {
                    ModelState.AddModelError(
                        nameof(model.SampleTypeId),
                        "The selected sample type is invalid.");
                }
            }


            // --------------------------------------------------------
            // MEASUREMENT UNIT
            // --------------------------------------------------------

            if (model.MeasurementUnit <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.MeasurementUnit),
                    "Measurement unit is required.");
            }
            else
            {
                bool measurementUnitExists =
                    await _context.MeasurementUnits
                        .AnyAsync(m =>
                            m.MeasurementUnitId ==
                            model.MeasurementUnit);


                if (!measurementUnitExists)
                {
                    ModelState.AddModelError(
                        nameof(model.MeasurementUnit),
                        "The selected measurement unit is invalid.");
                }
            }


            // --------------------------------------------------------
            // NORMAL RANGE
            // --------------------------------------------------------

            if (model.NormalMin >
                model.NormalMax)
            {
                ModelState.AddModelError(
                    nameof(model.NormalMin),
                    "Normal minimum cannot be greater than normal maximum.");
            }


            // --------------------------------------------------------
            // TURNAROUND TIME
            //
            // Stored in minutes even though the property name
            // remains TurnAroundTimeHours.
            // --------------------------------------------------------

            if (model.TurnAroundTimeHours < 1)
            {
                ModelState.AddModelError(
                    nameof(model.TurnAroundTimeHours),
                    "Turnaround time must be at least 1 minute.");
            }


            if (model.TurnAroundTimeHours > 10000)
            {
                ModelState.AddModelError(
                    nameof(model.TurnAroundTimeHours),
                    "Turnaround time cannot exceed 10,000 minutes.");
            }
        }


        // ============================================================
        // LOAD ACTIVE TEST TYPES
        // ============================================================

        private async Task<List<TestTypeModel>>
            GetActiveTestTypes()
        {
            return await _context.TestTypes

                .AsNoTracking()

                .Include(t => t.TestCategory)

                .Include(t => t.SampleType)

                .Include(t => t.MeasurementUnitDetails)

                .Include(t => t.TechnicianTestTypes)

                .Include(t => t.TestTypeConsumables)

                .Where(t =>
                    t.IsActive ==
                    TestTypeModel.RecordStatus.Active)

                .OrderBy(t => t.Name)

                .ToListAsync();
        }


        // ============================================================
        // LOAD DROPDOWN DATA
        // ============================================================
        //
        // IMPORTANT:
        //
        // TestCategoryModel uses RecordStatus.
        // SampleTypeModel uses bool IsActive.
        // MeasurementUnitModel DOES NOT have IsActive.
        //
        // Therefore the three queries intentionally use
        // different status conditions.
        // ============================================================

        private async Task LoadDropdowns(
            TestTypeModel? model = null)
        {
            // --------------------------------------------------------
            // TEST CATEGORIES
            // --------------------------------------------------------

            var testCategories =
                await _context.TestCategories

                    .AsNoTracking()

                    .Where(c =>
                        c.IsActive ==
                        TestCategoryModel.RecordStatus.Active)

                    .OrderBy(c => c.Description)

                    .ToListAsync();


            // --------------------------------------------------------
            // SAMPLE TYPES
            // --------------------------------------------------------

            var sampleTypes =
                await _context.SampleTypes

                    .AsNoTracking()

                    .Where(s =>
                        s.IsActive)

                    .OrderBy(s => s.Name)

                    .ToListAsync();


            // --------------------------------------------------------
            // MEASUREMENT UNITS
            // --------------------------------------------------------
            //
            // MeasurementUnitModel has no IsActive property.
            // Therefore all measurement units are loaded.
            // --------------------------------------------------------

            var measurementUnits =
                await _context.MeasurementUnits

                    .AsNoTracking()

                    .OrderBy(m => m.Unit)

                    .ToListAsync();


            // --------------------------------------------------------
            // SEND DATA TO VIEW
            // --------------------------------------------------------

            ViewBag.TestCategories =
                testCategories;

            ViewBag.SampleTypes =
                sampleTypes;

            ViewBag.MeasurementUnits =
                measurementUnits;
        }
    }
}
