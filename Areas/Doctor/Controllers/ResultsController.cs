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
        var resultGroups = results.GroupBy(x => x.TestRequestId)
            .ToDictionary(g => g.Key, g => g.ToList());
        ViewBag.RequestIdsWithCompleteSamples = requests.Where(r =>
            r.Samples.Any() && r.Samples.All(sample => sample.IsReceived)
            && resultGroups.TryGetValue(r.TestRequestId, out var entries)
            && entries.Count >= r.EffectiveTestTypes.Count
            && entries.Count > 0
            && entries.All(item => (item.Status == "Completed" || item.Status == "Verified") && item.IsVerified))
            .Select(r => r.TestRequestId).ToHashSet();
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
        if (!request.Samples.Any() || request.Samples.Any(s => !s.IsReceived) || allItems.Count < request.EffectiveTestTypes.Count || allItems.Any(x => (x.Status != "Completed" && x.Status != "Verified") || !x.IsVerified))
        { TempData["InfoMessage"] = "Results will be available once every required sample has been received and every requested test is complete."; return RedirectToAction(nameof(Index)); }
        var results = allItems;
        if (results.Count == 0) { TempData["InfoMessage"] = "No completed result entries are available for this request yet."; return RedirectToAction(nameof(Index)); }
        ViewBag.Request = request;
        return View(results);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Release(int id, string releaseNote, string notificationMethod = "Email")
    {
        var doctorId = _users.GetUserId(User);
        releaseNote = (releaseNote ?? string.Empty).Trim();
        if (releaseNote.Length == 0 || releaseNote.Length > 2000)
        {
            TempData["ErrorMessage"] = "A release note is required (maximum 2000 characters).";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (notificationMethod != "Email")
        {
            TempData["ErrorMessage"] = "Only email notification is currently supported.";
            return RedirectToAction(nameof(Details), new { id });
        }
        var request = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Samples).Include(r => r.RequestedTestTypes)
            .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId);
        if (request == null) return NotFound();
        var items = await _db.TestRequestTestTypes.Include(x => x.TestType)
            .Where(x => x.TestRequestId == id).ToListAsync();
        if (request.Status == "Released" || request.Status == "ReleasedByDoctor")
        {
            TempData["InfoMessage"] = "These results have already been released.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (!request.Samples.Any() || request.Samples.Any(x => !x.IsReceived) || items.Count < request.EffectiveTestTypes.Count || items.Count == 0 || items.Any(x => (x.Status != "Completed" && x.Status != "Verified") || !x.IsVerified))
        {
            TempData["ErrorMessage"] = "Results cannot be released until all samples and tests are complete and every result is verified.";
            return RedirectToAction(nameof(Details), new { id });
        }
        var doctor = await _users.GetUserAsync(User);
        request.Status = "ReleasedByDoctor";
        request.DoctorReleaseNote = releaseNote;
        request.PatientNotificationMethod = "Email";
        request.ReleasedByDoctorId = doctorId;
        request.ResultsReleasedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(request.Patient?.Email)) throw new InvalidOperationException("The patient has no email address.");
            await _email.SendAsync(request.Patient.Email, "Your laboratory results have been released",
                $"<p>Hello {System.Net.WebUtility.HtmlEncode(request.Patient.FullName)},</p><p>Dr. {System.Net.WebUtility.HtmlEncode(doctor?.FullName ?? "your doctor")} has reviewed and released the results for request <b>{System.Net.WebUtility.HtmlEncode(request.RequestNumber)}</b>.</p><p><b>Doctor's note:</b> {System.Net.WebUtility.HtmlEncode(releaseNote)}</p><p>Please sign in to your patient account to view your results.</p>");
            TempData["SuccessMessage"] = "Results released and patient email sent.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Results released but patient email failed for request {RequestNumber}", request.RequestNumber);
            TempData["WarningMessage"] = "Results were released, but the patient notification email failed. Check email settings and notify the patient through an approved channel.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var doctorId = _users.GetUserId(User);
        var request = await _db.TestRequests.Include(r => r.Patient).FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId && (r.Status == "Released" || r.Status == "ReleasedByDoctor"));
        if (request == null) return NotFound();
        var items = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails).Where(x => x.TestRequestId == id).OrderBy(x => x.TestType!.Name).ToListAsync();
        if (items.Count == 0 || items.Any(x => (x.Status != "Completed" && x.Status != "Verified") || !x.IsVerified)) return Forbid();
        var rows = items.Select(x => new[] { x.TestType?.Name ?? "Test", x.ResultValue?.ToString() ?? "—", x.TestType?.MeasurementUnitDetails?.Unit ?? "", $"{x.TestType?.NormalMin} - {x.TestType?.NormalMax}", x.IsAbnormal ? "ABNORMAL" : "Within range", x.ResultNotes ?? "" }).ToList();
        var doctor = await _users.GetUserAsync(User);
        var pdf = DoctorPdfReportBuilder.Build("Laboratory Results", $"Dr {doctor?.FullName ?? "Doctor"}", null, DateTime.Now.ToString("dd MMM yyyy HH:mm"), $"Request: {request.RequestNumber}; Patient: {request.Patient?.FullName}; Status: {request.Status}; Release note: {request.DoctorReleaseNote ?? "—"}", rows, "No result rows available.");
        return File(pdf, "application/pdf", $"results-{request.RequestNumber}.pdf");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestHistoricalConsent(string patientIdNumber, int testRequestId)
    {
        var doctorId = _users.GetUserId(User);
        var doctor = await _users.GetUserAsync(User);
        var cleanId = (patientIdNumber ?? string.Empty).Trim();
        if (cleanId.Length != 13 || cleanId.Any(c => c < '0' || c > '9'))
        {
            TempData["ErrorMessage"] = "Enter a valid 13-digit patient ID number.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        var patient = await _db.Patients.Include(p => p.User)
            .FirstOrDefaultAsync(p => p.IDNumber == cleanId);
        if (patient == null || patient.User == null)
        {
            TempData["ErrorMessage"] = "No patient was found for that ID number.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        var request = await _db.TestRequests
            .Include(r => r.Samples)
            .Include(r => r.RequestedTestTypes)
            .ThenInclude(rt => rt.TestType)
            .Include(r => r.TestTypes)
            .FirstOrDefaultAsync(r => r.TestRequestId == testRequestId
                && r.PatientId == patient.UserId
                && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"));
        if (request == null)
        {
            TempData["ErrorMessage"] = "That historical request does not belong to this patient or is not complete.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        var resultItems = await _db.TestRequestTestTypes
            .Where(x => x.TestRequestId == testRequestId).ToListAsync();
        if (!request.Samples.Any() || request.Samples.Any(x => !x.IsReceived)
            || resultItems.Count < request.EffectiveTestTypes.Count
            || resultItems.Count == 0
            || resultItems.Any(x => (x.Status != "Completed" && x.Status != "Verified") || !x.IsVerified))
        {
            TempData["ErrorMessage"] = "Access can only be requested for a request whose every sample and verified test result is complete.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        var alreadyHasAccess = await _db.PatientDoctorConsents.AnyAsync(c =>
            c.PatientId == patient.PatientId && c.DoctorUserId == doctorId
            && c.IsActive && c.RevokedDate == null
            && c.ConsentRequestItems.Any(i => i.TestRequestId == testRequestId));
        if (alreadyHasAccess)
        {
            TempData["InfoMessage"] = "You already have permission to view this patient's historical request.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        var pendingConsent = await _db.PatientDoctorConsents
            .Include(c => c.ConsentRequestItems)
            .FirstOrDefaultAsync(c => c.PatientId == patient.PatientId
                && c.DoctorUserId == doctorId && !c.IsActive
                && c.Notes != null && c.Notes.Contains("Pending")
                && c.ConsentRequestItems.Any(i => i.TestRequestId == testRequestId));
        if (pendingConsent != null)
        {
            TempData["InfoMessage"] = "A permission request for this historical record is already awaiting the patient.";
            return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
        }

        // Each requested record gets a pending consent row, so the patient can
        // approve or decline the exact record instead of granting broad access.
        var consent = new PatientDoctorConsentModel
        {
            PatientId = patient.PatientId,
            DoctorUserId = doctorId,
            IsActive = false,
            GrantedDate = DateTime.Now,
            Notes = $"Historical result access request Pending for request {request.RequestNumber}"
        };
        _db.PatientDoctorConsents.Add(consent);
        await _db.SaveChangesAsync();
        _db.ConsentRequestItems.Add(new ConsentRequestItemModel
        {
            PatientConsentId = consent.PatientDoctorConsentId,
            TestRequestId = request.TestRequestId
        });
        await _db.SaveChangesAsync();

        var consentUrl = Url.Action("Consent", "PatientDashboard", new { area = "" }, Request.Scheme);
        try
        {
            if (string.IsNullOrWhiteSpace(patient.Email))
                throw new InvalidOperationException("The patient has no email address.");
            await _email.SendAsync(patient.Email, "Action requested: historical laboratory result access",
                $"<p>Hello {System.Net.WebUtility.HtmlEncode(patient.FullName)},</p><p>Dr. {System.Net.WebUtility.HtmlEncode(doctor?.FullName ?? "your doctor")} has requested access to historical laboratory request <b>{System.Net.WebUtility.HtmlEncode(request.RequestNumber)}</b>.</p><p>No results have been shared. Please sign in and approve or decline this specific request in <a href=\"{System.Net.WebUtility.HtmlEncode(consentUrl ?? "")}\">Consent Management</a>.</p>");
            TempData["SuccessMessage"] = $"Permission request for {request.RequestNumber} was sent to the patient by email.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Historical consent email failed for patient {PatientId}, request {RequestId}", patient.PatientId, testRequestId);
            TempData["WarningMessage"] = "The permission request was saved, but the email could not be sent. Verify email settings and notify the patient through an approved channel.";
        }
        return RedirectToAction(nameof(Historical), new { idNumber = cleanId });
    }

    [HttpGet]
    public async Task<IActionResult> Historical(string? idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber)) return View(new List<TestRequestsModel>());
        var doctorId = _users.GetUserId(User);
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.IDNumber == idNumber.Trim());
        if (patient == null) { ViewBag.Error = "No patient found for this ID number."; return View(new List<TestRequestsModel>()); }
        ViewBag.IdNumber = patient.IDNumber; ViewBag.PatientName = patient.FullName;

        // List eligible historical requests without revealing their results.
        var candidateRequests = await _db.TestRequests
            .Include(r => r.Samples)
            .Include(r => r.TestTypes)
            .Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType)
            .Where(r => r.PatientId == patient.UserId
                && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"))
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
        var candidateIds = candidateRequests.Select(r => r.TestRequestId).ToList();
        var candidateResults = await _db.TestRequestTestTypes
            .Where(x => candidateIds.Contains(x.TestRequestId)).ToListAsync();
        var resultCounts = candidateResults.GroupBy(x => x.TestRequestId)
            .ToDictionary(g => g.Key, g => g.ToList());
        var eligibleRequests = candidateRequests.Where(r =>
            r.Samples.Any() && r.Samples.All(s => s.IsReceived)
            && resultCounts.TryGetValue(r.TestRequestId, out var entries)
            && entries.Count >= r.EffectiveTestTypes.Count
            && entries.Count > 0
            && entries.All(x => (x.Status == "Completed" || x.Status == "Verified") && x.IsVerified)).ToList();
        ViewBag.HiddenRequests = eligibleRequests;
        var pendingRequestIds = await _db.PatientDoctorConsents
            .Where(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId
                && !c.IsActive && c.Notes != null && c.Notes.Contains("Historical result access request Pending"))
            .SelectMany(c => c.ConsentRequestItems.Select(i => i.TestRequestId))
            .Distinct().ToListAsync();
        ViewBag.PendingRequestIds = pendingRequestIds.ToHashSet();

        var activeConsents = await _db.PatientDoctorConsents
            .Include(c => c.ConsentRequestItems)
            .Where(c => c.PatientId == patient.PatientId && c.DoctorUserId == doctorId
                && c.IsActive && c.RevokedDate == null)
            .ToListAsync();
        var requestIds = activeConsents.SelectMany(c => c.ConsentRequestItems)
            .Select(i => i.TestRequestId).Distinct().ToList();
        ViewBag.ConsentGranted = requestIds.Count > 0;
        if (requestIds.Count == 0)
        {
            ViewBag.Results = new List<NMB_HLabSys.Areas.Manager.Models.TestRequestTestTypeModel>();
            return View(new List<TestRequestsModel>());
        }

        // Consent is scoped to individual request IDs. An empty/general consent
        // does not reveal historical result values through this workflow.
        var requests = await _db.TestRequests
            .Include(r => r.TestTypes)
            .Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType)
            .Include(r => r.Samples)
            .Where(r => requestIds.Contains(r.TestRequestId) && r.PatientId == patient.UserId
                && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"))
            .OrderByDescending(r => r.RequestDate).ToListAsync();
        var historicalItems = await _db.TestRequestTestTypes
            .Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails)
            .Where(x => requestIds.Contains(x.TestRequestId))
            .OrderByDescending(x => x.CompletionDateTime).ToListAsync();
        var byRequest = historicalItems.GroupBy(x => x.TestRequestId)
            .ToDictionary(g => g.Key, g => g.ToList());
        requests = requests.Where(r => r.Samples.Any() && r.Samples.All(s => s.IsReceived)
            && byRequest.TryGetValue(r.TestRequestId, out var items)
            && items.Count >= r.EffectiveTestTypes.Count && items.Count > 0
            && items.All(x => (x.Status == "Completed" || x.Status == "Verified") && x.IsVerified)).ToList();
        var allowedRequestIds = requests.Select(r => r.TestRequestId).ToList();
        ViewBag.Results = historicalItems.Where(x => allowedRequestIds.Contains(x.TestRequestId)).ToList();
        return View(requests);
    }
}
