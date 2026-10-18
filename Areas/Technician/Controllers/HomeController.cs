using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // The dashboard calls this endpoint on load, when a period button is clicked,
        // and every 15 seconds while the page is visible.
        [HttpGet]
        public async Task<IActionResult> TestActivityStats(int days = 7, CancellationToken cancellationToken = default)
        {
            if (days != 7 && days != 30 && days != 90)
                return BadRequest(new { message = "Select 7, 30 or 90 days." });

            var technician = await _userManager.GetUserAsync(User);
            if (technician == null)
                return Unauthorized();

            var todayUtc = DateTime.UtcNow.Date;
            var firstDayUtc = todayUtc.AddDays(1 - days);
            var exclusiveEndUtc = todayUtc.AddDays(1);

            // Only retrieve the logged-in technician's relevant records.
            var events = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(t => t.TechnicianId == technician.Id &&
                    ((t.StartDateTime.HasValue &&
                      t.StartDateTime.Value >= firstDayUtc &&
                      t.StartDateTime.Value < exclusiveEndUtc) ||
                     (t.CompletionDateTime.HasValue &&
                      t.CompletionDateTime.Value >= firstDayUtc &&
                      t.CompletionDateTime.Value < exclusiveEndUtc &&
                      (t.Status == "Completed" || t.Status == "Verified"))))
                .Select(t => new { t.StartDateTime, t.CompletionDateTime, t.Status })
                .ToListAsync(cancellationToken);

            var labels = new List<string>(days);
            var started = new List<int>(days);
            var completed = new List<int>(days);

            for (var i = 0; i < days; i++)
            {
                var dayStart = firstDayUtc.AddDays(i);
                var dayEnd = dayStart.AddDays(1);
                labels.Add(dayStart.ToString("dd MMM"));

                started.Add(events.Count(t =>
                    t.StartDateTime.HasValue &&
                    t.StartDateTime.Value >= dayStart &&
                    t.StartDateTime.Value < dayEnd));

                completed.Add(events.Count(t =>
                    t.CompletionDateTime.HasValue &&
                    t.CompletionDateTime.Value >= dayStart &&
                    t.CompletionDateTime.Value < dayEnd &&
                    (t.Status == "Completed" || t.Status == "Verified")));
            }

            var totalStarted = started.Sum();
            var totalCompleted = completed.Sum();

            Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            Response.Headers.Pragma = "no-cache";

            // Keep labels/started/completed names for the existing dashboard JavaScript.
            return Json(new
            {
                labels,
                started,
                completed,
                totalStarted,
                totalCompleted,
                completionRate = totalStarted > 0
                    ? Math.Round(totalCompleted * 100.0 / totalStarted, 1)
                    : (double?)null,
                generatedAtUtc = DateTime.UtcNow
            });
        }
    }
}
