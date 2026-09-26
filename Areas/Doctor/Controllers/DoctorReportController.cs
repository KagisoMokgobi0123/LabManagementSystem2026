using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class DoctorReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doctorId = GetCurrentDoctorId();

            if (doctorId == 0)
            {
                TempData["Error"] = "Please login as a doctor to view reports.";
                return RedirectToAction("Dashboard", "DoctorDashboard");
            }

            var reports = _context.DoctorReports
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToList();

            return View(reports);
        }

        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Generate(DateTime dateFrom, DateTime dateTo)
        {
            var doctorId = GetCurrentDoctorId();

            if (doctorId == 0)
            {
                TempData["Error"] =
                    "Please login as a doctor to generate reports.";

                return RedirectToAction(
                    "Dashboard",
                    "DoctorDashboard");
            }

            if (dateFrom > dateTo)
            {
                TempData["Error"] =
                    "Date from cannot be later than date to.";

                return RedirectToAction("Index");
            }

            var requests = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                    .ThenInclude(i => i.TestType)
                .Where(r =>
                    r.DoctorId == doctorId &&
                    r.RequestDate >= dateFrom &&
                    r.RequestDate <=
                        dateTo.AddDays(1).AddTicks(-1))
                .OrderBy(r => r.RequestDate)
                .ToList();

            var report = new DoctorReport
            {
                DoctorId = doctorId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                GeneratedAt = DateTime.Now
            };

            _context.DoctorReports.Add(report);
            _context.SaveChanges();

            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            TempData["Message"] =
                "Report generated successfully.";

            return View(
                "ReportPreview",
                requests);

        }

        private int GetCurrentDoctorId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return 0;

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserID == userId);
            return doctor?.DoctorId ?? 0;
        }
    }
}