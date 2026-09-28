using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =========================================================
        // AVAILABLE TESTS
        //
        // Tests that:
        // 1. Belong to received requests
        // 2. Are still Submitted
        // 3. Match test types assigned to logged-in technician
        //
        // GET: /Technician/Tests/Available
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Available()
        {
            var technician = await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            // -----------------------------------------------------
            // Get test types assigned to this technician
            // -----------------------------------------------------

            var assignedTestTypeIds =
                await _context.TechnicianTestTypes
                    .AsNoTracking()
                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        x.IsActive)
                    .Select(x => x.TestTypeId)
                    .ToListAsync();


            if (!assignedTestTypeIds.Any())
            {
                return View(
                    new List<TestRequestTestTypeModel>());
            }


            // -----------------------------------------------------
            // Get available request tests
            // -----------------------------------------------------

            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Doctor)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.SampleType)

                    .Where(x =>
                        assignedTestTypeIds.Contains(x.TestTypeId) &&

                        x.Status == "Submitted" &&

                        x.TestRequest != null &&

                        (
                            x.TestRequest.Status ==
                                TestRequestStatus.SamplesReceived.ToString()
                            ||
                            x.TestRequest.Status ==
                                TestRequestStatus.InProgress.ToString()
                        ))

                    .OrderByDescending(x =>
                        x.TestRequest!.Urgency)

                    .ThenBy(x =>
                        x.TestRequest!.RequestDate)

                    .ToListAsync();


            return View(tests);
        }


        // =========================================================
        // SELECT TEST
        //
        // Technician claims/selects a test for processing.
        //
        // POST: /Technician/Tests/Select
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(
            int testRequestTestTypeId)
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            // -----------------------------------------------------
            // Find requested test
            // -----------------------------------------------------

            var requestTest =
                await _context.TestRequestTestTypes

                    .Include(x => x.TestRequest)

                    .Include(x => x.TestType)

                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId ==
                        testRequestTestTypeId);


            if (requestTest == null)
            {
                TempData["ErrorMessage"] =
                    "The selected test could not be found.";

                return RedirectToAction(nameof(Available));
            }


            // -----------------------------------------------------
            // Check current status
            // -----------------------------------------------------

            if (!string.Equals(
                    requestTest.Status,
                    "Submitted",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] =
                    "This test is no longer available for selection.";

                return RedirectToAction(nameof(Available));
            }


            // -----------------------------------------------------
            // Verify technician is assigned to this test type
            // -----------------------------------------------------

            var technicianCanPerformTest =
                await _context.TechnicianTestTypes
                    .AnyAsync(x =>
                        x.TechnicianId == technician.Id &&
                        x.TestTypeId == requestTest.TestTypeId &&
                        x.IsActive);


            if (!technicianCanPerformTest)
            {
                TempData["ErrorMessage"] =
                    "You are not assigned to perform this test type.";

                return RedirectToAction(nameof(Available));
            }


            // -----------------------------------------------------
            // Request must have received its samples
            // -----------------------------------------------------

            if (requestTest.TestRequest == null)
            {
                TempData["ErrorMessage"] =
                    "The test request could not be found.";

                return RedirectToAction(nameof(Available));
            }


            var request = requestTest.TestRequest;


            if (request.Status !=
                    TestRequestStatus.SamplesReceived.ToString()
                &&
                request.Status !=
                    TestRequestStatus.InProgress.ToString())
            {
                TempData["ErrorMessage"] =
                    "Samples must be received before this test can be selected.";

                return RedirectToAction(nameof(Available));
            }


            // -----------------------------------------------------
            // IMPORTANT
            // These are the fields your TestRequestTestTypeModel
            // should contain:
            //
            // TechnicianId
            // StartDateTime
            // CompletionDateTime
            //
            // -----------------------------------------------------

            requestTest.TechnicianId =
                technician.Id;

            requestTest.StartDateTime =
                DateTime.Now;

            requestTest.Status =
                "In Progress";


            // -----------------------------------------------------
            // First selected test changes overall request to
            // In Progress.
            // -----------------------------------------------------

            if (request.Status ==
                TestRequestStatus.SamplesReceived.ToString())
            {
                request.Status =
                    TestRequestStatus.InProgress.ToString();
            }


            // -----------------------------------------------------
            // DEDUCT CONSUMABLES
            // -----------------------------------------------------

            var consumables =
                await _context.TestTypeConsumables

                    .Include(x => x.Consumable)

                    .Where(x =>
                        x.TestTypeId ==
                        requestTest.TestTypeId)

                    .ToListAsync();


            foreach (var item in consumables)
            {
                if (item.Consumable == null)
                {
                    continue;
                }


                if (item.Consumable.QuantityOnHand <
                    item.QuantityRequired)
                {
                    TempData["ErrorMessage"] =
                        $"Insufficient stock for " +
                        $"{item.Consumable.Name}.";

                    return RedirectToAction(
                        nameof(Available));
                }
            }


            // Stock validation passed.
            // Now deduct.
            foreach (var item in consumables)
            {
                if (item.Consumable != null)
                {
                    item.Consumable.QuantityOnHand -=
                        item.QuantityRequired;
                }
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"{requestTest.TestType?.Name ?? "Test"} " +
                "was selected successfully.";


            return RedirectToAction(nameof(MyTests));
        }


        // =========================================================
        // MY TESTS
        //
        // Tests currently assigned to logged-in technician.
        //
        // GET: /Technician/Tests/MyTests
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> MyTests()
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Doctor)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.SampleType)

                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        (
                            x.Status == "In Progress"
                            ||
                            x.Status == "Completed"
                            ||
                            x.Status == "To Be Reviewed"
                        ))

                    .OrderByDescending(x =>
                        x.TestRequest!.Urgency)

                    .ThenBy(x =>
                        x.StartDateTime)

                    .ToListAsync();


            return View(tests);
        }


        // =========================================================
        // TEST DETAILS
        //
        // GET:
        // /Technician/Tests/Details/5
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var test =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Doctor)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.SampleType)

                    .Include(x => x.TestType)
                        .ThenInclude(t =>
                            t!.MeasurementUnitDetails)

                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == id &&
                        x.TechnicianId == technician.Id);


            if (test == null)
            {
                return NotFound();
            }


            return View(test);
        }


        // =========================================================
        // TESTS TO BE REVIEWED
        //
        // Returned by verifier to original technician.
        //
        // GET: /Technician/Tests/Review
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Review()
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        x.Status == "To Be Reviewed")

                    .OrderByDescending(x =>
                        x.TestRequest!.Urgency)

                    .ThenBy(x =>
                        x.StartDateTime)

                    .ToListAsync();


            return View(tests);
        }


        // =========================================================
        // URGENT / STAT TESTS
        //
        // GET: /Technician/Tests/Urgent
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Urgent()
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var assignedTestTypeIds =
                await _context.TechnicianTestTypes

                    .AsNoTracking()

                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        x.IsActive)

                    .Select(x => x.TestTypeId)

                    .ToListAsync();


            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Where(x =>
                        assignedTestTypeIds.Contains(
                            x.TestTypeId)

                        &&

                        x.TestRequest != null

                        &&

                        x.TestRequest.Urgency ==
                            Urgency.Stat

                        &&

                        (
                            x.Status == "Submitted"
                            ||
                            x.Status == "In Progress"
                        ))

                    .OrderBy(x =>
                        x.TestRequest!.RequestDate)

                    .ToListAsync();


            return View(tests);
        }


        // =========================================================
        // OVERDUE TESTS
        //
        // Due date =
        // StartDateTime + TestType.TurnAroundTimeHours
        //
        // GET: /Technician/Tests/Overdue
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Overdue()
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var now = DateTime.Now;


            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Where(x =>
                        x.TechnicianId ==
                            technician.Id

                        &&

                        x.Status == "In Progress"

                        &&

                        x.StartDateTime != null)

                    .ToListAsync();


            // DateTime.AddHours with another table value can be
            // awkward for EF translation, so evaluate after query.
            var overdueTests =
                tests
                    .Where(x =>
                        x.TestType != null &&
                        x.StartDateTime.HasValue &&
                        x.StartDateTime.Value
                            .AddHours(
                                x.TestType
                                    .TurnAroundTimeHours)
                            < now)

                    .OrderBy(x =>
                        x.StartDateTime)

                    .ToList();


            return View(overdueTests);
        }


        // =========================================================
        // NEARING TAT
        //
        // Here we define "nearing TAT" as <= 1 hour remaining.
        //
        // GET: /Technician/Tests/NearingTat
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> NearingTat()
        {
            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Unauthorized();
            }


            var now = DateTime.Now;


            var tests =
                await _context.TestRequestTestTypes

                    .AsNoTracking()

                    .Include(x => x.TestRequest)
                        .ThenInclude(r => r!.Patient)

                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)

                    .Where(x =>
                        x.TechnicianId ==
                            technician.Id

                        &&

                        x.Status == "In Progress"

                        &&

                        x.StartDateTime != null)

                    .ToListAsync();


            var nearingTat =
                tests
                    .Where(x =>
                    {
                        if (x.TestType == null ||
                            !x.StartDateTime.HasValue)
                        {
                            return false;
                        }


                        var due =
                            x.StartDateTime.Value
                                .AddHours(
                                    x.TestType
                                        .TurnAroundTimeHours);


                        var remaining =
                            due - now;


                        return remaining.TotalMinutes > 0
                               &&
                               remaining.TotalMinutes <= 60;
                    })

                    .OrderBy(x =>
                        x.StartDateTime)

                    .ToList();


            return View(nearingTat);
        }
    }
}
