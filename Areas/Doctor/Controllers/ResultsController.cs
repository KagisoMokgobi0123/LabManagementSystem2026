using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class ResultsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users;
    private readonly DoctorEmailService _email; private readonly ILogger<ResultsController> _logger;
    public ResultsController(ApplicationDbContext db, UserManager<ApplicationUser> users, DoctorEmailService email, ILogger<ResultsController> logger)
    { _db = db; _users = users; _email = email; _logger = logger; }

    [HttpGet]
    public Task<IActionResult> New() => Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var doctorId = _users.GetUserId(User);
        var requests = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Samples).Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType)
            .Where(r => r.DoctorId == doctorId && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"))
            .OrderByDescending(r => r.RequestDate).ToListAsync();
        var resultIds = requests.Select(r => r.TestRequestId).ToList();
        var results = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails).Where(x => resultIds.Contains(x.TestRequestId))
            .OrderByDescending(x => x.CompletionDateTime).ToListAsync();
        ViewBag.Results = results;
        var completedByRequest = results.Where(x => x.Status == "Completed").GroupBy(x => x.TestRequestId).ToDictionary(g => g.Key, g => g.Count());
        ViewBag.RequestIdsWithCompleteSamples = requests.Where(r => r.Samples.Any() && r.Samples.All(s => s.IsReceived) && completedByRequest.TryGetValue(r.TestRequestId, out var count) && count >= r.EffectiveTestTypes.Count).Select(r => r.TestRequestId).ToHashSet();
        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Samples).Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType)
            .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"));
        if (request == null) return NotFound();
        // Do not disclose partial results while any sample/test is incomplete.
        var allItems = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails).Where(x => x.TestRequestId == id).OrderBy(x => x.TestType!.Name).ToListAsync();
        if (!request.Samples.Any() || request.Samples.Any(s => !s.IsReceived) || allItems.Count < request.EffectiveTestTypes.Count || allItems.Any(x => x.Status != "Completed"))
        { TempData["InfoMessage"] = "Results will be available once every required sample has been received and every requested test is complete."; return RedirectToAction(nameof(Index)); }
        var results = allItems;
        if (results.Count == 0) { TempData["InfoMessage"] = "No completed result entries are available for this request yet."; return RedirectToAction(nameof(Index)); }
        ViewBag.Request = request;
        return View(results);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestHistoricalConsent(string patientIdNumber)
    {
        var doctorId = _users.GetUserId(User); var doctor = await _users.GetUserAsync(User);
        if (string.IsNullOrWhiteSpace(patientIdNumber)) { TempData["ErrorMessage"] = "Enter a patient ID number."; return RedirectToAction(nameof(Index)); }
        var patient = await _db.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.IDNumber == patientIdNumber.Trim());
        if (patient == null || patient.User == null) { TempData["ErrorMessage"] = "No patient was found for that ID number."; return RedirectToAction(nameof(Index)); }
        var existing = await _db.PatientDoctorConsents.FirstOrDefaultAsync(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId && c.IsActive);
        if (existing != null) { TempData["InfoMessage"] = "This patient has already granted access. Historical results are shown only for requests included in the consent."; return RedirectToAction(nameof(Historical), new { idNumber = patientIdNumber.Trim() }); }
        var pending = await _db.PatientDoctorConsents.FirstOrDefaultAsync(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId && !c.IsActive && c.Notes != null && c.Notes.Contains("Pending"));
        if (pending == null) _db.PatientDoctorConsents.Add(new PatientDoctorConsentModel { PatientId = patient.PatientId, DoctorUserId = doctorId, IsActive = false, GrantedDate = DateTime.Now, Notes = "Historical result access request Pending" });
        await _db.SaveChangesAsync();
        var consentUrl = Url.Action("Consent", "PatientDashboard", new { area = "" }, Request.Scheme);
        try { await _email.SendAsync(patient.Email, "Action requested: historical laboratory results", $"<p>Hello {System.Net.WebUtility.HtmlEncode(patient.FullName)},</p><p>Dr. {System.Net.WebUtility.HtmlEncode(doctor?.FullName ?? "your doctor")} has requested permission to view selected historical laboratory results in NMB-HLabSys.</p><p>Please sign in to your patient account and review the request at <a href=\"{System.Net.WebUtility.HtmlEncode(consentUrl ?? "")}\">Doctor Consent Requests</a>. Select only the requests you agree to share. Your historical results remain hidden from this doctor until you grant consent.</p>"); TempData["SuccessMessage"] = "Consent request saved and emailed to the patient."; }
        catch (Exception ex) { _logger.LogError(ex, "Consent email failed for patient {PatientId}", patient.PatientId); TempData["WarningMessage"] = "The consent request was saved, but the email could not be sent. Verify email settings and notify the patient through an approved channel."; }
        return RedirectToAction(nameof(Historical), new { idNumber = patientIdNumber.Trim() });
    }

    [HttpGet]
    public async Task<IActionResult> Historical(string? idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber)) return View(new List<TestRequestsModel>());
        var doctorId = _users.GetUserId(User);
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.IDNumber == idNumber.Trim());
        if (patient == null) { ViewBag.Error = "No patient found for this ID number."; return View(new List<TestRequestsModel>()); }
        ViewBag.IdNumber = patient.IDNumber; ViewBag.PatientName = patient.FullName;
        var consent = await _db.PatientDoctorConsents.Include(c => c.ConsentRequestItems).FirstOrDefaultAsync(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId && c.IsActive && c.RevokedDate == null);
        if (consent == null) { ViewBag.ConsentGranted = false; return View(new List<TestRequestsModel>()); }
        ViewBag.ConsentGranted = true;
        var requestIds = consent.ConsentRequestItems.Select(x => x.TestRequestId).ToList();
        var requests = await _db.TestRequests.Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType).Include(r => r.Samples)
            .Where(r => requestIds.Contains(r.TestRequestId) && r.PatientId == patient.UserId && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"))
            .OrderByDescending(r => r.RequestDate).ToListAsync();
        // Require every sample complete; consent alone never exposes incomplete results.
        var historicalItems = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails).Where(x => requestIds.Contains(x.TestRequestId)).OrderByDescending(x => x.CompletionDateTime).ToListAsync();
        var byRequest = historicalItems.GroupBy(x => x.TestRequestId).ToDictionary(g => g.Key, g => g.ToList());
        requests = requests.Where(r => r.Samples.Any() && r.Samples.All(s => s.IsReceived) && byRequest.TryGetValue(r.TestRequestId, out var items) && items.Count >= r.EffectiveTestTypes.Count && items.All(x => x.Status == "Completed")).ToList();
        var allowedRequestIds = requests.Select(r => r.TestRequestId).ToList();
        ViewBag.Results = historicalItems.Where(x => allowedRequestIds.Contains(x.TestRequestId)).ToList();
        return View(requests);
    }
}
