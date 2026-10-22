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

        //prevent role name to not become inconsistent throughout the controller from the seeded data.
        private const string TechnicianRole = "LabTechnician";

        public TechnicianTestTypeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var assignments = await GetActiveAssignments();

            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            var activeTechnicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            //Determine Technicians that need assigning of testype

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


            await LoadDropdownData();


            ViewBag.ActiveTechnicians =
                activeTechnicians;

            ViewBag.TechniciansWithoutAssignments =
                techniciansWithoutAssignments;

            ViewBag.TechnicianAssignmentCounts =
                technicianAssignmentCounts;


            return View(assignments);
        }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TechnicianTestTypeModel model)
        {
     

            model.TechnicianId =
                model.TechnicianId?.Trim()
                ?? string.Empty;


            var currentManager =
                await _userManager.GetUserAsync(User);

            if (currentManager == null)
            {
                TempData["ErrorMessage"] =
                    "The current manager could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }


            model.ManagerId =
                currentManager.Id;


            ModelState.Remove(nameof(TechnicianTestTypeModel.ManagerId));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.AssignedDate));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.IsActive));

            await ValidateAssignment(model);


            //duplicate assign check

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


            if (!ModelState.IsValid)
            {
                // Make sure all dropdowns and technician data are available when returning the Index view.

                var assignments =
                    await GetActiveAssignments();

                await LoadDropdownData();

                await LoadIndexTechnicianData();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            TechnicianTestTypeModel model)
        {


            model.TechnicianId =
                model.TechnicianId?.Trim()
                ?? string.Empty;

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


            var currentManager =
                await _userManager.GetUserAsync(User);

            if (currentManager == null)
            {
                TempData["ErrorMessage"] =
                    "The current manager could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }


            ModelState.Remove(
                nameof(TechnicianTestTypeModel.ManagerId));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.AssignedDate));

            ModelState.Remove(
                nameof(TechnicianTestTypeModel.IsActive));



            await ValidateAssignment(model);


            // DUPLICATE ASSIGNMENT CHECK
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

            if (!ModelState.IsValid)
            {
                var assignments =
                    await GetActiveAssignments();

                await LoadDropdownData();

                await LoadIndexTechnicianData();

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




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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


            if (!assignment.IsActive)
            {
                TempData["ErrorMessage"] =
                    "This technician test type assignment is already inactive.";

                return RedirectToAction(
                    nameof(Index));
            }


            var activeAssignmentCount =
                await _context.TechnicianTestTypes
                    .CountAsync(a =>
                        a.TechnicianId ==
                        assignment.TechnicianId &&

                        a.IsActive);



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

            assignment.IsActive = false;


            await _context.SaveChangesAsync();


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


            TempData["SuccessMessage"] =
                $"The '{testTypeName}' assignment for " +
                $"'{deletedTechnicianName}' was deactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        private async Task ValidateAssignment(
            TechnicianTestTypeModel model)
        {


            if (string.IsNullOrWhiteSpace(
                model.TechnicianId))
            {
                ModelState.AddModelError(
                    nameof(model.TechnicianId),
                    "Technician is required.");
            }



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
                            TechnicianRole);

                    if (!isTechnician)
                    {
                        ModelState.AddModelError(
                            nameof(model.TechnicianId),
                            "The selected user is not a lab technician.");
                    }


                    if (!technician.IsActive)
                    {
                        ModelState.AddModelError(
                            nameof(model.TechnicianId),
                            "The selected technician account is inactive.");
                    }
                }
            }


            if (model.TestTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.TestTypeId),
                    "Test type is required.");
            }


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


        private async Task LoadDropdownData()
        {


            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            ViewBag.Technicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();


            ViewBag.TestTypes =
                await _context.TestTypes
                    .Where(t =>
                        t.IsActive ==
                        TestTypeModel.RecordStatus.Active)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
        }



        private async Task LoadIndexTechnicianData()
        {


            var technicianUsers =
                await _userManager.GetUsersInRoleAsync(
                    TechnicianRole);


            var activeTechnicians =
                technicianUsers
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();



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




            var techniciansWithoutAssignments =
                activeTechnicians
                    .Where(t =>
                        !technicianAssignmentCounts
                            .ContainsKey(t.Id))
                    .ToList();



            ViewBag.ActiveTechnicians =
                activeTechnicians;

            ViewBag.TechniciansWithoutAssignments =
                techniciansWithoutAssignments;

            ViewBag.TechnicianAssignmentCounts =
                technicianAssignmentCounts;
        }



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