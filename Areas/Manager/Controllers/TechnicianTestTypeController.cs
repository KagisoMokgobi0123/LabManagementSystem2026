using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class TechnicianTestTypeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TechnicianTestTypeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var assignments =
                await GetActiveAssignments();

            // Load dropdown data.
            await LoadDropdownData();

            return View(assignments);
        }


        // ============================================================
        // DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var assignment =
                await _context.TechnicianTestTypes
                    .Include(a => a.Technician)
                    .Include(a => a.TestType)
                    .Include(a => a.Manager)
                    .FirstOrDefaultAsync(a =>
                        a.TechnicianTestTypeId == id);

            if (assignment == null)
                return NotFound();


            return Json(new
            {
                success = true,

                assignment = new
                {
                    assignment.TechnicianTestTypeId,

                    assignment.TechnicianId,

                    Technician =
                        assignment.Technician?.UserName,

                    assignment.TestTypeId,

                    TestType =
                        assignment.TestType?.Name,

                    assignment.ManagerId,

                    Manager =
                        assignment.Manager?.UserName,

                    assignment.AssignedDate,

                    assignment.IsActive
                }
            });
        }


        // ============================================================
        // CREATE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TechnicianTestTypeModel model)
        {
            model.TechnicianId =
                model.TechnicianId?.Trim() ?? string.Empty;


            // --------------------------------------------------------
            // CURRENT MANAGER
            // --------------------------------------------------------

            var currentManager =
                await _userManager.GetUserAsync(User);

            if (currentManager == null)
            {
                TempData["ErrorMessage"] =
                    "The current manager could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }


            // The authenticated Manager is always recorded
            // as the Manager responsible for the assignment.

            model.ManagerId =
                currentManager.Id;


            await ValidateAssignment(model);


            // --------------------------------------------------------
            // DUPLICATE ASSIGNMENT CHECK
            // --------------------------------------------------------

            bool duplicateAssignment =
                await _context.TechnicianTestTypes
                    .AnyAsync(a =>
                        a.TechnicianId ==
                        model.TechnicianId &&

                        a.TestTypeId ==
                        model.TestTypeId &&

                        a.IsActive);


            if (duplicateAssignment)
            {
                ModelState.AddModelError(
                    nameof(model.TestTypeId),
                    "This test type is already assigned to this technician.");
            }


            // --------------------------------------------------------
            // VALIDATION FAILURE
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                var assignments =
                    await GetActiveAssignments();

                // IMPORTANT:
                // Reload dropdown data when returning the view
                // because the request is not going through Index().

                await LoadDropdownData();

                return View("Index", assignments);
            }


            // --------------------------------------------------------
            // NEW ENTITY
            // --------------------------------------------------------

            model.TechnicianTestTypeId = 0;

            model.AssignedDate =
                DateTime.Now;

            model.IsActive = true;


            _context.TechnicianTestTypes.Add(model);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Test type assigned to technician successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // EDIT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            TechnicianTestTypeModel model)
        {
            model.TechnicianId =
                model.TechnicianId?.Trim() ?? string.Empty;


            // --------------------------------------------------------
            // FIND EXISTING ASSIGNMENT
            // --------------------------------------------------------

            var existing =
                await _context.TechnicianTestTypes
                    .FirstOrDefaultAsync(a =>
                        a.TechnicianTestTypeId ==
                        model.TechnicianTestTypeId);

            if (existing == null)
                return NotFound();


            await ValidateAssignment(model);


            // --------------------------------------------------------
            // DUPLICATE ASSIGNMENT CHECK
            // --------------------------------------------------------

            bool duplicateAssignment =
                await _context.TechnicianTestTypes
                    .AnyAsync(a =>
                        a.TechnicianTestTypeId !=
                        model.TechnicianTestTypeId &&

                        a.TechnicianId ==
                        model.TechnicianId &&

                        a.TestTypeId ==
                        model.TestTypeId &&

                        a.IsActive);


            if (duplicateAssignment)
            {
                ModelState.AddModelError(
                    nameof(model.TestTypeId),
                    "This test type is already assigned to this technician.");
            }


            // --------------------------------------------------------
            // VALIDATION FAILURE
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                var assignments =
                    await GetActiveAssignments();

                // IMPORTANT:
                // Reload dropdown data when returning the view.

                await LoadDropdownData();

                return View("Index", assignments);
            }


            // --------------------------------------------------------
            // UPDATE EDITABLE FIELDS
            // --------------------------------------------------------

            existing.TechnicianId =
                model.TechnicianId;

            existing.TestTypeId =
                model.TestTypeId;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Technician test type assignment updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // DELETE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var assignment =
                await _context.TechnicianTestTypes
                    .Include(a => a.Technician)
                    .Include(a => a.TestType)
                    .FirstOrDefaultAsync(a =>
                        a.TechnicianTestTypeId == id);


            if (assignment == null)
            {
                TempData["ErrorMessage"] =
                    "Technician test type assignment could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // SOFT DELETE
            // --------------------------------------------------------

            assignment.IsActive = false;

            await _context.SaveChangesAsync();


            var technicianName =
                assignment.Technician?.UserName
                ?? "technician";

            var testTypeName =
                assignment.TestType?.Name
                ?? "test type";


            TempData["SuccessMessage"] =
                $"The '{testTypeName}' assignment for '{technicianName}' was deactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // VALIDATION
        // ============================================================

        private async Task ValidateAssignment(
            TechnicianTestTypeModel model)
        {
            // --------------------------------------------------------
            // TECHNICIAN REQUIRED
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                model.TechnicianId))
            {
                ModelState.AddModelError(
                    nameof(model.TechnicianId),
                    "Technician is required.");
            }


            // --------------------------------------------------------
            // TECHNICIAN EXISTS
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                model.TechnicianId))
            {
                var technician =
                    await _userManager.FindByIdAsync(
                        model.TechnicianId);

                if (technician == null)
                {
                    ModelState.AddModelError(
                        nameof(model.TechnicianId),
                        "The selected technician could not be found.");
                }
                else
                {
                    var isTechnician =
                        await _userManager.IsInRoleAsync(
                            technician,
                            "Technician");

                    if (!isTechnician)
                    {
                        ModelState.AddModelError(
                            nameof(model.TechnicianId),
                            "The selected user is not a technician.");
                    }

                    // ------------------------------------------------
                    // TECHNICIAN ACTIVE CHECK
                    // ------------------------------------------------

                    if (!technician.IsActive)
                    {
                        ModelState.AddModelError(
                            nameof(model.TechnicianId),
                            "The selected technician account is inactive.");
                    }
                }
            }


            // --------------------------------------------------------
            // TEST TYPE REQUIRED
            // --------------------------------------------------------

            if (model.TestTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.TestTypeId),
                    "Test type is required.");
            }


            // --------------------------------------------------------
            // TEST TYPE EXISTS
            // --------------------------------------------------------

            if (model.TestTypeId > 0)
            {
                var testType =
                    await _context.TestTypes
                        .FirstOrDefaultAsync(t =>
                            t.TestTypeId ==
                            model.TestTypeId);

                if (testType == null)
                {
                    ModelState.AddModelError(
                        nameof(model.TestTypeId),
                        "The selected test type could not be found.");
                }
                else if (
                    testType.IsActive !=
                    TestTypeModel.RecordStatus.Active)
                {
                    ModelState.AddModelError(
                        nameof(model.TestTypeId),
                        "The selected test type is inactive.");
                }
            }
        }


        // ============================================================
        // LOAD DROPDOWN DATA
        // ============================================================

        private async Task LoadDropdownData()
        {
            // --------------------------------------------------------
            // LOAD ACTIVE TECHNICIANS
            // --------------------------------------------------------

            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    "Technician");


            ViewBag.Technicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            // --------------------------------------------------------
            // LOAD ACTIVE TEST TYPES
            // --------------------------------------------------------

            ViewBag.TestTypes =
                await _context.TestTypes
                    .Where(t =>
                        t.IsActive ==
                        TestTypeModel.RecordStatus.Active)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
        }


        // ============================================================
        // LOAD ACTIVE ASSIGNMENTS
        // ============================================================

        private async Task<List<TechnicianTestTypeModel>>
            GetActiveAssignments()
        {
            return await _context.TechnicianTestTypes
                .Include(a => a.Technician)
                .Include(a => a.TestType)
                .Include(a => a.Manager)
                .Where(a =>
                    a.IsActive)
                .OrderBy(a =>
                    a.Technician!.UserName)
                .ThenBy(a =>
                    a.TestType!.Name)
                .ToListAsync();
        }
    }
}
