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

        // IMPORTANT:
        // The seeded technician role is "LabTechnician".
        // Keep this in one place so the role name does not become
        // inconsistent throughout the controller.
        private const string TechnicianRole = "LabTechnician";

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
            // --------------------------------------------------------
            // LOAD ACTIVE ASSIGNMENTS
            // --------------------------------------------------------

            var assignments = await GetActiveAssignments();


            // --------------------------------------------------------
            // LOAD ACTIVE TECHNICIANS
            // --------------------------------------------------------

            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            var activeTechnicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            // --------------------------------------------------------
            // DETERMINE TECHNICIANS WITHOUT ASSIGNMENTS
            // --------------------------------------------------------

            var technicianAssignmentCounts =
                assignments
                    .GroupBy(a => a.TechnicianId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count());


            var techniciansWithoutAssignments =
                activeTechnicians
                    .Where(t =>
                        !technicianAssignmentCounts.ContainsKey(
                            t.Id))
                    .ToList();


            // --------------------------------------------------------
            // LOAD DROPDOWN DATA
            // --------------------------------------------------------

            await LoadDropdownData();


            // --------------------------------------------------------
            // VIEWBAG DATA
            // --------------------------------------------------------

            ViewBag.ActiveTechnicians =
                activeTechnicians;

            ViewBag.TechniciansWithoutAssignments =
                techniciansWithoutAssignments;

            ViewBag.TechnicianAssignmentCounts =
                technicianAssignmentCounts;


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
            // --------------------------------------------------------
            // CLEAN TECHNICIAN ID
            // --------------------------------------------------------

            model.TechnicianId =
                model.TechnicianId?.Trim()
                ?? string.Empty;


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


            // --------------------------------------------------------
            // SET MANAGER
            // --------------------------------------------------------

            model.ManagerId =
                currentManager.Id;


            // IMPORTANT:
            //
            // ManagerId is not submitted by the form because it must
            // always come from the authenticated Manager.
            //
            // If ManagerId has [Required] in TechnicianTestTypeModel,
            // ASP.NET Core may already have added a validation error
            // before this action executes.
            //
            // Remove that old validation result now that ManagerId
            // has been supplied by the server.

            ModelState.Remove(nameof(TechnicianTestTypeModel.ManagerId));


            // --------------------------------------------------------
            // SERVER-CONTROLLED FIELDS
            // --------------------------------------------------------

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.AssignedDate));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.IsActive));


            // --------------------------------------------------------
            // VALIDATION
            // --------------------------------------------------------

            await ValidateAssignment(model);


            // --------------------------------------------------------
            // DUPLICATE ASSIGNMENT CHECK
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(model.TechnicianId) &&
                model.TestTypeId > 0)
            {
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
            }


            // --------------------------------------------------------
            // VALIDATION FAILURE
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                // Make sure all dropdowns and technician data are
                // available when returning the Index view.

                var assignments =
                    await GetActiveAssignments();

                await LoadDropdownData();

                await LoadIndexTechnicianData();


                // ----------------------------------------------------
                // OPTIONAL: SHOW VALIDATION ERRORS IN SERVER LOG
                // ----------------------------------------------------

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        var errorMessage =
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? error.Exception?.Message
                                : error.ErrorMessage;

                        Console.WriteLine(
                            $"TechnicianTestType validation error - " +
                            $"{state.Key}: {errorMessage}");
                    }
                }


                return View("Index", assignments);
            }


            // --------------------------------------------------------
            // CREATE ASSIGNMENT
            // --------------------------------------------------------

            model.TechnicianTestTypeId = 0;

            model.AssignedDate =
                DateTime.Now;

            model.IsActive = true;


            _context.TechnicianTestTypes.Add(model);

            await _context.SaveChangesAsync();


            // --------------------------------------------------------
            // SUCCESS
            // --------------------------------------------------------

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
            // --------------------------------------------------------
            // CLEAN TECHNICIAN ID
            // --------------------------------------------------------

            model.TechnicianId =
                model.TechnicianId?.Trim()
                ?? string.Empty;


            // --------------------------------------------------------
            // FIND EXISTING ASSIGNMENT
            // --------------------------------------------------------

            var existing =
                await _context.TechnicianTestTypes
                    .FirstOrDefaultAsync(a =>
                        a.TechnicianTestTypeId ==
                        model.TechnicianTestTypeId);

            if (existing == null)
            {
                TempData["ErrorMessage"] =
                    "The technician test type assignment could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // CURRENT MANAGER
            // --------------------------------------------------------
            //
            // We do NOT allow the posted form to change the Manager.
            // The original manager remains associated with the
            // assignment.
            //
            // If your business rule is that editing should record the
            // current manager instead, change this accordingly.
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


            // --------------------------------------------------------
            // REMOVE SERVER-CONTROLLED MODEL VALIDATION
            // --------------------------------------------------------

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.ManagerId));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.AssignedDate));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.IsActive));


            // --------------------------------------------------------
            // VALIDATE TECHNICIAN AND TEST TYPE
            // --------------------------------------------------------

            await ValidateAssignment(model);


            // --------------------------------------------------------
            // DUPLICATE ASSIGNMENT CHECK
            // --------------------------------------------------------
            //
            // Ignore the assignment currently being edited.
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(model.TechnicianId) &&
                model.TestTypeId > 0)
            {
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
            }


            // --------------------------------------------------------
            // VALIDATION FAILURE
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                var assignments =
                    await GetActiveAssignments();

                await LoadDropdownData();

                await LoadIndexTechnicianData();


                // ----------------------------------------------------
                // WRITE VALIDATION ERRORS TO SERVER LOG
                // ----------------------------------------------------

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        var errorMessage =
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? error.Exception?.Message
                                : error.ErrorMessage;

                        Console.WriteLine(
                            $"TechnicianTestType Edit validation error - " +
                            $"{state.Key}: {errorMessage}");
                    }
                }


                return View("Index", assignments);
            }


            // --------------------------------------------------------
            // UPDATE ONLY EDITABLE FIELDS
            // --------------------------------------------------------

            existing.TechnicianId =
                model.TechnicianId;

            existing.TestTypeId =
                model.TestTypeId;


            // --------------------------------------------------------
            // SAVE
            // --------------------------------------------------------

            await _context.SaveChangesAsync();


            // --------------------------------------------------------
            // SUCCESS
            // --------------------------------------------------------

            TempData["SuccessMessage"] =
                "Technician test type assignment updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // DELETE / SOFT DELETE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // --------------------------------------------------------
            // FIND ASSIGNMENT
            // --------------------------------------------------------

            var assignment =
                await _context.TechnicianTestTypes
                    .Include(a => a.Technician)
                    .Include(a => a.TestType)
                    .FirstOrDefaultAsync(a =>
                        a.TechnicianTestTypeId == id);


            // --------------------------------------------------------
            // ASSIGNMENT NOT FOUND
            // --------------------------------------------------------

            if (assignment == null)
            {
                TempData["ErrorMessage"] =
                    "Technician test type assignment could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // ALREADY INACTIVE
            // --------------------------------------------------------

            if (!assignment.IsActive)
            {
                TempData["ErrorMessage"] =
                    "This technician test type assignment is already inactive.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // COUNT ACTIVE ASSIGNMENTS
            // --------------------------------------------------------

            var activeAssignmentCount =
                await _context.TechnicianTestTypes
                    .CountAsync(a =>
                        a.TechnicianId ==
                        assignment.TechnicianId &&

                        a.IsActive);


            // --------------------------------------------------------
            // PREVENT REMOVING LAST ASSIGNMENT
            // --------------------------------------------------------

            if (activeAssignmentCount <= 1)
            {
                var technicianName =
                    assignment.Technician?.FullName;

                if (string.IsNullOrWhiteSpace(technicianName))
                {
                    technicianName =
                        assignment.Technician?.UserName
                        ?? "This technician";
                }


                TempData["ErrorMessage"] =
                    $"{technicianName} must have at least one active " +
                    "test type assignment. Assign another test type " +
                    "before deactivating this assignment.";


                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // SOFT DELETE
            // --------------------------------------------------------

            assignment.IsActive = false;


            await _context.SaveChangesAsync();


            // --------------------------------------------------------
            // DISPLAY NAMES
            // --------------------------------------------------------

            var deletedTechnicianName =
                assignment.Technician?.FullName;

            if (string.IsNullOrWhiteSpace(deletedTechnicianName))
            {
                deletedTechnicianName =
                    assignment.Technician?.UserName
                    ?? "technician";
            }


            var testTypeName =
                assignment.TestType?.Name
                ?? "test type";


            // --------------------------------------------------------
            // SUCCESS
            // --------------------------------------------------------

            TempData["SuccessMessage"] =
                $"The '{testTypeName}' assignment for " +
                $"'{deletedTechnicianName}' was deactivated successfully.";


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
                    // IMPORTANT:
                    // Seeded role is LabTechnician, not Technician.
                    var isTechnician =
                        await _userManager.IsInRoleAsync(
                            technician,
                            TechnicianRole);

                    if (!isTechnician)
                    {
                        ModelState.AddModelError(
                            nameof(model.TechnicianId),
                            "The selected user is not a lab technician.");
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
            // ACTIVE LAB TECHNICIANS
            // --------------------------------------------------------

            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            ViewBag.Technicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            // --------------------------------------------------------
            // ACTIVE TEST TYPES
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
        // LOAD INDEX TECHNICIAN DATA
        // ============================================================

        private async Task LoadIndexTechnicianData()
        {
            // --------------------------------------------------------
            // LOAD ALL ACTIVE LAB TECHNICIANS
            // --------------------------------------------------------

            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            var activeTechnicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            // --------------------------------------------------------
            // LOAD ACTIVE ASSIGNMENT COUNTS
            // --------------------------------------------------------

            var technicianIds =
                activeTechnicians
                    .Select(t => t.Id)
                    .ToList();


            var technicianAssignmentCounts =
                await _context.TechnicianTestTypes
                    .Where(a =>
                        a.IsActive &&
                        technicianIds.Contains(
                            a.TechnicianId))
                    .GroupBy(a =>
                        a.TechnicianId)
                    .Select(g => new
                    {
                        TechnicianId =
                            g.Key,

                        Count =
                            g.Count()
                    })
                    .ToDictionaryAsync(
                        x => x.TechnicianId,
                        x => x.Count);


            // --------------------------------------------------------
            // TECHNICIANS WITHOUT ACTIVE ASSIGNMENTS
            // --------------------------------------------------------

            var techniciansWithoutAssignments =
                activeTechnicians
                    .Where(t =>
                        !technicianAssignmentCounts
                            .ContainsKey(t.Id))
                    .ToList();


            // --------------------------------------------------------
            // VIEW DATA
            // --------------------------------------------------------

            ViewBag.ActiveTechnicians =
                activeTechnicians;

            ViewBag.TechniciansWithoutAssignments =
                techniciansWithoutAssignments;

            ViewBag.TechnicianAssignmentCounts =
                technicianAssignmentCounts;
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