
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Areas.Doctor.ViewModels;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class ReportsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public ReportsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ReportFilterViewModel filter)
    {
        if (filter.From.HasValue &&
            filter.To.HasValue &&
            filter.From.Value.Date > filter.To.Value.Date)
        {
            ModelState.AddModelError(
                nameof(filter.To),
                "End date must be on or after the start date.");
        }

        var data = ModelState.IsValid
            ? await GetRequests(filter)
            : new List<TestRequestsModel>();

        ViewBag.Count = data.Count;
        ViewBag.TestTypes = await _db.TestTypes.OrderBy(t => t.Name).ToListAsync();

        return View(filter);
    }

    [HttpGet]
    public async Task<IActionResult> Download(ReportFilterViewModel filter)
    {
        if (filter.From.HasValue &&
            filter.To.HasValue &&
            filter.From.Value.Date > filter.To.Value.Date)
        {
            ModelState.AddModelError(
                nameof(filter.To),
                "End date must be on or after the start date.");

            ViewBag.Count = 0;
            ViewBag.TestTypes = await _db.TestTypes.OrderBy(t => t.Name).ToListAsync();
            return View(nameof(Index), filter);
        }

        if (!string.IsNullOrWhiteSpace(filter.PatientIdNumber))
        {
            var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.IDNumber == filter.PatientIdNumber.Trim());
            if (patient == null)
            {
                ModelState.AddModelError(nameof(filter.PatientIdNumber), "No patient matches that ID number.");
                ViewBag.Count = 0;
                ViewBag.TestTypes = await _db.TestTypes.OrderBy(t => t.Name).ToListAsync();
                return View(nameof(Index), filter);
            }
        }

        var validStatuses = new[]
        {
        "Submitted",
        "Samples Received",
        "SamplesReceived",
        "InProgress",
        "Completed",
        "Released",
        "ReleasedByDoctor",
        "Cancelled"
    };

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            !validStatuses.Contains(filter.Status))
        {
            ModelState.AddModelError(
                nameof(filter.Status),
                "Select a valid request status.");

            ViewBag.Count = 0;
            ViewBag.TestTypes = await _db.TestTypes.OrderBy(t => t.Name).ToListAsync();
            return View(nameof(Index), filter);
        }

        // Get the currently signed-in doctor, following the dashboard example.
        var currentUser = await _users.GetUserAsync(User);

        if (currentUser == null)
            return Challenge();

        var doctorName = currentUser.FullName ?? "Doctor";

        var requests = await GetRequests(filter);

        var rows = requests.Select(r => new[]
        {
        r.RequestNumber,
        r.Patient?.FullName ?? "Patient",
        r.RequestDate.ToString("dd MMM yyyy"),
        r.Urgency.ToString(),
        r.Status == "SamplesReceived"
            ? "Samples Received"
            : r.Status == "InProgress"
                ? "In progress"
                : r.Status
    }).ToList();

        var criteria =
            $"From: {filter.From?.ToString("dd MMM yyyy") ?? "Any"}; " +
            $"To: {filter.To?.ToString("dd MMM yyyy") ?? "Any"}; " +
            $"Patient ID: {filter.PatientIdNumber ?? "All"}; Test type: {filter.TestTypeId?.ToString() ?? "All"}; Urgency: {filter.Urgency ?? "All"}; " +
            $"Status: {filter.Status ?? "All"}; " +
            $"Records: {requests.Count}";

        var pdf = DoctorPdfReportBuilder.Build(
            "Doctor Test Request Report",
            $"Dr {doctorName}",
            null, // HPCSA number; populate this if required by your PDF builder.
            DateTime.Now.ToString("dd MMM yyyy HH:mm"),
            criteria,
            rows,
            $"No records found for the selected criteria " +
            $"({filter.From?.ToString("dd MMM yyyy") ?? "any start date"} " +
            $"to {filter.To?.ToString("dd MMM yyyy") ?? "any end date"}).");

        return File(
            pdf,
            "application/pdf",
            $"doctor-test-requests-{DateTime.Now:yyyyMMdd-HHmm}.pdf");
    }


    private async Task<List<TestRequestsModel>> GetRequests(
        ReportFilterViewModel filter)
    {
        var doctorId = _users.GetUserId(User);

        var query = _db.TestRequests
            .Include(r => r.Patient)
            .Include(r => r.RequestedTestTypes)
            .Include(r => r.TestTypes)
            .Where(r => r.DoctorId == doctorId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.PatientIdNumber))
        {
            var cleanId = filter.PatientIdNumber.Trim();
            query = query.Where(r => _db.Patients.Any(p => p.UserId == r.PatientId && p.IDNumber == cleanId));
        }

        if (filter.TestTypeId.HasValue)
        {
            query = query.Where(r => r.RequestedTestTypes.Any(t => t.TestTypeId == filter.TestTypeId.Value) || r.TestTypes.Any(t => t.TestTypeId == filter.TestTypeId.Value));
        }

        if (!string.IsNullOrWhiteSpace(filter.Urgency) && Enum.TryParse<Urgency>(filter.Urgency, true, out var urgency))
        {
            query = query.Where(r => r.Urgency == urgency);
        }

        if (filter.From.HasValue)
        {
            query = query.Where(
                r => r.RequestDate >= filter.From.Value.Date);
        }

        if (filter.To.HasValue)
        {
            var end = filter.To.Value.Date.AddDays(1);

            query = query.Where(
                r => r.RequestDate < end);
        }

        if (string.Equals(
            filter.Status,
            "Samples Received",
            StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(
                r => r.Status == "Samples Received" ||
                     r.Status == "SamplesReceived");
        }
        else if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(r => r.Status == filter.Status);
        }

        return await query
            .OrderByDescending(r => r.RequestDate)
            .ThenBy(r => r.RequestNumber)
            .ToListAsync();
    }
}
