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
    // Verification workflow: In Progress -> Completed -> Verified / To Be Reviewed
    // Returned tests: To Be Reviewed -> corrected -> Completed -> independent verification.
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
            // Include active and revoked assignments.
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

            // STEP 3: Keep one display row per assigned test type
            var assignments = assignmentRecords
                .GroupBy(x => x.TestTypeId)
                .Select(g => g
                    .OrderByDescending(x => x.IsActive)
                    .First())
                .OrderBy(x => x.Name)
                .ToList();

            // STEP 4: Retrieve selected tests
            // Include the ApplicationUser patient and doctor.
            var selectedTests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Doctor)
                .Where(x => x.TechnicianId == technicianId)
                .OrderByDescending(x => x.StartDateTime)
                .ThenBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            // STEP 5: Map selected tests and deadlines
            var deadlineRows = selectedTests.Select(x =>
            {
                var request = x.TestRequest;

                // Patient is an ApplicationUser
                var patient = request?.Patient;

                var patientName =
                    !string.IsNullOrWhiteSpace(patient?.FullName)
                        ? patient.FullName
                        : "Patient not found";

                return new TechnicianDeadlineRowViewModel
                {
                    RequestReference =
                        request?.RequestNumber
                        ?? x.TestRequestId.ToString(),

                    PatientName = patientName,

                    TestTypeName =
                        x.TestType?.Name
                        ?? "Unknown Test",

                    Status = x.Status,

                    // StartDateTime is stored in UTC
                    StartDateTimeUtc = x.StartDateTime,

                    TurnaroundHours =
                        x.TestType == null
                            ? null
                            : (double?)x.TestType.TurnAroundTimeHours
                };
            }).ToList();

            // STEP 6: Pass selected tests to the unified table
            ViewBag.SelectedTests = deadlineRows;

            // STEP 7: Return the existing single-table view
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
                    (x.Status == "Submitted" || x.Status == "SamplesReceived") &&
                    (x.TechnicianId == null || x.TechnicianId == "") &&
                    x.TestRequest != null &&
                    !x.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-") &&
                    x.TestType != null &&
                    (x.TestRequest.Status == "Submitted" ||
                     x.TestRequest.Status == "SamplesReceived" ||
                     x.TestRequest.Status == "InProgress" ||
                     x.TestRequest.Status == "In Progress") &&
                    (x.IsDeleted || _context.Samples.Any(sample =>
                        sample.TestRequestId == x.TestRequestId &&
                        sample.SampleTypeId == x.TestType.SampleTypeId &&
                        sample.IsReceived && !sample.IsDeleted)))
                // Highest clinical priority first: Stat, Urgent, Routine.
                .OrderByDescending(x => x.TestRequest!.Urgency)
                .ThenBy(x => x.TestRequestId)
                .ThenBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            // STEP 4: Collect patient Identity IDs and doctor Identity IDs
            var patientIds = availableTests
                .Where(x => x.TestRequest != null)
                .Select(x => x.TestRequest!.PatientId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var doctorIds = availableTests
                .Where(x => x.TestRequest != null)
                .Select(x => x.TestRequest!.DoctorId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            // STEP 5: Retrieve patient Identity accounts
            var patientUsers = await _userManager.Users
                .AsNoTracking()
                .Where(x => patientIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            // STEP 6: Retrieve patient profiles using their Identity UserId
            var patientProfiles = await _context.Patients
                .AsNoTracking()
                .Where(x => patientIds.Contains(x.UserId))
                .ToDictionaryAsync(x => x.UserId);

            // STEP 7: Retrieve doctor Identity accounts
            var doctorUsers = await _userManager.Users
                .AsNoTracking()
                .Where(x => doctorIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            // STEP 8: Map results to the view model
            var model = availableTests.Select(x =>
            {
                var request = x.TestRequest;

                // Match the patient's Identity account
                var patientUser = request != null &&
                                  patientUsers.TryGetValue(
                                      request.PatientId,
                                      out var matchedPatientUser)
                    ? matchedPatientUser
                    : null;

                // Match the patient profile
                var patientProfile = request != null &&
                                     patientProfiles.TryGetValue(
                                         request.PatientId,
                                         out var matchedPatientProfile)
                    ? matchedPatientProfile
                    : null;

                // Match the doctor's Identity account
                var doctor = request != null &&
                             doctorUsers.TryGetValue(
                                 request.DoctorId,
                                 out var matchedDoctor)
                    ? matchedDoctor
                    : null;

                return new SelectPatientTestViewModel
                {
                    RequestedTestId = x.TestRequestTestTypeId,

                    RequestReference =
                        request?.RequestNumber
                        ?? x.TestRequestId.ToString(),

                    // Prefer the patient profile's name.
                    // Fall back to the Identity account's name.
                    PatientName =
                        !string.IsNullOrWhiteSpace(patientProfile?.FullName)
                            ? patientProfile.FullName
                            : patientUser?.FullName ?? "Patient not found",

                    PatientIdNumber =
                        patientProfile?.IDNumber
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

                    TestStatus = x.Status,
                    IsDeactivated = x.IsDeleted,
                    DeactivationReason = x.DeactivationReason
                };
            }).ToList();

            // STEP 9: Return the Select view
            return View("Select", model);
        }



        // Edit clinical notes on an eligible, unclaimed patient request.
        // ClinicalNotes belongs to the parent request, so changes affect all tests on it.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSelectedTestDetails(int testRequestTestTypeId, string? clinicalNotes)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Please sign in again." });

            clinicalNotes = clinicalNotes?.Trim() ?? string.Empty;
            if (testRequestTestTypeId <= 0 || clinicalNotes.Length > 2000)
                return BadRequest(new { success = false, message = "Clinical notes must be at most 2000 characters." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == testRequestTestTypeId);

                if (test == null) return NotFound(new { success = false, message = "Test not found." });
                if (test.IsDeleted || !string.IsNullOrEmpty(test.TechnicianId) ||
                    (test.Status != "Submitted" && test.Status != "SamplesReceived") ||
                    test.TestRequest == null || test.TestType == null ||
                    test.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                    return Conflict(new { success = false, message = "This test can no longer be edited." });

                bool authorised = await _context.TechnicianTestTypes.AnyAsync(a =>
                    a.TechnicianId == technicianId && a.TestTypeId == test.TestTypeId && a.IsActive);
                if (!authorised)
                    return StatusCode(403, new { success = false, message = "You are not assigned to this test type." });

                test.TestRequest.ClinicalNotes = clinicalNotes;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Clinical notes updated successfully." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Unable to update the test details." });
            }
        }

        // Soft de-activate a requested patient test, preserving it for audit.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateRequestedTest(int testRequestTestTypeId, string? reason)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Session expired. Please sign in again." });

            reason = reason?.Trim();
            if (testRequestTestTypeId <= 0 || string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
                return BadRequest(new { success = false, message = "A reason between 1 and 500 characters is required." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .Include(x => x.TestType)
                    .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == testRequestTestTypeId);

                if (test == null)
                    return NotFound(new { success = false, message = "Requested test was not found." });

                if (test.IsDeleted)
                    return Conflict(new { success = false, message = "This test is already de-activated." });

                if (!string.IsNullOrWhiteSpace(test.TechnicianId) ||
                    (test.Status != "Submitted" && test.Status != "SamplesReceived"))
                    return Conflict(new { success = false, message = "Only unclaimed tests may be de-activated." });

                if (test.TestRequest == null || test.TestType == null ||
                    test.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                    return Conflict(new { success = false, message = "This test is not eligible for de-activation." });

                var assigned = await _context.TechnicianTestTypes.AnyAsync(x =>
                    x.TechnicianId == technicianId && x.TestTypeId == test.TestTypeId && x.IsActive);
                if (!assigned)
                    return StatusCode(403, new { success = false, message = "You are not assigned to this test type." });

                var now = DateTime.UtcNow;
                test.IsDeleted = true;
                test.DeactivationReason = reason;
                test.DeactivatedByTechnicianId = technicianId;
                test.DeactivatedAt = now;

                _context.Set<RequestedTestDeactivationAuditModel>().Add(new RequestedTestDeactivationAuditModel
                {
                    TestRequestTestTypeId = test.TestRequestTestTypeId,
                    TechnicianId = technicianId,
                    Reason = reason,
                    DeactivatedAt = now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Test de-activated successfully. Audit recorded." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Unable to de-activate the test." });
            }
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

                if (requestTest.IsDeleted)
                {
                    TempData["ErrorMessage"] = "This test has been de-activated and cannot be selected.";
                    return RedirectToAction(nameof(Select));
                }

                // Do not allow demo requests to be selected via a direct POST.
                if (requestTest.TestRequest == null ||
                    requestTest.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                {
                    TempData["ErrorMessage"] = "This request is not available for technician selection.";
                    return RedirectToAction(nameof(Select));
                }

                // -----------------------------------------------------
                // 2. Ensure the sample has been received and test is available
                // -----------------------------------------------------
                if (requestTest.Status != "Submitted" &&
                    requestTest.Status != "SamplesReceived")
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

                // A request may still be Submitted when only some of its
                // required sample types have arrived. Check the actual sample
                // for this test instead of relying solely on request status.
                if (request.Status != "Submitted" &&
                    request.Status != samplesReceived &&
                    request.Status != inProgress &&
                    request.Status != "In Progress")
                {
                    TempData["ErrorMessage"] =
                        "This request is not in a selectable workflow state.";
                    return RedirectToAction(nameof(Select));
                }

                if (requestTest.TestType == null ||
                    !await _context.Samples.AnyAsync(sample =>
                        sample.TestRequestId == requestTest.TestRequestId &&
                        sample.SampleTypeId == requestTest.TestType.SampleTypeId &&
                        sample.IsReceived && !sample.IsDeleted))
                {
                    TempData["ErrorMessage"] =
                        "The required sample type has not been received yet.";
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
                if (request.Status == samplesReceived ||
                    request.Status == "Submitted")
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





        // Update request-level clinical notes for a test currently assigned to this technician.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMyTestDetails(int testRequestTestTypeId, string? clinicalNotes)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Please sign in again." });

            clinicalNotes = clinicalNotes?.Trim() ?? string.Empty;
            if (testRequestTestTypeId <= 0 || clinicalNotes.Length > 2000)
                return BadRequest(new { success = false, message = "Invalid test or notes exceed 2000 characters." });

            var test = await _context.TestRequestTestTypes
                .Include(x => x.TestRequest)
                .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == testRequestTestTypeId
                    && x.TechnicianId == technicianId && !x.IsDeleted && x.Status == "In Progress");
            if (test?.TestRequest == null)
                return NotFound(new { success = false, message = "Editable assigned test was not found." });
            if (test.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                return BadRequest(new { success = false, message = "Demo tests cannot be edited." });

            // ClinicalNotes is shared by the whole request; retain a separate audit trail in production.
            test.TestRequest.ClinicalNotes = clinicalNotes;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Test notes updated." });
        }

        // Soft-deactivate a test already claimed by the current technician.
        // Does not reverse consumed stock or delete the patient request.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateMyTest(int testRequestTestTypeId, string? reason)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Please sign in again." });

            reason = reason?.Trim();
            if (testRequestTestTypeId <= 0 || string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
                return BadRequest(new { success = false, message = "A reason between 1 and 500 characters is required." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == testRequestTestTypeId);

                if (test == null || test.TechnicianId != technicianId)
                    return NotFound(new { success = false, message = "Assigned test not found." });

                if (test.IsDeleted)
                    return Conflict(new { success = false, message = "This test is already de-activated." });

                if (test.Status != "In Progress")
                    return Conflict(new { success = false, message = "Only in-progress tests may be de-activated. Completed or reviewed tests cannot be de-activated." });

                if (test.TestRequest == null || test.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                    return BadRequest(new { success = false, message = "This test is not eligible for de-activation." });

                var now = DateTime.UtcNow;
                test.IsDeleted = true;
                test.DeactivationReason = reason;
                test.DeactivatedByTechnicianId = technicianId;
                test.DeactivatedAt = now;

                _context.Set<RequestedTestDeactivationAuditModel>().Add(new RequestedTestDeactivationAuditModel
                {
                    TestRequestTestTypeId = test.TestRequestTestTypeId,
                    TechnicianId = technicianId,
                    Reason = reason,
                    DeactivatedAt = now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Assigned test de-activated. The audit record has been saved." });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict(new { success = false, message = "This test was changed by another user. Refresh and try again." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Unable to de-activate this test." });
            }
        }

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
                            x.TestTypeId == test.TestTypeId &&
                            x.IsActive);
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



        // test to be reviewed




        // Returned test soft-deactivation: assigned technician only; reason and audit required.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateReviewTest(int testRequestTestTypeId, string? reason)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Please sign in again." });

            reason = reason?.Trim();
            if (testRequestTestTypeId <= 0 || string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
                return BadRequest(new { success = false, message = "Enter a reason of 1–500 characters." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(t => t.TestRequest)
                    .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == testRequestTestTypeId);

                if (test == null || test.TechnicianId != technicianId)
                    return NotFound(new { success = false, message = "Your returned test was not found." });
                if (test.IsDeleted)
                    return Conflict(new { success = false, message = "Test is already de-activated." });
                if (test.Status != "To Be Reviewed")
                    return Conflict(new { success = false, message = "Only tests returned to you for correction may be de-activated here." });
                if (test.TestRequest == null || test.TestRequest.RequestNumber.StartsWith("NMB-REQ-DEMO-"))
                    return BadRequest(new { success = false, message = "This test is not eligible for de-activation." });

                var now = DateTime.UtcNow;
                test.IsDeleted = true;
                test.DeactivationReason = reason;
                test.DeactivatedByTechnicianId = technicianId;
                test.DeactivatedAt = now;
                _context.Set<RequestedTestDeactivationAuditModel>().Add(new RequestedTestDeactivationAuditModel
                {
                    TestRequestTestTypeId = test.TestRequestTestTypeId,
                    TechnicianId = technicianId,
                    Reason = reason,
                    DeactivatedAt = now
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Test de-activated and audit recorded." });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict(new { success = false, message = "Test changed while processing. Refresh and try again." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Unable to de-activate the test." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Review()
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            // Keep all filtering in SQL; avoid local-array Contains expressions,
            // which caused EF parameter extraction to fail in this environment.
            var authorisedTypes = _context.TechnicianTestTypes
                .AsNoTracking()
                .Where(a => a.TechnicianId == technicianId && a.IsActive)
                .Select(a => a.TestTypeId);

            var eligibleTests = _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(t =>
                    (t.Status == "To Be Reviewed" && t.TechnicianId == technicianId)
                    ||
                    (t.Status == "Completed" &&
                     t.TechnicianId != null &&
                     t.TechnicianId != technicianId &&
                     authorisedTypes.Contains(t.TestTypeId)));

            // Preserve the entities and navigation properties used by Review.cshtml.
            var tests = await eligibleTests
                .Include(t => t.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Include(t => t.TestRequest)
                    .ThenInclude(r => r!.Doctor)
                .Include(t => t.TestType)
                    .ThenInclude(tt => tt!.TestCategory)
                .OrderBy(t => t.Status == "To Be Reviewed" ? 0 : 1)
                .ThenByDescending(t => t.CompletionDateTime)
                .ToListAsync();

            // SQL joins replace local testIds.Contains(...) and resultIds.Contains(...).
            // Distinct removes duplicates without changing result entities.
            var savedResults = await (
                from result in _context.TestResults.AsNoTracking()
                join test in eligibleTests
                    on result.TestRequestTestTypeId equals test.TestRequestTestTypeId
                select result
            ).Distinct().ToListAsync();

            var histories = await (
                from history in _context.TestResultVerificationHistories.AsNoTracking()
                join result in _context.TestResults.AsNoTracking()
                    on history.TestResultId equals result.TestResultId
                join test in eligibleTests
                    on result.TestRequestTestTypeId equals test.TestRequestTestTypeId
                orderby history.ActionDateTime descending
                select history
            ).Distinct().ToListAsync();

            histories = histories
                .OrderByDescending(h => h.ActionDateTime)
                .ThenByDescending(h => h.TestResultVerificationHistoryId)
                .ToList();

            ViewData["SavedResults"] = savedResults;
            ViewData["ReviewHistories"] = histories;
            ViewData["LatestReviewHistory"] = histories
                .Where(h => h.Action == "ReturnedForReview")
                .GroupBy(h => h.TestResultId)
                .ToDictionary(g => g.Key, g => g.First());
            ViewData["ReviewCount"] = tests.Count(t => t.Status == "To Be Reviewed");
            ViewData["CompletedCount"] = tests.Count(t => t.Status == "Completed");
            ViewData["TotalCount"] = tests.Count;

            return View(tests);
        }



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


        //over due tests

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
            // STEP 1: Identify the logged-in technician
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            // STEP 2: Retrieve the test assigned to this technician
            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.MeasurementUnitDetails)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Doctor)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id &&
                    x.TechnicianId == technicianId);

            if (test == null)
                return NotFound();

            // STEP 3: Calculate the expected completion in HOURS
            // StartDateTime is assumed to be stored in UTC.
            DateTime? expectedCompletionUtc = null;
            bool isOverdue = false;

            if (test.StartDateTime.HasValue && test.TestType != null)
            {
                expectedCompletionUtc =
                    DateTime.SpecifyKind(
                        test.StartDateTime.Value,
                        DateTimeKind.Utc)
                    .AddHours(test.TestType.TurnAroundTimeHours);

                isOverdue =
                    DateTime.UtcNow > expectedCompletionUtc.Value &&
                    !string.Equals(
                        test.Status,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(
                        test.Status,
                        "Verified",
                        StringComparison.OrdinalIgnoreCase);
            }

            // STEP 4: Supply deadline information to CaptureResults.cshtml
            ViewData["ExpectedCompletion"] = expectedCompletionUtc;
            ViewData["IsOverdue"] = isOverdue;

            // STEP 5: Retrieve existing result(s)
            var existingResults = await _context.TestResults
                .AsNoTracking()
                .Where(r =>
                    r.TestRequestTestTypeId == test.TestRequestTestTypeId)
                .OrderBy(r => r.TestResultId)
                .ToListAsync();

            ViewData["ExistingResults"] = existingResults;

            // STEP 6: Supply clinical notes
            ViewData["ClinicalNotes"] = test.TestRequest?.ClinicalNotes;

            return View(test);
        }


        // ============================================================
        // GET - TEST RESULTS PAGE
        // ============================================================
        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> TestResults()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var technicianId = currentUser.Id;

            var results = await _context.TestResults
                .AsNoTracking()

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestType)

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(request => request!.Patient)

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(request => request!.Doctor)

                .Where(r =>
                    r.TestRequestTestType != null &&
                    r.TestRequestTestType.TechnicianId == technicianId)

                .OrderByDescending(r => r.CapturedDateTime)

                .ToListAsync();

            return View(results);
        }


        // ============================================================
        // GET - LOAD VIEW RESULTS MODAL
        // ============================================================

        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> ResultDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid TestResultId: " + id);

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var result = await _context.TestResults
                .AsNoTracking()
                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestType)
                        .ThenInclude(t => t!.MeasurementUnitDetails)
                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(r => r!.Patient)
                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(r => r!.Doctor)
                .FirstOrDefaultAsync(r => r.TestResultId == id);

            if (result == null)
                return NotFound(
                    $"No test result exists with TestResultId = {id}."
                );

            // Diagnostic: distinguish a missing result from an access mismatch.
            if (result.TechnicianId != currentUser.Id &&
                result.TestRequestTestType?.TechnicianId != currentUser.Id)
            {
                return StatusCode(
                    403,
                    "Result exists, but the logged-in technician " +
                    "does not match the capturing or assigned technician."
                );
            }

            return PartialView("_ViewResultsModal", result);
        }


        // load Edit results
        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> EditResult(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid TestResultId: " + id);

            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var technicianId = currentUser.Id;

            // Find the result by its actual primary key first.
            var result = await _context.TestResults
                .AsNoTracking()

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestType)
                        .ThenInclude(t => t!.MeasurementUnitDetails)

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(request => request!.Patient)

                .Include(r => r.TestRequestTestType)
                    .ThenInclude(t => t!.TestRequest)
                        .ThenInclude(request => request!.Doctor)

                .FirstOrDefaultAsync(r =>
                    r.TestResultId == id);

            if (result == null)
            {
                return NotFound(
                    $"Test result with ID {id} was not found."
                );
            }

            // Only the capturing technician may edit.
            if (result.TechnicianId != technicianId)
            {
                return StatusCode(
                    403,
                    "Only the technician who captured this result can edit it."
                );
            }

            if (result.TestRequestTestType == null)
            {
                return BadRequest(
                    "This result is not linked to an assigned test."
                );
            }

            if (result.TestRequestTestType.TechnicianId != technicianId)
            {
                return StatusCode(
                    403,
                    "This test is not assigned to the current technician."
                );
            }

            // Verified results must remain locked.
            if (result.VerifiedDateTime.HasValue ||
                !string.IsNullOrWhiteSpace(
                    result.VerifiedByTechnicianId))
            {
                return Conflict(
                    "This result has already been verified and cannot be edited."
                );
            }

            var status = result.TestRequestTestType.Status;

            if (status != "In Progress" &&
                status != "To Be Reviewed")
            {
                return Conflict(
                    "Only results in progress or returned for review can be edited."
                );
            }

            return PartialView(
                "_EditResultsModal",
                result
            );
        }


        // Save corrections from the AJAX Edit Laboratory Result modal.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResult(int id, string resultValue, string? notes)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized(new { success = false, message = "Please sign in again." });

            resultValue = resultValue?.Trim() ?? string.Empty;
            notes = notes?.Trim();
            if (id <= 0 || string.IsNullOrWhiteSpace(resultValue) ||
                resultValue.Length > 200 || (notes?.Length ?? 0) > 2000)
                return BadRequest(new { success = false, message = "Enter a result (up to 200 characters) and notes (up to 2000 characters)." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var result = await _context.TestResults
                    .Include(r => r.TestRequestTestType)
                        .ThenInclude(t => t!.TestType)
                    .FirstOrDefaultAsync(r => r.TestResultId == id);

                if (result == null) return NotFound(new { success = false, message = "Result not found." });
                var test = result.TestRequestTestType;
                if (result.TechnicianId != technicianId || test?.TechnicianId != technicianId)
                    return StatusCode(403, new { success = false, message = "Only the capturing technician may edit this result." });
                if (test.IsDeleted || result.VerifiedDateTime.HasValue ||
                    !string.IsNullOrWhiteSpace(result.VerifiedByTechnicianId) ||
                    (test.Status != "In Progress" && test.Status != "To Be Reviewed"))
                    return Conflict(new { success = false, message = "This result is locked or not eligible for correction." });

                var previousValue = result.ResultValue;
                var previousNotes = result.Notes;
                if (previousValue == resultValue && previousNotes == notes)
                    return Json(new { success = true, message = "No changes were necessary." });

                // Recalculate the clinical flag when the corrected value is numeric.
                var min = result.ReferenceMin;
                var max = result.ReferenceMax;
                if (min == 0m && max == 0m) { min = null; max = null; }
                bool abnormal = false;
                string flag = "Not Evaluated";
                if (decimal.TryParse(resultValue, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out var numericValue))
                {
                    if (min.HasValue && numericValue < min.Value) { abnormal = true; flag = "Low"; }
                    else if (max.HasValue && numericValue > max.Value) { abnormal = true; flag = "High"; }
                    else if (min.HasValue || max.HasValue) flag = "Normal";
                }

                var oldFlag = result.ResultFlag;
                var oldAbnormal = result.IsAbnormal;
                result.ResultValue = resultValue;
                result.Notes = notes;
                result.ResultFlag = flag;
                result.IsAbnormal = abnormal;
                result.CapturedDateTime = DateTime.UtcNow;
                RecordResultHistory(result, technicianId, "Corrected",
                    "Result corrected using Edit Laboratory Result modal.", previousValue, resultValue);
                // Preserve the detailed audit fields used by the capture workflow.
                var history = _context.ChangeTracker.Entries<TestResultVerificationHistoryModel>()
                    .Last().Entity;
                history.PreviousResultNotes = previousNotes;
                history.NewResultNotes = notes;
                history.PreviousResultFlag = oldFlag;
                history.NewResultFlag = flag;
                history.PreviousIsAbnormal = oldAbnormal;
                history.NewIsAbnormal = abnormal;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Laboratory result updated and audit history recorded." });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict(new { success = false, message = "Result changed by another user. Reload and try again." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Unable to save the result. Check the server logs." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public Task<IActionResult> EditResultModal(int id)
        {
            return EditResult(id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureResults(
            int testRequestTestTypeId,
            string resultValue,
            string? notes)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
                return Challenge();

            resultValue = resultValue?.Trim() ?? string.Empty;
            notes = notes?.Trim();

            if (string.IsNullOrWhiteSpace(resultValue) ||
                resultValue.Length > 200 ||
                (notes?.Length ?? 0) > 2000)
            {
                TempData["ErrorMessage"] =
                    "Enter a result (maximum 200 characters) and valid notes (maximum 2000 characters).";
                return RedirectToAction(nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

            try
            {
                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.MeasurementUnitDetails)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == testRequestTestTypeId &&
                        x.TechnicianId == technicianId);

                if (test == null)
                    return NotFound();

                if (test.Status != "In Progress" &&
                    test.Status != "To Be Reviewed")
                {
                    TempData["ErrorMessage"] = "This test cannot accept results.";
                    return RedirectToAction(nameof(MyTests));
                }

                var testType = test.TestType;
                if (testType == null)
                    return NotFound();

                string resultName = testType.Name;
                string measurementUnit = testType.MeasurementUnitDetails?.Unit ?? "";

                if (string.IsNullOrWhiteSpace(resultName) ||
                    resultName.Length > 200 ||
                    string.IsNullOrWhiteSpace(measurementUnit) ||
                    measurementUnit.Length > 100)
                {
                    TempData["ErrorMessage"] =
                        "The test catalogue has an invalid name or measurement unit.";
                    return RedirectToAction(nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                decimal? referenceMin = testType.NormalMin;
                decimal? referenceMax = testType.NormalMax;

                // Existing seed data uses 0/0 as a placeholder for missing ranges.
                // Do not classify numeric results against this placeholder.
                if (referenceMin == 0m && referenceMax == 0m)
                {
                    referenceMin = null;
                    referenceMax = null;
                }

                if (referenceMin.HasValue && referenceMax.HasValue &&
                    referenceMin.Value > referenceMax.Value)
                {
                    TempData["ErrorMessage"] = "The configured reference range is invalid.";
                    return RedirectToAction(nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                bool isAbnormal = false;
                string resultFlag = "Not Evaluated";

                if (decimal.TryParse(resultValue,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out decimal numericValue))
                {
                    if (referenceMin.HasValue && numericValue < referenceMin.Value)
                    {
                        isAbnormal = true;
                        resultFlag = "Low";
                    }
                    else if (referenceMax.HasValue && numericValue > referenceMax.Value)
                    {
                        isAbnormal = true;
                        resultFlag = "High";
                    }
                    else if (referenceMin.HasValue || referenceMax.HasValue)
                    {
                        resultFlag = "Normal";
                    }
                }

                // One result per requested test type. Never silently overwrite
                // a legacy multi-measurement result set.
                var existingResults = await _context.TestResults
                    .Where(r => r.TestRequestTestTypeId == testRequestTestTypeId)
                    .ToListAsync();

                if (existingResults.Count > 1)
                {
                    TempData["ErrorMessage"] =
                        "Multiple legacy results exist for this test. " +
                        "Please reconcile them before using single-result capture.";
                    return RedirectToAction(nameof(CaptureResults),
                        new { id = testRequestTestTypeId });
                }

                var result = existingResults.SingleOrDefault();
                if (result != null && result.TechnicianId != technicianId)
                    return Forbid();

                bool isNewResult = result == null;
                string? oldValue = result?.ResultValue;
                string? oldName = result?.ResultName;
                string? oldUnit = result?.MeasurementUnit;
                decimal? oldMin = result?.ReferenceMin;
                decimal? oldMax = result?.ReferenceMax;
                string? oldFlag = result?.ResultFlag;
                bool? oldAbnormal = result?.IsAbnormal;
                string? oldNotes = result?.Notes;
                int? oldMeasurementId = result?.TestTypeMeasurementId;

                if (result == null)
                {
                    result = new TestResultModel
                    {
                        TestRequestTestTypeId = testRequestTestTypeId,
                        TechnicianId = technicianId,
                        TestTypeMeasurementId = null
                    };
                    _context.TestResults.Add(result);
                }

                bool changed = isNewResult ||
                    oldValue != resultValue ||
                    oldName != resultName ||
                    oldUnit != measurementUnit ||
                    oldMin != referenceMin ||
                    oldMax != referenceMax ||
                    oldFlag != resultFlag ||
                    oldAbnormal != isAbnormal ||
                    oldNotes != notes ||
                    oldMeasurementId.HasValue;

                result.TestTypeMeasurementId = null;
                result.ResultName = resultName;
                result.ResultValue = resultValue;
                result.MeasurementUnit = measurementUnit;
                result.ReferenceMin = referenceMin;
                result.ReferenceMax = referenceMax;
                result.IsAbnormal = isAbnormal;
                result.ResultFlag = resultFlag;
                result.Notes = notes;
                // Corrections awaiting resubmission must never retain approval metadata.
                if (test.Status == "To Be Reviewed")
                {
                    result.VerifiedByTechnicianId = null;
                    result.VerifiedDateTime = null;
                    result.VerificationNotes = null;
                }

                if (changed)
                {
                    var now = DateTime.UtcNow;
                    result.CapturedDateTime = now;

                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResult = result,
                            ActionByTechnicianId = technicianId,
                            Action = isNewResult ? "Submitted" : "Corrected",
                            Notes = isNewResult
                                ? "Test result captured."
                                : "Test result updated.",
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

                // Saving results does not complete the test.
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = changed
                    ? "Test result saved successfully. You may now finish the test."
                    : "No changes were detected.";

                return RedirectToAction(nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] =
                    "An error occurred while saving the test result.";
                return RedirectToAction(nameof(CaptureResults),
                    new { id = testRequestTestTypeId });
            }
        }


        // Replace BOTH existing FinishTest actions in Areas/Technician/Controllers/TestsController.cs.
        // Requires Microsoft.EntityFrameworkCore and System.Data (or fully qualified IsolationLevel).
        // Replace the existing FinishTest GET and POST actions with these methods.
        // The POST uses a different C# method name to avoid CS0111.

        [HttpGet]
        public async Task<IActionResult> CompleteTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestRequest)
                .Include(x => x.TestType)
                    .ThenInclude(t => t!.MeasurementUnitDetails)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id &&
                    x.TechnicianId == technicianId);

            if (test == null)
                return NotFound();

            if (test.Status != "In Progress" && test.Status != "To Be Reviewed")
            {
                TempData["ErrorMessage"] = "This test is not available for completion.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Option A: exactly one result per requested test.
            // Read ALL results for the test, not just the current technician's,
            // so legacy duplicate records cannot be overlooked.
            var results = await _context.TestResults
                .AsNoTracking()
                .Where(r => r.TestRequestTestTypeId == id)
                .ToListAsync();

            var hasValidResult = results.Count == 1 &&
                                 results[0].TechnicianId == technicianId &&
                                 !string.IsNullOrWhiteSpace(results[0].ResultValue);

            ViewData["HasValidResult"] = hasValidResult;
            ViewData["ResultCount"] = results.Count;
            ViewData["SavedResult"] = hasValidResult ? results[0] : null;

            return View("CompleteTest", test);
        }

        // Clinical information for the Complete Test confirmation modal.
        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> CompleteTestClinicalContext(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId)) return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(t => t.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id &&
                                          t.TechnicianId == technicianId);

            if (test == null) return NotFound();
            if (test.Status != "In Progress" && test.Status != "To Be Reviewed")
                return Conflict(new { message = "This test is not available for completion." });

            var patientUserId = test.TestRequest?.Patient?.Id;
            NMB_HLabSys.Areas.Patient.Models.PatientModel? record = null;

            if (!string.IsNullOrWhiteSpace(patientUserId))
            {
                record = await _context.Set<NMB_HLabSys.Areas.Patient.Models.PatientModel>()
                    .AsNoTracking()
                    .Include(p => p.PatientConditions)
                    .Include(p => p.PatientAllergies)
                    .Include(p => p.PatientMedications)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.UserId == patientUserId);
            }

            // Select only clinically relevant, human-readable properties.
            // Adjust these property names if your medical-history models use other names.
            static string FormatEntries(System.Collections.IEnumerable? entries)
            {
                if (entries == null) return "Not recorded";
                var lines = new List<string>();
                string[] allowed = { "Name", "ConditionName", "AllergyName", "MedicationName",
                                     "Description", "Dosage", "Frequency", "Notes", "Severity" };
                foreach (var entry in entries)
                {
                    if (entry == null) continue;
                    var parts = new List<string>();
                    foreach (var name in allowed)
                    {
                        var property = entry.GetType().GetProperty(name);
                        if (property == null || property.PropertyType != typeof(string)) continue;
                        var value = property.GetValue(entry) as string;
                        if (!string.IsNullOrWhiteSpace(value)) parts.Add(value.Trim());
                    }
                    if (parts.Count > 0) lines.Add(string.Join(" · ", parts.Distinct()));
                }
                return lines.Count == 0 ? "Not recorded" : string.Join("\n", lines);
            }

            return Json(new
            {
                medicalConditions = FormatEntries(record?.PatientConditions),
                allergies = FormatEntries(record?.PatientAllergies),
                medications = FormatEntries(record?.PatientMedications),
                clinicalNotes = string.IsNullOrWhiteSpace(test.TestRequest?.ClinicalNotes)
                    ? "Not recorded"
                    : test.TestRequest.ClinicalNotes
            });
        }

        [HttpPost]
        [ActionName("CompleteTest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTestConfirmed(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            await using var transaction = await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

            try
            {
                var test = await _context.TestRequestTestTypes
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == id &&
                        x.TechnicianId == technicianId);

                if (test == null)
                    return NotFound();

                if (test.Status != "In Progress" && test.Status != "To Be Reviewed")
                {
                    TempData["ErrorMessage"] = "This test cannot be completed in its current status.";
                    return RedirectToAction(nameof(MyTests));
                }

                var results = await _context.TestResults
                    .Where(r => r.TestRequestTestTypeId == id)
                    .ToListAsync();

                if (results.Count != 1 ||
                    results[0].TechnicianId != technicianId ||
                    string.IsNullOrWhiteSpace(results[0].ResultValue))
                {
                    TempData["ErrorMessage"] = results.Count > 1
                        ? "Multiple legacy results exist for this test. Reconcile them before completion."
                        : "Save a valid test result before finishing this test.";

                    return RedirectToAction(nameof(CaptureResults), new { id });
                }

                var result = results[0];
                var now = DateTime.UtcNow;

                // If this test was returned for correction, remove the previous
                // verification decision from the current result. Historical events
                // remain intact in TestResultVerificationHistories.
                if (test.Status == "To Be Reviewed")
                {
                    result.VerifiedByTechnicianId = null;
                    result.VerifiedDateTime = null;
                    result.VerificationNotes = null;
                    result.RequiresReview = false;
                    result.ReviewNotes = null;

                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResultId = result.TestResultId,
                            ActionByTechnicianId = technicianId,
                            Action = "ResubmittedForVerification",
                            Notes = "Corrected result resubmitted for independent verification.",
                            ActionDateTime = now
                        });
                }

                // Record first submission as well as resubmission in the full audit trail.
                if (test.Status == "In Progress")
                {
                    _context.TestResultVerificationHistories.Add(
                        new TestResultVerificationHistoryModel
                        {
                            TestResultId = result.TestResultId,
                            ActionByTechnicianId = technicianId,
                            Action = "SubmittedForVerification",
                            Notes = "Test completed and submitted for independent verification.",
                            ActionDateTime = now
                        });
                }

                // 'Completed' means ready for a DIFFERENT authorised technician
                // to verify. It does not mean 'Verified'.
                test.Status = "Completed";
                test.CompletionDateTime = now;

                // Notify other active technicians authorised to verify this test.
                // The technician who completed it cannot verify their own work.
                var verifierIds = await _context.TechnicianTestTypes
                    .AsNoTracking()
                    .Where(a =>
                        a.TestTypeId == test.TestTypeId &&
                        a.IsActive &&
                        a.TechnicianId != technicianId)
                    .Select(a => a.TechnicianId)
                    .Distinct()
                    .ToListAsync();

                foreach (var verifierId in verifierIds)
                {
                    _context.Set<TechnicianNotificationModel>().Add(
                        new TechnicianNotificationModel
                        {
                            TechnicianId = verifierId,
                            Title = "Test Awaiting Verification",
                            Message = $"A completed laboratory test (ID: {test.TestRequestTestTypeId}) is ready for independent verification.",
                            ActionUrl = "/Technician/Tests/VerifyResultsQueue",
                            IsRead = false,
                            CreatedAt = now
                        });
                }

                // IMPORTANT: Do NOT update the parent TestRequest.Status here.
                // Do NOT send the doctor's PDF/email here.
                // The verification workflow must do that only after ALL tests
                // belonging to the request have reached the 'Verified' status.

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Test submitted for independent verification successfully.";
                return RedirectToAction(nameof(MyTests));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] =
                    "An error occurred while submitting the test for verification.";
                return RedirectToAction(nameof(CaptureResults), new { id });
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

            // Only test types actively assigned by the Lab Manager.
            var assignedTestTypeIds = _context.TechnicianTestTypes
                .AsNoTracking()
                .Where(x =>
                    x.TechnicianId == technicianId &&
                    x.IsActive)
                .Select(x => x.TestTypeId);

            // Completed tests belonging to OTHER technicians,
            // with exactly one valid saved result (Option A).
            var tests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Where(x =>
                    x.Status == "Completed" &&
                    x.TechnicianId != null &&
                    x.TechnicianId != technicianId &&
                    assignedTestTypeIds.Contains(x.TestTypeId) &&
                    _context.TestResults.Count(r =>
                        r.TestRequestTestTypeId ==
                            x.TestRequestTestTypeId) == 1 &&
                    _context.TestResults.Any(r =>
                        r.TestRequestTestTypeId ==
                            x.TestRequestTestTypeId &&
                        r.TechnicianId == x.TechnicianId &&
                        r.ResultValue != null &&
                        r.ResultValue.Trim() != ""))
                .OrderBy(x => x.CompletionDateTime)
                .ToListAsync();

            ViewData["PendingCount"] = tests.Count;

            return View("PendingVerification", tests);
        }



        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> Verified()
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            // Only tests this technician verified, or tests originally performed
            // by them, are shown. The history relation records the verifier.
            var tests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .Where(t => t.Status == "Verified" &&
                    (t.TechnicianId == technicianId ||
                     _context.TestResultVerificationHistories.Any(h =>
                         h.TestResult != null &&
                         h.TestResult.TestRequestTestTypeId == t.TestRequestTestTypeId &&
                         h.Action == "Verified" &&
                         h.ActionByTechnicianId == technicianId)))
                .OrderByDescending(t => t.CompletionDateTime)
                .ToListAsync();

            return View("Verified", tests);
        }


        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> VerifyTestModal(int id)
        {
            var verifierId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            if (id <= 0)
                return BadRequest();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestRequest)
                    .ThenInclude(r => r!.Patient)
                .Include(x => x.TestType)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != "Completed")
                return Conflict("Only completed tests can be verified.");

            // The second technician cannot verify their own test.
            if (string.IsNullOrWhiteSpace(test.TechnicianId) ||
                test.TechnicianId == verifierId)
                return Forbid();

            // The Laboratory Manager must have assigned this test type.
            var authorised = await _context.TechnicianTestTypes
                .AsNoTracking()
                .AnyAsync(x =>
                    x.TechnicianId == verifierId &&
                    x.TestTypeId == test.TestTypeId &&
                    x.IsActive);

            if (!authorised)
                return Forbid();

            // Ensure exactly one valid captured result exists.
            var results = await _context.TestResults
                .AsNoTracking()
                .Where(x => x.TestRequestTestTypeId == id)
                .ToListAsync();

            if (results.Count != 1 ||
                results[0].TechnicianId != test.TechnicianId ||
                string.IsNullOrWhiteSpace(results[0].ResultValue))
            {
                return Conflict(
                    "Exactly one valid captured result is required.");
            }

            ViewData["VerificationResult"] = results[0];

            return PartialView("_VerifyTestModal", test);
        }



        // Paste these two actions INSIDE your existing TestsController.
        // Replace the old VerifyResult GET and POST; do not append duplicates.
        // Uses existing project models and DbSets shown in your original actions.
        // Add this GET action inside TestsController. Keep your existing VerifyResultDecision POST unchanged.
        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> VerifyTest(int id)
        {
            var verifierId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(verifierId)) return Challenge();
            if (id <= 0) return BadRequest();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestRequest).ThenInclude(r => r!.Patient)
                .Include(x => x.TestType)
                .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == id);

            if (test == null) return NotFound();
            if (test.Status != "Completed") return Conflict("Only completed tests may be verified.");
            if (string.IsNullOrWhiteSpace(test.TechnicianId) || test.TechnicianId == verifierId)
                return Forbid();

            var authorised = await _context.TechnicianTestTypes.AsNoTracking()
                .AnyAsync(a => a.TechnicianId == verifierId && a.TestTypeId == test.TestTypeId && a.IsActive);
            if (!authorised) return Forbid();

            // Validate ALL result records, so a duplicate/legacy record cannot be overlooked.
            var results = await _context.TestResults.AsNoTracking()
                .Where(r => r.TestRequestTestTypeId == id)
                .ToListAsync();
            if (results.Count != 1 ||
                results[0].TechnicianId != test.TechnicianId ||
                string.IsNullOrWhiteSpace(results[0].ResultValue))
                return Conflict("Exactly one valid result from the original technician is required.");

            ViewData["VerificationResult"] = results[0];
            return PartialView("_VerifyTestModal", test);
        }
        //This is a GET endpoint.Keep your existing CaptureResults POST action.
        [HttpGet]
        [Authorize(Roles = "LabTechnician")]
        public async Task<IActionResult> CorrectTestModal(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id &&
                                          t.TechnicianId == technicianId);

            if (test == null) return NotFound();
            if (test.Status != "To Be Reviewed")
                return Conflict("Only tests returned for review can be corrected.");

            var results = await _context.TestResults
                .AsNoTracking()
                .Where(r => r.TestRequestTestTypeId == id)
                .ToListAsync();

            if (results.Count != 1 || results[0].TechnicianId != technicianId)
                return Conflict("Exactly one result captured by the original technician is required.");

            var result = results[0];
            result.TestRequestTestType = test;
            return PartialView("_CorrectTestResultModal", result);
        }
        // Sidebar GET: /Technician/Tests/VerifyResultsQueue (no test ID).
        // Displays an independent-verification selection page.
        [HttpGet]
        public async Task<IActionResult> VerifyResultsQueue()
        {
            var verifierId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            // Get test types assigned to the currently logged-in technician.
            var assignedTypeIds = _context.TechnicianTestTypes
                .AsNoTracking()
                .Where(a =>
                    a.TechnicianId == verifierId &&
                    a.IsActive)
                .Select(a => a.TestTypeId);

            // Retrieve tests eligible for independent verification.
            var tests = await _context.TestRequestTestTypes
                .AsNoTracking()

                // Load test information.
                .Include(t => t.TestType)

                // Load request and patient information.
                .Include(t => t.TestRequest)
                    .ThenInclude(r => r!.Patient)

                // Load the technician who performed the test.
                // Provides FirstName, LastName and UserName.
                .Include(t => t.Technician)

                .Where(t =>

                    // Only completed tests can be verified.
                    t.Status == "Completed" &&

                    // Test must have an assigned technician.
                    t.TechnicianId != null &&

                    // Independent verification:
                    // The verifier cannot verify their own test.
                    t.TechnicianId != verifierId &&

                    // The verifier must be assigned to this test type.
                    assignedTypeIds.Contains(t.TestTypeId) &&

                    // The test must have exactly one saved result.
                    _context.TestResults.Count(r =>
                        r.TestRequestTestTypeId ==
                        t.TestRequestTestTypeId) == 1 &&

                    // The result must belong to the original technician
                    // and must contain a non-empty value.
                    _context.TestResults.Any(r =>
                        r.TestRequestTestTypeId ==
                            t.TestRequestTestTypeId &&
                        r.TechnicianId == t.TechnicianId &&
                        r.ResultValue != null &&
                        r.ResultValue.Trim() != "")
                )

                // Display oldest completed tests first.
                .OrderBy(t => t.CompletionDateTime)

                .ToListAsync();

            return View("VerifyResultsQueue", tests);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyResult(int id = 0)
        {
            // A sidebar link has no test ID: open the verification queue.
            if (id <= 0)
                return RedirectToAction(nameof(VerifyResultsQueue));

            var verifierId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestRequest).ThenInclude(r => r!.Patient)
                .Include(x => x.TestRequest).ThenInclude(r => r!.Doctor)
                .Include(x => x.TestType).ThenInclude(t => t!.SampleType)
                .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != "Completed")
            {
                TempData["ErrorMessage"] = "Only completed tests can be verified.";
                return RedirectToAction(nameof(PendingVerification));
            }

            // A test must have an original, different technician.
            if (string.IsNullOrWhiteSpace(test.TechnicianId) ||
                test.TechnicianId == verifierId)
                return Forbid();

            // Enforce Laboratory Manager's active test-type assignment.
            var authorised = await _context.TechnicianTestTypes
                .AsNoTracking()
                .AnyAsync(x => x.TechnicianId == verifierId &&
                               x.TestTypeId == test.TestTypeId &&
                               x.IsActive);

            if (!authorised)
                return Forbid();

            var results = await _context.TestResults
                .AsNoTracking()
                .Where(x => x.TestRequestTestTypeId == id &&
                            x.TechnicianId == test.TechnicianId)
                .OrderBy(x => x.ResultName)
                .ToListAsync();

            // Option A: exactly one captured result per requested test type.
            if (results.Count != 1 || string.IsNullOrWhiteSpace(results[0].ResultValue))
            {
                TempData["ErrorMessage"] =
                    "This test must have exactly one saved result before verification. " +
                    "Ask the original technician to resolve missing or legacy results.";
                return RedirectToAction(nameof(PendingVerification));
            }

            ViewData["TestResults"] = results;
            return View("VerifyResult", test);
        }
        // Add inside TestsController, alongside the existing VerificationHistory(int id) action.
        // Add INSIDE Areas/Technician/Controllers/TestsController.cs
        // Keep your existing VerificationHistory(int id) action unchanged.

        [HttpGet]
        public async Task<IActionResult> VerificationHistoryIndex()
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            var histories = await _context.TestResultVerificationHistories
                .AsNoTracking()
                .Include(h => h.ActionByTechnician)
                .Include(h => h.TestResult)
                    .ThenInclude(r => r!.TestRequestTestType)
                        .ThenInclude(t => t!.TestType)
                .Include(h => h.TestResult)
                    .ThenInclude(r => r!.TestRequestTestType)
                        .ThenInclude(t => t!.TestRequest)
                .Where(h =>
                    h.TestResult != null &&
                    h.TestResult.TestRequestTestType != null &&
                    (h.TestResult.TestRequestTestType.TechnicianId == technicianId ||
                     h.ActionByTechnicianId == technicianId))
                .OrderByDescending(h => h.ActionDateTime)
                .ThenByDescending(h => h.TestResultVerificationHistoryId)
                .ToListAsync();

            return View("VerificationHistoryIndex", histories);
        }
        [HttpGet]

        public async Task<IActionResult> VerificationTimelineModal(int id)
        {
            // 1. Identify the logged-in technician.
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            if (id <= 0)
                return BadRequest("Invalid test ID.");

            // 2. Retrieve the selected test.
            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t =>
                    t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound("Test not found.");

            // 3. Check technician access.
            bool isTestOwner = test.TechnicianId == technicianId;

            bool participated = await _context
                .TestResultVerificationHistories
                .AsNoTracking()
                .AnyAsync(h =>
                    h.TestResult != null &&
                    h.TestResult.TestRequestTestTypeId == id &&
                    h.ActionByTechnicianId == technicianId);

            if (!isTestOwner && !participated)
                return Forbid();

            // 4. Retrieve the current saved test result.
            var currentResult = await _context.TestResults
                .AsNoTracking()
                .Include(r => r.Technician)
                .Include(r => r.VerifiedByTechnician)
                .Where(r => r.TestRequestTestTypeId == id)
                .OrderByDescending(r => r.CapturedDateTime)
                .ThenByDescending(r => r.TestResultId)
                .FirstOrDefaultAsync();

            // 5. Retrieve verification history.
            var history = await _context
                .TestResultVerificationHistories
                .AsNoTracking()
                .Include(h => h.ActionByTechnician)
                .Where(h =>
                    h.TestResult != null &&
                    h.TestResult.TestRequestTestTypeId == id)
                .OrderBy(h => h.ActionDateTime)
                .ThenBy(h => h.TestResultVerificationHistoryId)
                .ToListAsync();

            // 6. Temporary diagnostic response.
            // This confirms whether the database queries work.
            ViewData["Test"] = test;
            ViewData["CurrentResult"] = currentResult;

            return PartialView("_VerificationTimelineModal", history);

            // Once the diagnostic succeeds, we will restore:
            //
            // ViewData["Test"] = test;
            // ViewData["CurrentResult"] = currentResult;
            //
            // return PartialView(
            //     "_VerificationTimelineModal",
            //     history);
        }



        // C# cannot overload GET and POST by HTTP attributes alone.
        // Use a different C# method name and map it to the same action name.

        [HttpPost]
        [ActionName("VerifyResult")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResultDecision(
            int testRequestTestTypeId,
            string decision,
            string? notes)
        {
            var verifierId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(verifierId))
                return Challenge();

            if (testRequestTestTypeId <= 0)
                return BadRequest();

            if (decision != "Verify" && decision != "ReturnForReview")
            {
                TempData["ErrorMessage"] =
                    "Invalid verification decision.";

                return RedirectToAction(
                    nameof(VerifyResult),
                    new { id = testRequestTestTypeId });
            }

            notes = notes?.Trim();

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
                // ==========================================
                // 1. LOAD THE REQUESTED TEST
                // ==========================================

                var test = await _context.TestRequestTestTypes
                    .Include(x => x.TestRequest)
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId ==
                        testRequestTestTypeId);

                if (test == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound();
                }

                if (test.Status != "Completed")
                {
                    await transaction.RollbackAsync();

                    TempData["ErrorMessage"] =
                        "This test is no longer awaiting verification.";

                    return RedirectToAction(
                        nameof(PendingVerification));
                }

                // ==========================================
                // 2. INDEPENDENT VERIFICATION
                // ==========================================

                if (string.IsNullOrWhiteSpace(test.TechnicianId) ||
                    test.TechnicianId == verifierId)
                {
                    await transaction.RollbackAsync();
                    return Forbid();
                }

                var authorised = await _context.TechnicianTestTypes
                    .AnyAsync(x =>
                        x.TechnicianId == verifierId &&
                        x.TestTypeId == test.TestTypeId &&
                        x.IsActive);

                if (!authorised)
                {
                    await transaction.RollbackAsync();
                    return Forbid();
                }

                // ==========================================
                // 3. LOAD THE SAVED TEST RESULT
                // ==========================================

                var results = await _context.TestResults
                    .Where(x =>
                        x.TestRequestTestTypeId ==
                            testRequestTestTypeId &&
                        x.TechnicianId == test.TechnicianId)
                    .ToListAsync();

                if (results.Count != 1 ||
                    string.IsNullOrWhiteSpace(results[0].ResultValue))
                {
                    await transaction.RollbackAsync();

                    TempData["ErrorMessage"] =
                        "Exactly one saved result is required before verification.";

                    return RedirectToAction(
                        nameof(PendingVerification));
                }

                var result = results[0];
                var now = DateTime.UtcNow;

                // ==========================================
                // 4. APPLY THE VERIFICATION DECISION
                // ==========================================

                if (decision == "Verify")
                {
                    result.VerifiedByTechnicianId = verifierId;
                    result.VerifiedDateTime = now;
                    result.VerificationNotes = notes;

                    result.RequiresReview = false;
                    result.ReviewNotes = null;

                    test.Status = "Verified";
                }
                else
                {
                    result.VerifiedByTechnicianId = null;
                    result.VerifiedDateTime = null;
                    result.VerificationNotes = null;

                    result.RequiresReview = true;
                    result.ReviewNotes = notes;

                    test.Status = "To Be Reviewed";
                }

                // ==========================================
                // 5. RECORD VERIFICATION HISTORY
                // ==========================================

                _context.TestResultVerificationHistories.Add(
                    new TestResultVerificationHistoryModel
                    {
                        TestResultId = result.TestResultId,
                        ActionByTechnicianId = verifierId,

                        Action = decision == "Verify"
                            ? "Verified"
                            : "ReturnedForReview",

                        Notes = notes,
                        ActionDateTime = now
                    });

                await _context.SaveChangesAsync();

                // ==========================================
                // 6. CHECK ALL TESTS ON THE REQUEST
                // ==========================================

                // A request is completed only when every
                // requested test has been verified.

                var allTestsVerified =
                    decision == "Verify" &&
                    !await _context.TestRequestTestTypes
                        .AnyAsync(x =>
                            x.TestRequestId == test.TestRequestId &&
                            x.Status != "Verified");

                // ==========================================
                // 7. COMPLETE REQUEST AND QUEUE REPORT
                // ==========================================

                if (allTestsVerified && test.TestRequest != null)
                {
                    test.TestRequest.Status =
                        TestRequestStatus.Completed.ToString();

                    // Prevent duplicate outgoing email records.
                    // The database also has a unique index
                    // on TestRequestId.

                    var alreadyQueued =
                        await _context.TechnicianSentEmails
                            .AnyAsync(x =>
                                x.TestRequestId ==
                                test.TestRequestId);

                    if (!alreadyQueued)
                    {
                        var outgoingEmail =
                            new TechnicianSentEmailModel
                            {
                                TestRequestId =
                                    test.TestRequestId,

                                // Resolved by the future delivery
                                // service, not used for SMTP yet.
                                RecipientEmail =
                                    "Pending recipient resolution",

                                Subject =
                                    $"Laboratory Results - Request #{test.TestRequestId}",

                                Body =
                                    "Your verified laboratory results report is ready.",

                                PdfFileName =
                                    $"LaboratoryReport_{test.TestRequestId}.pdf",

                                PdfAttachment = null,

                                Status = "Pending",

                                CreatedAt = now,

                                SentAt = null,

                                ErrorMessage = null
                            };

                        _context.TechnicianSentEmails.Add(
                            outgoingEmail);
                    }
                }
                else if (
                    decision == "ReturnForReview" &&
                    test.TestRequest != null &&
                    test.TestRequest.Status ==
                        TestRequestStatus.Completed.ToString())
                {
                    test.TestRequest.Status =
                        TestRequestStatus.InProgress.ToString();
                }

                // ==========================================
                // 8. SAVE EVERYTHING ATOMICALLY
                // ==========================================

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // ==========================================
                // 9. DISPLAY SUCCESS MESSAGE
                // ==========================================

                if (decision == "Verify")
                {
                    TempData["SuccessMessage"] =
                        allTestsVerified
                            ? "Test verified successfully. All tests on this request are verified. The laboratory report has been queued for delivery."
                            : "Test result verified successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] =
                        "Test result returned to the original technician for review.";
                }

                return RedirectToAction(
                    nameof(PendingVerification));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "Verification could not be saved. Please try again.";

                return RedirectToAction(
                    nameof(PendingVerification));
            }
        }




        // =====================================================
        // 4. VERIFICATION HISTORY
        // =====================================================
        // Add to Areas/Technician/Controllers/TestsController.cs
        [HttpGet]
        public async Task<IActionResult> VerificationHistory(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId))
                return Challenge();

            var test = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Include(x => x.TestType)
                .Include(x => x.TestRequest)
                .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Authorisation is enforced on the server, not only in the view.
            var authorised = test.TechnicianId == technicianId ||
                await _context.TechnicianTestTypes.AsNoTracking().AnyAsync(x =>
                    x.TechnicianId == technicianId &&
                    x.TestTypeId == test.TestTypeId &&
                    x.IsActive);

            if (!authorised)
                return Forbid();

            var history = await _context.TestResultVerificationHistories
                .AsNoTracking()
                .Include(x => x.ActionByTechnician)
                .Include(x => x.TestResult)
                .Where(x => x.TestResult != null &&
                            x.TestResult.TestRequestTestTypeId == id)
                .OrderByDescending(x => x.ActionDateTime)
                .ThenByDescending(x => x.TestResultVerificationHistoryId)
                .ToListAsync();

            // The current result is shown separately from historical snapshots.
            var currentResults = await _context.TestResults.AsNoTracking()
                .Where(x => x.TestRequestTestTypeId == id)
                .ToListAsync();

            ViewData["Test"] = test;
            ViewData["CurrentResults"] = currentResults;
            return View("VerificationHistory", history);
        }




    }
}
