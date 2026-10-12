using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Areas.Doctor.ViewModels;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class TestRequestsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users;
    private readonly DoctorEmailService _email; private readonly ILogger<TestRequestsController> _logger;
    public TestRequestsController(ApplicationDbContext db, UserManager<ApplicationUser> users, DoctorEmailService email, ILogger<TestRequestsController> logger)
    { _db = db; _users = users; _email = email; _logger = logger; }

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        var doctorId = _users.GetUserId(User);
        var q = _db.TestRequests.Include(r => r.Patient).Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(x => x.TestType).Include(r => r.Samples)
            .Where(r => r.DoctorId == doctorId).AsQueryable();
        if (string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
            q = q.Where(r => r.Status == "Submitted" || r.Status == "Samples Received" || r.Status == "SamplesReceived" || r.Status == "InProgress");
        else if (string.Equals(status, "Samples Received", StringComparison.OrdinalIgnoreCase)) q = q.Where(r => r.Status == "Samples Received" || r.Status == "SamplesReceived");
        else if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        ViewBag.StatusFilter = status;
        return View(await q.OrderByDescending(r => r.RequestDate).Take(300).ToListAsync());
    }

    [HttpGet]
    public Task<IActionResult> Pending() => Task.FromResult<IActionResult>(RedirectToAction(nameof(Index), new { status = "Pending" }));

    [HttpGet]
    public async Task<IActionResult> Create(string? idNumber)
    {
        var model = new TestRequestCreateViewModel { PatientIDNumber = idNumber?.Trim() ?? "" };
        await LoadRequestOptionsAsync();
        if (!string.IsNullOrWhiteSpace(model.PatientIDNumber))
        {
            var patient = await _db.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.IDNumber == model.PatientIDNumber);
            ViewBag.PatientFound = patient != null;
            ViewBag.Patient = patient;
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestRequestCreateViewModel model)
    {
        model.PatientIDNumber = (model.PatientIDNumber ?? "").Trim();
        if (!new NMB_HLabSys.Validation.SouthAfricanIdNumberAttribute().IsValid(model.PatientIDNumber))
            ModelState.AddModelError(nameof(model.PatientIDNumber), "Enter a valid 13-digit South African ID number.");
        var patient = await _db.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.IDNumber == model.PatientIDNumber);
        if (patient == null) ModelState.AddModelError(nameof(model.PatientIDNumber), "No patient matches this ID number. Use Add Patient to register them first.");
        if (model.TestTypeIds == null || model.TestTypeIds.Count == 0) ModelState.AddModelError(nameof(model.TestTypeIds), "Select at least one test type.");

        var selectedIds = model.TestTypeIds?.Distinct().ToList() ?? new List<int>();
        var testTypes = await _db.TestTypes.Include(t => t.SampleType)
            .Where(t => selectedIds.Contains(t.TestTypeId) && t.IsActive == NMB_HLabSys.Areas.Manager.Models.TestTypeModel.RecordStatus.Active && t.SampleTypeId > 0 && t.SampleType != null)
            .OrderBy(t => t.Name).ToListAsync();
        if (testTypes.Count != selectedIds.Count) ModelState.AddModelError(nameof(model.TestTypeIds), "One or more selected tests are unavailable. Refresh the page and try again.");

        var requiredSampleTypes = testTypes.Select(t => t.SampleTypeId).Distinct().ToList();
        var postedSamples = model.Samples ?? new List<SampleEntryViewModel>();
        if (requiredSampleTypes.Count != postedSamples.Count || requiredSampleTypes.Count != postedSamples.Select(s => s.SampleTypeId).Distinct().Count() || !requiredSampleTypes.All(id => postedSamples.Any(s => s.SampleTypeId == id)))
            ModelState.AddModelError(nameof(model.Samples), "Enter one barcode for every required sample type shown for the selected tests.");
        var duplicateBarcodes = postedSamples.Select(s => (s.Barcode ?? "").Trim().ToUpperInvariant()).Where(s => s.Length > 0).GroupBy(s => s).Any(g => g.Count() > 1);
        if (duplicateBarcodes) ModelState.AddModelError(nameof(model.Samples), "Each sample must have a different barcode.");
        var cleanBarcodes = postedSamples.Select(s => (s.Barcode ?? "").Trim()).Where(s => s.Length > 0).ToList();
        if (cleanBarcodes.Any() && await _db.Samples.AnyAsync(s => cleanBarcodes.Contains(s.Barcode) && !s.IsDeleted)) ModelState.AddModelError(nameof(model.Samples), "One or more barcodes are already registered. Check the tube labels and enter unique barcodes.");
        if (!ModelState.IsValid) { await LoadRequestOptionsAsync(); ViewBag.PatientFound = patient != null; ViewBag.Patient = patient; return View(model); }

        var doctorId = _users.GetUserId(User);
        var doctor = await _users.GetUserAsync(User);
        if (doctorId == null || doctor == null) return Challenge();
        var request = new TestRequestsModel
        {
            RequestNumber = $"NMB-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            PatientId = patient!.UserId, DoctorId = doctorId, RequestDate = DateTime.Now,
            Urgency = model.Urgency, ClinicalNotes = model.ClinicalNotes?.Trim(),
            Status = TestRequestStatus.Submitted.ToString()
        };
        foreach (var testType in testTypes)
            request.RequestedTestTypes.Add(new TestRequestTestTypeModel { TestTypeId = testType.TestTypeId, Status = "Submitted" });
        foreach (var sampleTypeId in requiredSampleTypes)
        {
            var entry = postedSamples.First(s => s.SampleTypeId == sampleTypeId);
            request.Samples.Add(new SampleModel { SampleTypeId = sampleTypeId, Barcode = entry.Barcode.Trim(), Status = "Submitted", IsReceived = false, IsDeleted = false });
        }
        _db.TestRequests.Add(request);
        try
        {
            await _db.SaveChangesAsync();
            try
            {
                await _email.SendAsync(patient.Email, "Laboratory test request received", $"<p>Hello {System.Net.WebUtility.HtmlEncode(patient.FullName)},</p><p>Your doctor has submitted a laboratory test request.</p><p><b>Request number:</b> {System.Net.WebUtility.HtmlEncode(request.RequestNumber)}<br/><b>Date:</b> {request.RequestDate:dd MMM yyyy HH:mm}<br/><b>Status:</b> Submitted</p><p>Contact your healthcare provider if you have questions.</p>");
                TempData["SuccessMessage"] = $"Request {request.RequestNumber} submitted successfully. The patient was emailed a confirmation.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {RequestNumber} saved but patient confirmation email failed", request.RequestNumber);
                TempData["WarningMessage"] = $"Request {request.RequestNumber} was saved, but the confirmation email could not be sent. Check the email configuration and resend the notification.";
            }
            return RedirectToAction(nameof(Details), new { id = request.TestRequestId });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Could not save test request for patient {PatientId}", patient.PatientId);
            ModelState.AddModelError("", "The request could not be saved. Check the barcodes and try again.");
            await LoadRequestOptionsAsync(); ViewBag.PatientFound = true; ViewBag.Patient = patient; return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Doctor)
            .Include(r => r.Samples).ThenInclude(s => s.SampleType)
            .Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType).ThenInclude(t => t!.SampleType)
            .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId);
        if (request == null) return NotFound();
        ViewBag.PatientRecord = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == request.PatientId);
        return View(request);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Samples).Include(r => r.RequestedTestTypes)
            .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId);
        if (request == null) return NotFound();
        if (request.Status != "Submitted" || request.Samples.Any(s => s.IsReceived))
        { TempData["ErrorMessage"] = "A request cannot be cancelled after sample processing has started."; return RedirectToAction(nameof(Details), new { id }); }
        request.Status = TestRequestStatus.Cancelled.ToString();
        foreach (var sample in request.Samples) { sample.Status = "Cancelled"; }
        foreach (var test in request.RequestedTestTypes) test.Status = "Cancelled";
        await _db.SaveChangesAsync();
        try
        {
            if (!string.IsNullOrWhiteSpace(request.Patient?.Email))
                await _email.SendAsync(request.Patient.Email, "Laboratory test request cancelled", $"<p>Hello {System.Net.WebUtility.HtmlEncode(request.Patient.FullName)},</p><p>Laboratory request <b>{System.Net.WebUtility.HtmlEncode(request.RequestNumber)}</b> was cancelled by your doctor before sample processing began.</p>");
            TempData["SuccessMessage"] = $"Request {request.RequestNumber} was cancelled and the patient was notified. The request and barcode history were retained for audit.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cancellation email failed for request {RequestNumber}", request.RequestNumber);
            TempData["WarningMessage"] = $"Request {request.RequestNumber} was cancelled, but the patient notification email could not be sent.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Samples).FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId);
        if (request == null) return NotFound();
        if (request.Status != "Submitted" || request.Samples.Any(s => s.IsReceived))
        { TempData["InfoMessage"] = "A request can only be edited before any sample is received."; return RedirectToAction(nameof(Details), new { id }); }
        return View(new TestRequestEditViewModel { TestRequestId = request.TestRequestId, RequestNumber = request.RequestNumber, Urgency = request.Urgency, ClinicalNotes = request.ClinicalNotes });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TestRequestEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Samples).FirstOrDefaultAsync(r => r.TestRequestId == model.TestRequestId && r.DoctorId == doctorId);
        if (request == null) return NotFound();
        if (request.Status != "Submitted" || request.Samples.Any(s => s.IsReceived))
        { TempData["ErrorMessage"] = "This request can no longer be edited because sample processing has started."; return RedirectToAction(nameof(Details), new { id = request.TestRequestId }); }
        request.Urgency = model.Urgency; request.ClinicalNotes = model.ClinicalNotes?.Trim();
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Request {request.RequestNumber} was updated.";
        return RedirectToAction(nameof(Details), new { id = request.TestRequestId });
    }

    [HttpGet]
    public async Task<IActionResult> FindPatient(string idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber)) return BadRequest(new { found = false, message = "Enter a patient ID number." });
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.IDNumber == idNumber.Trim());
        if (patient == null) return Json(new { found = false, message = "Patient not found.", createUrl = Url.Action("Create", "Patients", new { area = "Doctor", idNumber = idNumber.Trim() }) });
        var conditions = await _db.PatientConditions.Include(x => x.MedicalCondition).Where(x => x.PatientId == patient.PatientId).Select(x => x.MedicalCondition!.ConditionName).ToListAsync();
        var allergies = await _db.PatientAllergies.Include(x => x.Allergy).Where(x => x.PatientId == patient.PatientId).Select(x => x.Allergy!.AllergyName).ToListAsync();
        var meds = await _db.PatientMedications.Include(x => x.Medication).Where(x => x.PatientId == patient.PatientId).Select(x => x.Medication!.MedicationName).ToListAsync();
        var doctorId = _users.GetUserId(User);
        var ownIds = await _db.TestRequests.Where(r => r.PatientId == patient.UserId && r.DoctorId == doctorId && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor")).Select(r => r.TestRequestId).ToListAsync();
        var consent = await _db.PatientDoctorConsents.Include(c => c.ConsentRequestItems).FirstOrDefaultAsync(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId && c.IsActive && c.RevokedDate == null);
        var allowedIds = ownIds.Concat(consent?.ConsentRequestItems.Select(i => i.TestRequestId) ?? Enumerable.Empty<int>()).Distinct().ToList();
        var previousRequests = await _db.TestRequests.Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType).Include(r => r.Samples).Where(r => allowedIds.Contains(r.TestRequestId) && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor")).ToListAsync();
        var items = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails).Where(x => allowedIds.Contains(x.TestRequestId)).OrderByDescending(x => x.CompletionDateTime).ToListAsync();
        var previousResults = previousRequests.Where(r => r.Samples.Any() && r.Samples.All(s => s.IsReceived) && items.Count(x => x.TestRequestId == r.TestRequestId) >= r.EffectiveTestTypes.Count && items.Where(x => x.TestRequestId == r.TestRequestId).All(x => x.Status == "Completed")).SelectMany(r => items.Where(x => x.TestRequestId == r.TestRequestId).Select(x => new { requestNumber = r.RequestNumber, date = r.RequestDate.ToString("dd MMM yyyy"), test = x.TestType!.Name, result = x.ResultValue, unit = x.TestType.MeasurementUnitDetails?.Unit ?? "", low = x.TestType.NormalMin, high = x.TestType.NormalMax, abnormal = x.IsAbnormal, notes = x.ResultNotes })).ToList();
        return Json(new { found = true, patient = new { patient.Name, patient.Surname, patient.DateOfBirth, patient.Email, patient.ContactNumber, patient.PatientId }, conditions, allergies, medications = meds, previousResults, detailsUrl = Url.Action("Details", "Patients", new { area = "Doctor", id = patient.PatientId }) });
    }

    private async Task LoadRequestOptionsAsync()
    {
        ViewBag.TestTypes = await _db.TestTypes.Include(t => t.TestCategory).Include(t => t.SampleType)
            .Where(t => t.IsActive == NMB_HLabSys.Areas.Manager.Models.TestTypeModel.RecordStatus.Active && t.SampleTypeId > 0 && t.SampleType != null).OrderBy(t => t.Name).ToListAsync();
        ViewBag.SampleTypes = await _db.SampleTypes.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        ViewBag.Urgencies = Enum.GetValues<Urgency>();
    }
}
