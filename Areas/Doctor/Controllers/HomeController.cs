using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.ViewModels;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger)
    {
        _db = db;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var doctorId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(doctorId))
            return Challenge();

        // PatientModel has no doctor-owner field in the current schema, so this card
        // reports all registered patient records. Requests and analytics are scoped
        // to the signed-in doctor.
        var model = new DoctorDashboardViewModel
        {
            PatientCount = await _db.Patients.AsNoTracking().CountAsync()
        };

        var requests = await _db.TestRequests
            .AsNoTracking()
            .Where(r => r.DoctorId == doctorId)
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new RequestSnapshot
            {
                TestRequestId = r.TestRequestId,
                RequestNumber = r.RequestNumber,
                PatientId = r.PatientId,
                RequestDate = r.RequestDate,
                Status = r.Status,
                Urgency = r.Urgency.ToString()
            })
            .ToListAsync();

        model.TestRequestCount = requests.Count;
        model.SubmittedCount = requests.Count(r => IsStatus(r.Status, "Submitted"));
        model.InProgressCount = requests.Count(r =>
            IsStatus(r.Status, "InProgress") || IsStatus(r.Status, "In Progress") ||
            IsStatus(r.Status, "Samples Received") || IsStatus(r.Status, "Received"));
        model.ReadyToReleaseCount = requests.Count(r => IsStatus(r.Status, "Completed"));
        model.ActiveRequestCount = requests.Count(r =>
            !IsStatus(r.Status, "Completed") && !IsStatus(r.Status, "Released") &&
            !IsStatus(r.Status, "Released by doctor") && !IsStatus(r.Status, "Cancelled"));

        // Count abnormal per-test entries only for this doctor's requests.
        // RequestedTestTypes is the project navigation to TestRequestTestTypes.
        try
        {
            var requestedTests = _db.TestRequests
                .AsNoTracking()
                .Where(r => r.DoctorId == doctorId)
                .SelectMany(r => r.RequestedTestTypes);

            model.AbnormalResultsCount = await requestedTests.CountAsync(t => t.IsAbnormal);
            model.CompletedResultsCount = await requestedTests.CountAsync(t => t.ResultValue != null);
        }
        catch (Exception ex)
        {
            // Do not fail the entire dashboard if a database has not yet applied
            // the result-fields migration. Log it so the schema issue is visible.
            _logger.LogWarning(ex, "Could not load abnormal-result counts. Check the doctor result-fields migration.");
            model.AbnormalResultsCount = 0;
            model.CompletedResultsCount = 0;
            ViewBag.ResultAnalyticsWarning = "Abnormal-result analytics are unavailable until the result-fields database migration is applied.";
        }

        var patientsByUserId = await _db.Patients
            .AsNoTracking()
            .Select(p => new { p.UserId, p.Name, p.Surname })
            .ToDictionaryAsync(p => p.UserId, p => (p.Name + " " + p.Surname).Trim());

        model.RecentRequests = requests.Take(8).Select(r => new DashboardRequestRowViewModel
        {
            TestRequestId = r.TestRequestId,
            RequestNumber = r.RequestNumber ?? $"REQ-{r.TestRequestId}",
            PatientName = patientsByUserId.TryGetValue(r.PatientId, out var patientName)
                ? patientName : "Patient record unavailable",
            RequestDate = r.RequestDate,
            Urgency = r.Urgency,
            Status = r.Status ?? "Unknown"
        }).ToList();

        var statusGroups = requests
            .GroupBy(r => string.IsNullOrWhiteSpace(r.Status) ? "Unknown" : r.Status.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new DashboardChartItemViewModel { Label = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();
        model.StatusChart = statusGroups;

        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        for (var i = 5; i >= 0; i--)
        {
            var month = currentMonth.AddMonths(-i);
            model.MonthlyChart.Add(new DashboardChartItemViewModel
            {
                Label = month.ToString("MMM yyyy"),
                Count = requests.Count(r => r.RequestDate.Year == month.Year && r.RequestDate.Month == month.Month)
            });
        }

        return View(model);
    }

    private static bool IsStatus(string? actual, string expected) =>
        string.Equals(actual?.Replace("_", " ").Trim(), expected.Replace("_", " ").Trim(), StringComparison.OrdinalIgnoreCase);

    private sealed class RequestSnapshot
    {
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Urgency { get; set; } = string.Empty;
    }
}
