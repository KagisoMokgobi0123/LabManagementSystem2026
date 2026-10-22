using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public sealed class AlertsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> users,
        ILogger<AlertsController> logger)
    {
        _db = db;
        _users = users;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? startDate,
        DateTime? endDate)
    {
        var doctorId = _users.GetUserId(User);

        if (string.IsNullOrWhiteSpace(doctorId))
            return Challenge();

        var today = DateTime.Today;
        var start = (startDate ?? today.AddDays(-4)).Date;
        var end = (endDate ?? today).Date;

        if (start > end)
        {
            ModelState.AddModelError(
                string.Empty,
                "The start date cannot be after the end date.");

            start = today.AddDays(-4);
            end = today;
        }

        // End date is inclusive. Use an exclusive upper boundary
        // so results recorded at any time on that date are included.
        var endExclusive = end.AddDays(1);

        var requestIds = await _db.TestRequests
            .AsNoTracking()
            .Where(r => r.DoctorId == doctorId)
            .Select(r => r.TestRequestId)
            .ToListAsync();

        var alerts = await _db.TestRequestTestTypes
            .AsNoTracking()
            .Include(x => x.TestType)
                .ThenInclude(t => t!.MeasurementUnitDetails)
            .Where(x =>
                requestIds.Contains(x.TestRequestId) &&
                x.IsAbnormal &&
                x.Status == "Completed" &&
                x.CompletionDateTime.HasValue &&
                x.CompletionDateTime.Value >= start &&
                x.CompletionDateTime.Value < endExclusive)
            .OrderByDescending(x => x.CompletionDateTime)
            .ToListAsync();

        var requests = await _db.TestRequests
            .AsNoTracking()
            .Include(r => r.Patient)
            .Where(r => r.DoctorId == doctorId)
            .ToDictionaryAsync(r => r.TestRequestId);

        var readIds = await _db.DoctorAlertReads
            .AsNoTracking()
            .Where(x => x.DoctorId == doctorId)
            .Select(x => x.TestRequestTestTypeId)
            .ToListAsync();

        ViewBag.Requests = requests;
        ViewBag.ReadAlertIds = readIds;
        ViewBag.StartDate = start.ToString("yyyy-MM-dd");
        ViewBag.EndDate = end.ToString("yyyy-MM-dd");

        return View(alerts);
    }

    // Called by the dashboard bell to update its unread badge.
    [HttpGet]
    public async Task<IActionResult> NotificationStatus()
    {
        var doctorId = _users.GetUserId(User);

        if (string.IsNullOrWhiteSpace(doctorId))
            return Unauthorized();

        var cutoff = DateTime.Today.AddDays(-4);

        var requestIds = _db.TestRequests
            .AsNoTracking()
            .Where(r => r.DoctorId == doctorId)
            .Select(r => r.TestRequestId);

        var abnormalQuery = _db.TestRequestTestTypes
            .AsNoTracking()
            .Where(x =>
                requestIds.Contains(x.TestRequestId) &&
                x.IsAbnormal &&
                x.Status == "Completed" &&
                x.CompletionDateTime.HasValue &&
                x.CompletionDateTime.Value >= cutoff);

        var readIds = _db.DoctorAlertReads
            .AsNoTracking()
            .Where(x => x.DoctorId == doctorId)
            .Select(x => x.TestRequestTestTypeId);

        var unread = await abnormalQuery
            .Where(x => !readIds.Contains(x.TestRequestTestTypeId))
            .OrderByDescending(x => x.CompletionDateTime)
            .Select(x => new
            {
                id = x.TestRequestTestTypeId,
                date = x.CompletionDateTime
            })
            .ToListAsync();

        return Json(new
        {
            unreadCount = unread.Count,
            unreadAlerts = unread
        });
    }

    // Acknowledge one alert. A doctor cannot mark another doctor's
    // abnormal result as read by submitting a different ID.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var doctorId = _users.GetUserId(User);

        if (string.IsNullOrWhiteSpace(doctorId))
            return Unauthorized();

        var alert = await _db.TestRequestTestTypes
            .AsNoTracking()
            .Where(x =>
                x.TestRequestTestTypeId == id &&
                x.IsAbnormal &&
                x.Status == "Completed")
            .Select(x => new
            {
                x.TestRequestTestTypeId,
                x.TestRequestId
            })
            .FirstOrDefaultAsync();

        if (alert == null)
            return NotFound();

        var belongsToDoctor = await _db.TestRequests
            .AnyAsync(r =>
                r.TestRequestId == alert.TestRequestId &&
                r.DoctorId == doctorId);

        if (!belongsToDoctor)
            return Forbid();

        var alreadyRead = await _db.DoctorAlertReads
            .AnyAsync(x =>
                x.DoctorId == doctorId &&
                x.TestRequestTestTypeId == id);

        if (!alreadyRead)
        {
            _db.DoctorAlertReads.Add(new DoctorAlertReadModel
            {
                DoctorId = doctorId,
                TestRequestTestTypeId = id,
                ReadAtUtc = DateTime.UtcNow
            });

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Concurrent duplicate acknowledgement is harmless;
                // log it and return a safe error for other DB failures.
                _logger.LogWarning(ex, "Unable to save alert read state.");
                return Conflict(new { message = "The alert read state could not be saved. Refresh and try again." });
            }
        }

        return Ok(new { success = true });
    }
}