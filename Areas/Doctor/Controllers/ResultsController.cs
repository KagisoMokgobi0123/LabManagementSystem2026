using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Areas.Doctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize]
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;

        public ResultsController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper method to get DoctorID from Identity claims
        private int GetCurrentDoctorId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return 0;

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserID == userId);
            return doctor?.DoctorId ?? 0;  // DoctorId not DoctorID
        }

        // GET: /Doctor/Results/ViewResults
        // View all results across all patients for this doctor
        public IActionResult ViewResults()
        {
            var doctorId = GetCurrentDoctorId();
            if (doctorId == 0) return Unauthorized();

            var requests = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                    .ThenInclude(i => i.TestType)
                .Where(r => r.DoctorId == doctorId.ToString() && r.TestRequestItems.Any(i => i.ResultValue.HasValue))
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }

        // GET: /Doctor/Results/ViewAbnormal
        // View all abnormal results for last 5 days (alerts)
        public IActionResult ViewAbnormal(DateTime? fromDate)
        {
            var doctorId = GetCurrentDoctorId();
            if (doctorId == 0) return Unauthorized();

            var cutoff = fromDate ?? DateTime.Now.AddDays(-5);

            var abnormalItems = _context.TestRequestItems
                .Include(i => i.TestRequest)
                    .ThenInclude(r => r.Patient)
                .Include(i => i.TestType)
                .Where(i =>
                    i.TestRequest.DoctorId == doctorId.ToString() &&
                    i.ResultValue.HasValue &&
                    i.TestType != null &&
                    (i.ResultValue < i.TestType.NormalMin ||
                     i.ResultValue > i.TestType.NormalMax) &&
                    i.CompletionDateTime.HasValue &&
                    i.CompletionDateTime.Value >= cutoff)
                .OrderByDescending(i => i.CompletionDateTime)
                .ToList();

            ViewBag.FromDate = cutoff;
            return View(abnormalItems);
        }

        // GET: /Doctor/Results/ReleaseResults/5
        // View results before releasing to patient
        public IActionResult ReleaseResults(int requestId)
        {
            var doctorId = GetCurrentDoctorId();
            if (doctorId == 0) return Unauthorized();

            var request = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                    .ThenInclude(i => i.TestType)
                .FirstOrDefault(r => r.TestRequestId == requestId && r.DoctorId == doctorId.ToString());

            if (request == null) return NotFound();

            if (request.Status != "Completed")
            {
                TempData["Error"] = "Results can only be released once all tests are completed.";
                return RedirectToAction("ViewResults");
            }

            return View(request);
        }

        // POST: /Doctor/Results/ReleaseResults
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReleaseResults(int requestId, string note, string notifyType)
        {
            var doctorId = GetCurrentDoctorId();
            if (doctorId == 0) return Unauthorized();

            var request = _context.TestRequests
                .FirstOrDefault(r => r.TestRequestId == requestId && r.DoctorId == doctorId.ToString());

            if (request == null) return NotFound();

            var release = new ResultRelease
            {
                TestRequestId = requestId,
                ReleasedAt = DateTime.Now,
                Note = note
            };

            _context.ResultReleases.Add(release);
            request.Status = TestRequestStatus.ReleasedByDoctor.ToString();
            _context.SaveChanges();

            TempData["Message"] = "Results released to patient.";
            return RedirectToAction("ViewResults");
        }

        // GET: /Doctor/Results/DownloadPDF/5
        public IActionResult DownloadPDF(int requestId)
        {
            var doctorId = GetCurrentDoctorId();
            if (doctorId == 0) return Unauthorized();

            var request = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                    .ThenInclude(i => i.TestType)
                .Include(r => r.Doctor)
                .FirstOrDefault(r => r.TestRequestId == requestId && r.DoctorId == doctorId.ToString());

            if (request == null) return NotFound();

            // TODO: Generate PDF using iTextSharp or DinkToPdf
            TempData["Message"] = "PDF generation not yet implemented.";
            return RedirectToAction("ViewResults");
        }
        [AllowAnonymous]
        public IActionResult Debug()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAuthenticated = User.Identity?.IsAuthenticated;
            var userName = User.Identity?.Name;

            var doctor = string.IsNullOrEmpty(userId)
                ? null
                : _context.Doctors.FirstOrDefault(d => d.UserID.ToString() == userId);

            return Content(
                $"IsAuthenticated: {isAuthenticated}\n" +
                $"UserName: {userName}\n" +
                $"UserId from claims: {userId ?? "NULL"}\n" +
                $"Doctor found: {(doctor != null ? doctor.DoctorId.ToString() : "NULL")}\n" +
                $"All Doctors in DB: {string.Join(", ", _context.Doctors.Select(d => $"DoctorId={d.DoctorId} UserID={d.UserID}").ToList())}"
            );
        }
    }
}