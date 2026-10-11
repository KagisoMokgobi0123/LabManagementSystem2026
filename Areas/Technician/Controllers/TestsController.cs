using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using System.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Areas.Technician.ViewModels;

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



        // GET: /Technician/Tests/AssignedTestTypes
        [HttpGet]
        public async Task<IActionResult> AssignedTestTypes()
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
            {
                return Challenge();
            }

            var assignedTestTypes = await _context.TechnicianTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.TestCategory)
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.SampleType)
                .Where(x =>
                    x.TechnicianId == technicianId &&
                    x.IsActive)
                .OrderBy(x => x.TestType!.Name)
                .ToListAsync();

            return View(assignedTestTypes);
        }





        [HttpGet]
        public async Task<IActionResult> Available()
        {
            // STEP 1: Identify logged-in technician
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            // STEP 2: Retrieve Laboratory Manager assignments
            // Keep both active and revoked assignments for audit visibility.
            var assignmentRecords = await (
                from assignment in _context.TechnicianTestTypes
                    .AsNoTracking()
                join test in _context.TestTypes
                    .AsNoTracking()
                    on assignment.TestTypeId equals test.TestTypeId
                where assignment.TechnicianId == technicianId
                select new TechnicianAssignmentRowViewModel
                {
                    TestTypeId = test.TestTypeId,
                    Name = test.Name,
                    Category = test.TestCategory != null
                        ? test.TestCategory.Description
                        : null,
                    SampleType = test.SampleType != null
                        ? test.SampleType.Name
                        : null,
                    IsActive = assignment.IsActive
                }
            ).ToListAsync();

            // STEP 3: Keep one row per assigned test type.
            // If historical duplicates exist, prefer an active assignment.
            var assignments = assignmentRecords
                .GroupBy(x => x.TestTypeId)
                .Select(g => g
                    .OrderByDescending(x => x.IsActive)
                    .First())
                .OrderBy(x => x.Name)
                .ToList();

            // STEP 4: Retrieve patient tests selected by technician
            var selectedTests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                .Where(x => x.TechnicianId == technicianId)
                .OrderByDescending(x => x.StartDateTime)
                .ThenBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            // STEP 5: Retrieve associated patients
            var patientIds = selectedTests
                .Where(x => x.TestRequest != null)
                .Select(x => x.TestRequest!.PatientId)
                .Distinct()
                .ToList();

            var patients = await _context.Patients
                .AsNoTracking()
                .Where(x => patientIds.Contains(x.PatientId))
                .ToDictionaryAsync(x => x.PatientId);

            // STEP 6: Map selected tests and deadlines
            var deadlineRows = selectedTests.Select(x =>
            {
                var request = x.TestRequest;

                var patient = request != null &&
                              patients.TryGetValue(
                                  request.PatientId,
                                  out var matchedPatient)
                    ? matchedPatient
                    : null;

                return new TechnicianDeadlineRowViewModel
                {
                    RequestReference =
                        request?.RequestNumber
                        ?? x.TestRequestId.ToString(),

                    PatientName =
                        patient?.FullName
                        ?? "Patient not found",

                    TestTypeName =
                        x.TestType?.Name
                        ?? "Unknown Test",

                    Status = x.Status,

                    // Stored as UTC when test is selected
                    StartDateTimeUtc = x.StartDateTime,

                    TurnaroundHours = x.TestType == null
                        ? null
                        : (double?)x.TestType.TurnAroundTimeHours
                };
            }).ToList();

            // STEP 7: Pass deadline information to the unified view
            ViewBag.SelectedTests = deadlineRows;

            // STEP 8: Return Manager assignments
            // The view combines both collections into ONE table.
            return View("Available", assignments);
        }


        [HttpGet]
        public async Task<IActionResult> AssignmentDetails(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            var assignment = await (
                from a in _context.TechnicianTestTypes.AsNoTracking()
                join t in _context.TestTypes.AsNoTracking()
                    on a.TestTypeId equals t.TestTypeId
                where a.TechnicianId == technicianId
                      && a.TestTypeId == id
                select new TechnicianAssignmentRowViewModel
                {
                    TestTypeId = t.TestTypeId,
                    Name = t.Name,
                    Category = t.TestCategory != null
                        ? t.TestCategory.Description : null,
                    SampleType = t.SampleType != null
                        ? t.SampleType.Name : null,
                    IsActive = a.IsActive
                }
            ).FirstOrDefaultAsync();

            if (assignment == null)
                return NotFound();

            return View(assignment);
        }


        // Replace the existing GET and POST Select actions in TestsController with these methods.
        // StartDateTime is stored in UTC; convert to local time only when displaying it.

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            // STEP 1: Get logged-in technician
            var technician = await _userManager.GetUserAsync(User);

            if (technician == null)
                return Challenge();

            // STEP 2: Get active test types assigned to this technician
            var assignedTestTypeIds = await _context.TechnicianTestTypes
                .AsNoTracking()
                .Where(x =>
                    x.TechnicianId == technician.Id &&
                    x.IsActive)
                .Select(x => x.TestTypeId)
                .Distinct()
                .ToListAsync();

            // STEP 3: Get available patient tests
            var availableTests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                .Where(x =>
                    assignedTestTypeIds.Contains(x.TestTypeId) &&

                    x.Status == "SamplesReceived" &&

                    (x.TechnicianId == null ||
                     x.TechnicianId == "") &&

                    x.TestRequest != null &&

                    (
                        x.TestRequest.Status ==
                            TestRequestStatus.SamplesReceived.ToString() ||

                        x.TestRequest.Status ==
                            TestRequestStatus.InProgress.ToString()
                    ))
                .OrderBy(x => x.TestRequestId)
                .ThenBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            // STEP 4: Collect patient and doctor IDs
            var patientIds = availableTests
                .Where(x => x.TestRequest != null)
                .Select(x => x.TestRequest!.PatientId)
                .Distinct()
                .ToList();

            var doctorIds = availableTests
                .Where(x => x.TestRequest != null)
                .Select(x => x.TestRequest!.DoctorId)
                .Distinct()
                .ToList();

            // STEP 5: Retrieve matching patient records
            var patients = await _context.Patients
                .AsNoTracking()
                .Where(x => patientIds.Contains(x.PatientId))
                .ToDictionaryAsync(x => x.PatientId);

            // STEP 6: Retrieve matching doctor Identity accounts
            var doctorUsers = await _userManager.Users
                .AsNoTracking()
                .Where(x => doctorIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            // STEP 7: Map results to the view model
            var model = availableTests.Select(x =>
            {
                var request = x.TestRequest;

                var patient = request != null &&
                              patients.TryGetValue(
                                  request.PatientId,
                                  out var matchedPatient)
                    ? matchedPatient
                    : null;

                var doctor = request != null &&
                             doctorUsers.TryGetValue(
                                 request.DoctorId,
                                 out var matchedDoctor)
                    ? matchedDoctor
                    : null;

                return new SelectPatientTestViewModel
                {
                    RequestedTestId =
                        x.TestRequestTestTypeId,

                    RequestReference =
                        request?.RequestNumber
                        ?? x.TestRequestId.ToString(),

                    PatientName =
                        patient?.FullName
                        ?? "Patient not found",

                    PatientIdNumber =
                        patient?.IDNumber
                        ?? "Not available",

                    DoctorFullName =
                        doctor?.FullName
                        ?? "Doctor not found",

                    DoctorUsername =
                        doctor?.UserName
                        ?? "Not available",

                    ClinicalNotes =
                        request?.ClinicalNotes
                        ?? "No clinical notes provided",

                    TestTypeName =
                        x.TestType?.Name
                        ?? "Unknown Test",

                    Priority =
                        request?.Urgency.ToString()
                        ?? "Not specified",

                    TurnaroundDisplay =
                        x.TestType == null
                            ? "Not specified"
                            : $"{x.TestType.TurnAroundTimeHours} hours",

                    RequestStatus =
                        request?.Status
                        ?? "Unknown",

                    TestStatus = x.Status
                };
            }).ToList();

            // STEP 8: Return your existing Select.cshtml
            return View("Select", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(int testRequestTestTypeId)
        {
            var technician = await _userManager.GetUserAsync(User);

            if (technician == null)
                return Challenge();

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            try
            {
                // -----------------------------------------------------
                // 1. Find the requested patient test
                // -----------------------------------------------------
                var requestTest = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .Include(x => x.TestType)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == testRequestTestTypeId);

                if (requestTest == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected patient test could not be found.";

                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 2. Ensure the sample has been received and test is available
                // -----------------------------------------------------
                if (!string.Equals(
                        requestTest.Status,
                        "SamplesReceived",
                        StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] =
                        "This test is no longer available for selection. Its sample must be received and it must not already be in progress.";

                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 3. Ensure the test has not been claimed
                // -----------------------------------------------------
                if (!string.IsNullOrEmpty(requestTest.TechnicianId))
                {
                    TempData["ErrorMessage"] =
                        "Another technician has already selected this test.";

                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 4. Verify active manager authorization
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
                        "You are not authorized to perform this test type. " +
                        "Your assignment may have been deactivated.";

                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 5. Check the parent test request
                // -----------------------------------------------------
                var request = requestTest.TestRequest;

                if (request == null)
                {
                    TempData["ErrorMessage"] =
                        "The parent test request could not be found.";

                    return RedirectToAction(nameof(Select));
                }

                var samplesReceived =
                    TestRequestStatus.SamplesReceived.ToString();

                var inProgress =
                    TestRequestStatus.InProgress.ToString();

                if (request.Status != samplesReceived &&
                    request.Status != inProgress)
                {
                    TempData["ErrorMessage"] =
                        "Samples must be received before this test can be selected.";

                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 6. Retrieve required consumables
                // -----------------------------------------------------
                var consumables = await _context.TestTypeConsumables
                    .Include(x => x.Consumable)
                    .Where(x =>
                        x.TestTypeId == requestTest.TestTypeId)
                    .ToListAsync();

                // -----------------------------------------------------
                // 7. Validate consumable availability
                // -----------------------------------------------------
                foreach (var item in consumables)
                {
                    if (item.Consumable == null)
                    {
                        TempData["ErrorMessage"] =
                            "A required consumable could not be found.";

                        return RedirectToAction(nameof(Select));
                    }

                    if (item.QuantityRequired <= 0)
                    {
                        TempData["ErrorMessage"] =
                            "Invalid consumable quantity configuration.";

                        return RedirectToAction(nameof(Select));
                    }

                    if (item.Consumable.QuantityOnHand <
                        item.QuantityRequired)
                    {
                        TempData["ErrorMessage"] =
                            $"Insufficient stock for {item.Consumable.Name}.";

                        return RedirectToAction(nameof(Select));
                    }
                }

                // -----------------------------------------------------
                // 8. Record selection and update test progress
                // -----------------------------------------------------
                requestTest.TechnicianId = technician.Id;
                requestTest.StartDateTime = DateTime.UtcNow;
                requestTest.CompletionDateTime = null;
                requestTest.Status = "In Progress";

                // -----------------------------------------------------
                // 9. Update the parent request when testing starts
                // -----------------------------------------------------
                if (request.Status == samplesReceived)
                {
                    request.Status = inProgress;
                }

                // -----------------------------------------------------
                // 10. Deduct required consumables
                // -----------------------------------------------------
                foreach (var item in consumables)
                {
                    item.Consumable!.QuantityOnHand -=
                        item.QuantityRequired;
                }

                // -----------------------------------------------------
                // 11. Save and commit all changes
                // -----------------------------------------------------
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    $"{requestTest.TestType?.Name ?? "Test"} " +
                    "has been selected successfully and is now In Progress.";

                return RedirectToAction(nameof(MyTests));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "Another user modified this test or its stock. " +
                    "Please refresh the page and try again.";

                return RedirectToAction(nameof(Select));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "The test could not be selected. Please try again.";

                return RedirectToAction(nameof(Select));
            }
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
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> Details(int id)
        {
            // ==========================================
            // 1. IDENTIFY LOGGED-IN TECHNICIAN
            // ==========================================

            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                return Challenge();
            }

            // ==========================================
            // 2. RETRIEVE REQUESTED TEST
            // ==========================================

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
                        x.TestRequestTestTypeId == id);

            if (test == null)
            {
                return NotFound();
            }

            // ==========================================
            // 3. CHECK ORIGINAL TECHNICIAN
            // ==========================================

            bool isTestOwner =
                test.TechnicianId == technician.Id;

            // ==========================================
            // 4. CHECK INDEPENDENT VERIFIER
            // ==========================================

            bool canVerify = false;

            if (!isTestOwner &&
                test.Status == "Completed")
            {
                canVerify =
                    await _context.Set<TechnicianTestTypeModel>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.TechnicianId == technician.Id &&
                            x.TestTypeId == test.TestTypeId);
            }

            // ==========================================
            // 5. ENFORCE ACCESS CONTROL
            // ==========================================

            if (!isTestOwner && !canVerify)
            {
                return Forbid();
            }

            // ==========================================
            // 6. PASS PERMISSIONS TO VIEW
            // ==========================================

            ViewBag.IsTestOwner = isTestOwner;
            ViewBag.CanVerify = canVerify;

            // ==========================================
            // 7. RETRIEVE CAPTURED RESULTS
            // ==========================================

            var results = await _context.TestResults
                .AsNoTracking()
                .Where(x =>
                    x.TestRequestTestTypeId == id)
                .OrderBy(x => x.ResultName)
                .ToListAsync();

            ViewBag.TestResults = results;

            // ==========================================
            // 8. RETRIEVE MANAGER-APPROVED MEASUREMENTS
            // ==========================================

            var measurements =
                await _context.TestTypeMeasurements

                    .AsNoTracking()

                    .Include(x => x.MeasurementUnit)

                    .Where(x =>
                        x.TestTypeId == test.TestTypeId)

                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.MeasurementName)

                    .ToListAsync();

            ViewBag.TestMeasurements = measurements;

            // ==========================================
            // 9. RETURN TEST DETAILS
            // ==========================================

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

        private void RecordResultHistory(
            TestResultModel result,
            string technicianId,
            string action,
            string? notes,
            string? previousValue = null,
            string? newValue = null)
        {
            _context.TestResultVerificationHistories.Add(
                new TestResultVerificationHistoryModel
                {
                    TestResultId = result.TestResultId,
                    TestResult = result,
                    ActionByTechnicianId = technicianId,
                    Action = action,
                    Notes = notes,
                    PreviousResultValue = previousValue,
                    NewResultValue = newValue,
                    ActionDateTime = DateTime.UtcNow
                });
        }

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


  

        [HttpGet]
        public async Task<IActionResult> CaptureResults(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            // -----------------------------------------------------
            // Retrieve the test assigned to this technician
            // -----------------------------------------------------

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()

                // Patient
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)

                // Requesting doctor
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Doctor)

                // Test category
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.TestCategory)

                // Sample type
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.SampleType)

                // Measurement units
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.MeasurementUnitDetails)

                .AsSplitQuery()

                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id &&
                    x.TechnicianId == technicianId);

            if (test == null)
                return NotFound();

            // -----------------------------------------------------
            // Check test status
            // -----------------------------------------------------

            if (test.Status != "In Progress" &&
                test.Status != "To Be Reviewed")
            {
                TempData["ErrorMessage"] =
                    "This test is not available for result capture.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            // -----------------------------------------------------
            // Retrieve patient medical history
            // -----------------------------------------------------

            var patientUserId = test.TestRequest?.Patient?.Id;

            NMB_HLabSys.Areas.Patient.Models.PatientModel?
                patientMedicalRecord = null;

            if (!string.IsNullOrEmpty(patientUserId))
            {
                patientMedicalRecord = await _context
                    .Set<NMB_HLabSys.Areas.Patient.Models.PatientModel>()
                    .AsNoTracking()
                    .Include(p => p.PatientConditions)
                    .Include(p => p.PatientAllergies)
                    .Include(p => p.PatientMedications)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p =>
                        p.UserId == patientUserId);
            }

            ViewData["PatientMedicalRecord"] = patientMedicalRecord;

            // -----------------------------------------------------
            // Expected completion time
            // -----------------------------------------------------

            DateTime? expectedCompletion = null;
            bool isOverdue = false;

            if (test.StartDateTime.HasValue &&
                test.TestType != null)
            {
                expectedCompletion =
    test.StartDateTime.Value.AddMinutes(
        test.TestType.TurnAroundTimeHours);

                isOverdue =
                    test.Status == "In Progress" &&
                    DateTime.Now > expectedCompletion.Value;
            }

            ViewData["ExpectedCompletion"] = expectedCompletion;
            ViewData["IsOverdue"] = isOverdue;

            // -----------------------------------------------------
            // Display Capture Results view
            // -----------------------------------------------------

            return View(test);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]


       
        public async Task<IActionResult> CaptureResults(
    int testRequestTestTypeId,
    int testTypeMeasurementId,
    string resultValue,
    string? notes)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            // ==========================================
            // VALIDATE USER INPUT
            // ==========================================

            if (testTypeMeasurementId <= 0 ||
                string.IsNullOrWhiteSpace(resultValue))
            {
                TempData["ErrorMessage"] =
                    "Select a measurement and enter its result.";

                return RedirectToAction(
                    nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }

            resultValue = resultValue.Trim();
            notes = notes?.Trim();

            if (resultValue.Length > 200 ||
                (notes?.Length ?? 0) > 2000)
            {
                TempData["ErrorMessage"] =
                    "One or more fields exceed the allowed length.";

                return RedirectToAction(
                    nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }

            // ==========================================
            // BEGIN TRANSACTION
            // ==========================================

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            try
            {
                // ==========================================
                // FIND TEST AND CHECK OWNERSHIP
                // ==========================================

                var test = await _context.TestRequestTestTypes
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == testRequestTestTypeId &&
                        x.TechnicianId == technicianId);

                if (test == null)
                    return NotFound();

                if (test.Status != "In Progress" &&
                    test.Status != "To Be Reviewed")
                {
                    TempData["ErrorMessage"] =
                        "This test cannot accept measurements.";

                    return RedirectToAction(nameof(MyTests));
                }

                // ==========================================
                // RETRIEVE MANAGER-APPROVED MEASUREMENT
                // ==========================================

                var definition = await _context.TestTypeMeasurements
                    .AsNoTracking()
                    .Include(x => x.MeasurementUnit)
                    .FirstOrDefaultAsync(x =>
                        x.TestTypeMeasurementId == testTypeMeasurementId &&
                        x.TestTypeId == test.TestTypeId);

                if (definition == null ||
                    definition.MeasurementUnit == null ||
                    string.IsNullOrWhiteSpace(definition.MeasurementUnit.Unit))
                {
                    TempData["ErrorMessage"] =
                        "The selected measurement is not configured correctly.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                // These values come from the approved catalogue,
                // not from technician-submitted form fields.

                string resultName = definition.MeasurementName;
                string measurementUnit = definition.MeasurementUnit.Unit;

                decimal? referenceMin = definition.NormalMin;
                decimal? referenceMax = definition.NormalMax;

                if (string.IsNullOrWhiteSpace(resultName) ||
                    resultName.Length > 200 ||
                    measurementUnit.Length > 100)
                {
                    TempData["ErrorMessage"] =
                        "The measurement catalogue contains invalid details.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                if (referenceMin.HasValue &&
                    referenceMax.HasValue &&
                    referenceMin.Value > referenceMax.Value)
                {
                    TempData["ErrorMessage"] =
                        "The configured reference range is invalid.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                // ==========================================
                // CLASSIFY RESULT
                // ==========================================

                bool isAbnormal = false;
                string resultFlag = "Not Evaluated";

                if (decimal.TryParse(
                    resultValue,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out decimal numericValue))
                {
                    if (referenceMin.HasValue &&
                        numericValue < referenceMin.Value)
                    {
                        isAbnormal = true;
                        resultFlag = "Low";
                    }
                    else if (referenceMax.HasValue &&
                             numericValue > referenceMax.Value)
                    {
                        isAbnormal = true;
                        resultFlag = "High";
                    }
                    else if (referenceMin.HasValue ||
                             referenceMax.HasValue)
                    {
                        resultFlag = "Normal";
                    }
                }

                // ==========================================
                // FIND RESULT BY MEASUREMENT ID
                // ==========================================

                var result = await _context.TestResults
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == testRequestTestTypeId &&
                        x.TestTypeMeasurementId == testTypeMeasurementId);

                // Prevent duplicate results for historical records
                // that have not yet been linked to a measurement ID.
                if (result == null)
                {
                    bool hasUnlinkedResult = await _context.TestResults
                        .AnyAsync(x =>
                            x.TestRequestTestTypeId == testRequestTestTypeId &&
                            x.TestTypeMeasurementId == null &&
                            x.ResultName == resultName);

                    if (hasUnlinkedResult)
                    {
                        TempData["ErrorMessage"] =
                            "An existing result needs to be linked to this " +
                            "measurement before it can be edited. " +
                            "Please contact the Laboratory Manager.";

                        return RedirectToAction(
                            nameof(CaptureResults),
                            new { id = testRequestTestTypeId });
                    }
                }

                bool isNewResult = result == null;

                // ==========================================
                // SNAPSHOT PREVIOUS VALUES FOR AUDIT
                // ==========================================

                string? oldValue = result?.ResultValue;
                string? oldName = result?.ResultName;
                string? oldUnit = result?.MeasurementUnit;

                decimal? oldMin = result?.ReferenceMin;
                decimal? oldMax = result?.ReferenceMax;

                string? oldFlag = result?.ResultFlag;
                bool? oldAbnormal = result?.IsAbnormal;

                string? oldNotes = result?.Notes;

                // ==========================================
                // CREATE OR AUTHORIZE RESULT
                // ==========================================

                if (result == null)
                {
                    result = new TestResultModel
                    {
                        TestRequestTestTypeId = testRequestTestTypeId,
                        TestTypeMeasurementId = testTypeMeasurementId,
                        TechnicianId = technicianId
                    };

                    _context.TestResults.Add(result);
                }
                else if (result.TechnicianId != technicianId)
                {
                    return Forbid();
                }

                // ==========================================
                // DETECT CHANGES
                // ==========================================

                bool changed =
                    isNewResult ||
                    oldValue != resultValue ||
                    oldName != resultName ||
                    oldUnit != measurementUnit ||
                    oldMin != referenceMin ||
                    oldMax != referenceMax ||
                    oldFlag != resultFlag ||
                    oldAbnormal != isAbnormal ||
                    oldNotes != notes;

                // ==========================================
                // SAVE MEASUREMENT VALUES
                // ==========================================

                result.TestTypeMeasurementId = testTypeMeasurementId;
                result.ResultName = resultName;
                result.ResultValue = resultValue;
                result.MeasurementUnit = measurementUnit;

                result.ReferenceMin = referenceMin;
                result.ReferenceMax = referenceMax;

                result.IsAbnormal = isAbnormal;
                result.ResultFlag = resultFlag;

                result.Notes = notes;

                // ==========================================
                // RECORD FULL AUDIT HISTORY
                // ==========================================

                if (changed)
                {
                    var now = DateTime.UtcNow;

                    result.CapturedDateTime = now;

                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResult = result,

                            ActionByTechnicianId = technicianId,

                            Action = isNewResult
                                ? "Submitted"
                                : "Corrected",

                            Notes = isNewResult
                                ? "Measurement captured."
                                : "Measurement details updated.",

                            ActionDateTime = now,

                            PreviousResultValue = oldValue,
                            NewResultValue = resultValue,

                            PreviousMeasurementUnit = oldUnit,
                            NewMeasurementUnit = measurementUnit,

                            PreviousReferenceMin = oldMin,
                            NewReferenceMin = referenceMin,

                            PreviousReferenceMax = oldMax,
                            NewReferenceMax = referenceMax,

                            PreviousResultFlag = oldFlag,
                            NewResultFlag = resultFlag,

                            PreviousIsAbnormal = oldAbnormal,
                            NewIsAbnormal = isAbnormal,

                            PreviousResultNotes = oldNotes,
                            NewResultNotes = notes
                        });
                }

                // ==========================================
                // PRESERVE VERIFICATION WORKFLOW
                // ==========================================

                // Do not mark the test Completed here.
                // Do not clear verification or review notes here.
                // FinishTest handles submission or resubmission.

                // ==========================================
                // SAVE AND COMMIT
                // ==========================================

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = changed
                    ? "Measurement saved successfully. " +
                      "Capture another measurement or finish the test."
                    : "No changes were detected in this measurement.";

                return RedirectToAction(
                    nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "An error occurred while saving the measurement.";

                return RedirectToAction(
                    nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }
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

        // =====================================================
        // 1. PENDING TEST RESULT VERIFICATION
        // =====================================================


        [HttpGet]
        public async Task<IActionResult> CaptureResultsList()
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            var tests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.TestCategory)
                .Include(x => x.TestRequest)
                .Where(x =>
                    x.TechnicianId == technicianId &&
                    (x.Status == "In Progress" ||
                     x.Status == "To Be Reviewed"))
                .OrderBy(x => x.StartDateTime)
                .ThenBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            return View(tests);
        }



        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> PendingVerification()
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            var assignedTestTypeIds =
                _context.Set<TechnicianTestTypeModel>()
                    .AsNoTracking()
                    .Where(x => x.TechnicianId == technicianId)
                    .Select(x => x.TestTypeId);

            var tests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                .Where(x =>
                    x.Status == "Completed" &&
                    x.TechnicianId != technicianId &&
                    assignedTestTypeIds.Contains(x.TestTypeId))
                .OrderBy(x => x.CompletionDateTime)
                .ToListAsync();

            return View(tests);
        }




        // =====================================================
        // 2. VERIFY RESULT - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> VerifyResult(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Doctor)
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.SampleType)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != "Completed")
            {
                TempData["ErrorMessage"] =
                    "Only completed tests can be verified.";

                return RedirectToAction(
                    nameof(PendingVerification));
            }

            // Technician cannot verify their own test
            if (test.TechnicianId == technicianId)
                return Forbid();

            // Technician must be authorised for the test type
            var authorised =
                await _context.TechnicianTestTypes
                    .AnyAsync(x =>
                        x.TechnicianId == technicianId &&
                        x.TestTypeId == test.TestTypeId &&
                        x.IsActive);

            if (!authorised)
                return Forbid();

            var results = await _context.TestResults
                .AsNoTracking()
                .Where(x =>
                    x.TestRequestTestTypeId == id)
                .OrderBy(x => x.ResultName)
                .ToListAsync();

            ViewData["TestResults"] = results;

            return View(test);
        }


        // =====================================================
        // 3. VERIFY RESULT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResult(
            int testRequestTestTypeId,
            string decision,
            string? notes)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            if (decision != "Verify" &&
                decision != "ReturnForReview")
            {
                TempData["ErrorMessage"] =
                    "Invalid verification decision.";

                return RedirectToAction(
                    nameof(VerifyResult),
                    new { id = testRequestTestTypeId });
            }

            if (decision == "ReturnForReview" &&
                string.IsNullOrWhiteSpace(notes))
            {
                TempData["ErrorMessage"] =
                    "Review notes are required when returning results.";

                return RedirectToAction(
                    nameof(VerifyResult),
                    new { id = testRequestTestTypeId });
            }

            if ((notes?.Length ?? 0) > 2000)
            {
                TempData["ErrorMessage"] =
                    "Verification notes cannot exceed 2000 characters.";

                return RedirectToAction(
                    nameof(VerifyResult),
                    new { id = testRequestTestTypeId });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId ==
                            testRequestTestTypeId);

                if (test == null)
                    return NotFound();

                if (test.Status != "Completed")
                {
                    TempData["ErrorMessage"] =
                        "This test is no longer awaiting verification.";

                    return RedirectToAction(
                        nameof(PendingVerification));
                }

                if (string.IsNullOrEmpty(test.TechnicianId) ||
                    test.TechnicianId == technicianId)
                {
                    return Forbid();
                }

                var authorised =
                    await _context.TechnicianTestTypes
                        .AnyAsync(x =>
                            x.TechnicianId == technicianId &&
                            x.TestTypeId == test.TestTypeId &&
                            x.IsActive);

                if (!authorised)
                    return Forbid();

                var results = await _context.TestResults
                    .Where(x =>
                        x.TestRequestTestTypeId ==
                            testRequestTestTypeId)
                    .ToListAsync();

                if (results.Count == 0)
                {
                    TempData["ErrorMessage"] =
                        "No captured results were found for this test.";

                    return RedirectToAction(
                        nameof(PendingVerification));
                }

                var actionDateTime = DateTime.UtcNow;

                if (decision == "Verify")
                {
                    // -----------------------------------------
                    // APPROVE ALL RESULTS
                    // -----------------------------------------

                    foreach (var result in results)
                    {
                        result.VerifiedByTechnicianId =
                            technicianId;

                        result.VerifiedDateTime =
                            actionDateTime;

                        result.VerificationNotes =
                            notes?.Trim();

                        result.RequiresReview = false;
                        result.ReviewNotes = null;

                        _context.TestResultVerificationHistories.Add(
                            new TestResultVerificationHistoryModel
                            {
                                TestResultId = result.TestResultId,
                                ActionByTechnicianId = technicianId,
                                Action = "Verified",
                                Notes = notes?.Trim(),
                                ActionDateTime = actionDateTime
                            });
                    }

                    test.Status = "Verified";
                }
                else
                {
                    // -----------------------------------------
                    // RETURN RESULTS FOR REVIEW
                    // -----------------------------------------

                    foreach (var result in results)
                    {
                        result.VerifiedByTechnicianId = null;
                        result.VerifiedDateTime = null;
                        result.VerificationNotes = null;

                        result.RequiresReview = true;
                        result.ReviewNotes = notes!.Trim();

                        _context.TestResultVerificationHistories.Add(
                            new TestResultVerificationHistoryModel
                            {
                                TestResultId = result.TestResultId,
                                ActionByTechnicianId = technicianId,
                                Action = "ReturnedForReview",
                                Notes = notes.Trim(),
                                ActionDateTime = actionDateTime
                            });
                    }

                    test.Status = "To Be Reviewed";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    decision == "Verify"
                        ? "Test results verified successfully."
                        : "Test results returned to the original technician for review.";

                return RedirectToAction(
                    nameof(PendingVerification));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "The verification could not be saved. Please try again.";

                return RedirectToAction(
                    nameof(PendingVerification));
            }
        }


        // =====================================================
        // 4. VERIFICATION HISTORY
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> VerificationHistory(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Original technician or a technician authorised
            // for this test type may view verification history.
            var authorised =
                test.TechnicianId == technicianId ||
                await _context.TechnicianTestTypes
                    .AnyAsync(x =>
                        x.TechnicianId == technicianId &&
                        x.TestTypeId == test.TestTypeId &&
                        x.IsActive);

            if (!authorised)
                return Forbid();

            var history =
                await _context.TestResultVerificationHistories
                    .AsNoTracking()
                    .Include(x => x.TestResult)
                    .Include(x => x.ActionByTechnician)
                    .Where(x =>
                        x.TestResult != null &&
                        x.TestResult.TestRequestTestTypeId == id)
                    .OrderByDescending(x => x.ActionDateTime)
                    .ToListAsync();

            ViewData["Test"] = test;

            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            try
            {
                // ==========================================
                // 1. RETRIEVE TEST AND VERIFY OWNERSHIP
                // ==========================================

                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == id &&
                        x.TechnicianId == technicianId);

                if (test == null)
                    return NotFound();

                // ==========================================
                // 2. CHECK ALLOWED TEST STATUS
                // ==========================================

                if (test.Status != "In Progress" &&
                    test.Status != "To Be Reviewed")
                {
                    TempData["ErrorMessage"] =
                        "Only tests in progress or returned for review " +
                        "can be submitted for verification.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id });
                }

                bool isResubmission =
                    test.Status == "To Be Reviewed";

                // ==========================================
                // 3. RETRIEVE APPROVED MEASUREMENTS
                // ==========================================

                var definitions = await _context.TestTypeMeasurements
                    .AsNoTracking()
                    .Where(x => x.TestTypeId == test.TestTypeId)
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.MeasurementName)
                    .ToListAsync();

                if (definitions.Count == 0)
                {
                    TempData["ErrorMessage"] =
                        "No measurements have been configured for this " +
                        "test type. Contact the Laboratory Manager.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id });
                }

                // ==========================================
                // 4. RETRIEVE CAPTURED RESULTS
                // ==========================================

                var results = await _context.TestResults
                    .Where(x =>
                        x.TestRequestTestTypeId == id &&
                        x.TechnicianId == technicianId)
                    .ToListAsync();

                if (results.Count == 0)
                {
                    TempData["ErrorMessage"] =
                        "Capture at least one measurement before " +
                        "finishing the test.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id });
                }

                // ==========================================
                // 5. CHECK REQUIRED MEASUREMENTS
                // ==========================================

                var capturedIds = results
                    .Where(x =>
                        x.TestTypeMeasurementId.HasValue &&
                        !string.IsNullOrWhiteSpace(x.ResultValue))
                    .Select(x => x.TestTypeMeasurementId!.Value)
                    .ToHashSet();

                var missingMeasurements = definitions
                    .Where(x =>
                        x.IsRequired &&
                        !capturedIds.Contains(
                            x.TestTypeMeasurementId))
                    .Select(x => x.MeasurementName)
                    .ToList();

                if (missingMeasurements.Count > 0)
                {
                    TempData["ErrorMessage"] =
                        "The following required measurements " +
                        "have not been captured: " +
                        string.Join(", ", missingMeasurements);

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id });
                }

                // ==========================================
                // 6. VALIDATE MEASUREMENT ASSOCIATIONS
                // ==========================================

                var validDefinitionIds = definitions
                    .Select(x => x.TestTypeMeasurementId)
                    .ToHashSet();

                bool containsInvalidMeasurement = results.Any(x =>
                    !x.TestTypeMeasurementId.HasValue ||
                    !validDefinitionIds.Contains(
                        x.TestTypeMeasurementId.Value));

                if (containsInvalidMeasurement)
                {
                    TempData["ErrorMessage"] =
                        "One or more results are not linked to " +
                        "valid test measurements. Contact the " +
                        "Laboratory Manager.";

                    return RedirectToAction(
                        nameof(CaptureResults),
                        new { id });
                }

                // ==========================================
                // 7. CLEAR PREVIOUS VERIFICATION STATE
                // ==========================================

                var now = DateTime.UtcNow;

                foreach (var result in results)
                {
                    // Record the resubmission before clearing
                    // previous verification information.

                    if (isResubmission)
                    {
                        _context.TestResultVerificationHistories.Add(
                            new TestResultVerificationHistoryModel
                            {
                                TestResultId = result.TestResultId,
                                ActionByTechnicianId = technicianId,

                                Action = "Resubmitted",

                                Notes =
                                    "Measurement resubmitted for " +
                                    "independent verification.",

                                ActionDateTime = now,

                                NewResultValue = result.ResultValue,
                                NewMeasurementUnit =
                                    result.MeasurementUnit,

                                NewReferenceMin =
                                    result.ReferenceMin,

                                NewReferenceMax =
                                    result.ReferenceMax,

                                NewResultFlag =
                                    result.ResultFlag,

                                NewIsAbnormal =
                                    result.IsAbnormal,

                                NewResultNotes =
                                    result.Notes
                            });
                    }

                    result.VerifiedByTechnicianId = null;
                    result.VerifiedDateTime = null;
                    result.VerificationNotes = null;

                    result.RequiresReview = false;
                    result.ReviewNotes = null;
                }

                // ==========================================
                // 8. SUBMIT TEST FOR VERIFICATION
                // ==========================================

                test.Status = "Completed";
                test.CompletionDateTime = DateTime.Now;

                // ==========================================
                // 9. UPDATE PARENT REQUEST STATUS
                // ==========================================

                // Keep the parent request from being marked
                // fully completed while other tests are still
                // awaiting completion.

                bool hasOutstandingTests =
                    await _context.TestRequestTestTypes
                        .AnyAsync(x =>
                            x.TestRequestId == test.TestRequestId &&
                            x.TestRequestTestTypeId != id &&
                            x.Status != "Completed" &&
                            x.Status != "Verified");

                if (!hasOutstandingTests &&
                    test.TestRequest != null)
                {
                    test.TestRequest.Status =
                        TestRequestStatus.Completed.ToString();
                }

                // ==========================================
                // 10. SAVE CHANGES
                // ==========================================

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    isResubmission
                        ? "Corrected test results have been " +
                          "resubmitted for independent verification."
                        : "All required measurements have been " +
                          "captured. The test has been submitted " +
                          "for independent verification.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "An error occurred while submitting " +
                    "the test for verification.";

                return RedirectToAction(
                    nameof(CaptureResults),
                    new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTest(int id, string? verificationNotes)
        {
            var verifierId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            verificationNotes = verificationNotes?.Trim();

            if ((verificationNotes?.Length ?? 0) > 2000)
            {
                TempData["ErrorMessage"] =
                    "Verification notes cannot exceed 2000 characters.";

                return RedirectToAction(nameof(Details), new { id });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == id);

                if (test == null)
                    return NotFound();

                if (test.Status != "Completed")
                {
                    TempData["ErrorMessage"] =
                        "Only completed tests can be verified.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                if (test.TechnicianId == verifierId)
                {
                    TempData["ErrorMessage"] =
                        "You cannot verify a test that you performed.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                // Authorization against technician-test-type assignments
                // must also be enforced here. See Step 2 below.

                var results = await _context.TestResults
                    .Where(x => x.TestRequestTestTypeId == id)
                    .ToListAsync();

                if (results.Count == 0)
                {
                    TempData["ErrorMessage"] =
                        "This test has no captured results.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                if (results.Any(x =>
                    x.TechnicianId == verifierId ||
                    string.IsNullOrWhiteSpace(x.ResultValue) ||
                    !x.TestTypeMeasurementId.HasValue))
                {
                    TempData["ErrorMessage"] =
                        "Results are incomplete or cannot be independently verified.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                var now = DateTime.UtcNow;

                foreach (var result in results)
                {
                    result.VerifiedByTechnicianId = verifierId;
                    result.VerifiedDateTime = now;
                    result.VerificationNotes = verificationNotes;

                    result.RequiresReview = false;
                    result.ReviewNotes = null;

                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResult = result,
                            ActionByTechnicianId = verifierId,
                            Action = "Verified",
                            Notes = verificationNotes ??
                                "Measurement independently verified.",
                            ActionDateTime = now,

                            PreviousResultValue = result.ResultValue,
                            NewResultValue = result.ResultValue,

                            PreviousMeasurementUnit = result.MeasurementUnit,
                            NewMeasurementUnit = result.MeasurementUnit,

                            PreviousReferenceMin = result.ReferenceMin,
                            NewReferenceMin = result.ReferenceMin,

                            PreviousReferenceMax = result.ReferenceMax,
                            NewReferenceMax = result.ReferenceMax,

                            PreviousIsAbnormal = result.IsAbnormal,
                            NewIsAbnormal = result.IsAbnormal,

                            PreviousResultFlag = result.ResultFlag,
                            NewResultFlag = result.ResultFlag,

                            PreviousResultNotes = result.Notes,
                            NewResultNotes = result.Notes
                        });
                }

                test.Status = "Verified";

                // The parent request can be marked Completed only
                // when every requested test type is Verified.
                bool hasUnverifiedTests =
                    await _context.TestRequestTestTypes
                        .AnyAsync(x =>
                            x.TestRequestId == test.TestRequestId &&
                            x.TestRequestTestTypeId != id &&
                            x.Status != "Verified");

                if (!hasUnverifiedTests && test.TestRequest != null)
                {
                    test.TestRequest.Status =
                        TestRequestStatus.Completed.ToString();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Test results verified successfully.";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "An error occurred while verifying the results.";

                return RedirectToAction(nameof(Details), new { id });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectTest(int id, string reviewNotes)
        {
            var verifierId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            reviewNotes = reviewNotes?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(reviewNotes) ||
                reviewNotes.Length > 2000)
            {
                TempData["ErrorMessage"] =
                    "A rejection reason is required and must not " +
                    "exceed 2000 characters.";

                return RedirectToAction(nameof(Details), new { id });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == id);

                if (test == null)
                    return NotFound();

                if (test.Status != "Completed")
                {
                    TempData["ErrorMessage"] =
                        "Only completed tests can be rejected.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                if (test.TechnicianId == verifierId)
                {
                    TempData["ErrorMessage"] =
                        "You cannot review a test that you performed.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                // Authorization against technician-test-type assignments
                // must also be enforced here. See Step 2 below.

                var results = await _context.TestResults
                    .Where(x => x.TestRequestTestTypeId == id)
                    .ToListAsync();

                if (results.Count == 0)
                {
                    TempData["ErrorMessage"] =
                        "No results are available for review.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                if (results.Any(x => x.TechnicianId == verifierId))
                {
                    TempData["ErrorMessage"] =
                        "You cannot review measurements you captured.";

                    return RedirectToAction(nameof(Details), new { id });
                }

                var now = DateTime.UtcNow;

                foreach (var result in results)
                {
                    result.RequiresReview = true;
                    result.ReviewNotes = reviewNotes;

                    result.VerifiedByTechnicianId = null;
                    result.VerifiedDateTime = null;
                    result.VerificationNotes = null;

                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResult = result,
                            ActionByTechnicianId = verifierId,
                            Action = "Rejected",
                            Notes = reviewNotes,
                            ActionDateTime = now,

                            PreviousResultValue = result.ResultValue,
                            NewResultValue = result.ResultValue,

                            PreviousMeasurementUnit = result.MeasurementUnit,
                            NewMeasurementUnit = result.MeasurementUnit,

                            PreviousReferenceMin = result.ReferenceMin,
                            NewReferenceMin = result.ReferenceMin,

                            PreviousReferenceMax = result.ReferenceMax,
                            NewReferenceMax = result.ReferenceMax,

                            PreviousIsAbnormal = result.IsAbnormal,
                            NewIsAbnormal = result.IsAbnormal,

                            PreviousResultFlag = result.ResultFlag,
                            NewResultFlag = result.ResultFlag,

                            PreviousResultNotes = result.Notes,
                            NewResultNotes = result.Notes
                        });
                }

                test.Status = "To Be Reviewed";
                test.CompletionDateTime = null;

                // A rejected test must not leave its parent
                // request in a completed/releasable state.
                if (test.TestRequest != null &&
                    test.TestRequest.Status ==
                        TestRequestStatus.Completed.ToString())
                {
                    test.TestRequest.Status = "In Progress";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Test rejected and returned for correction.";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "An error occurred while rejecting the test.";

                return RedirectToAction(nameof(Details), new { id });
            }
        }

    }
}
