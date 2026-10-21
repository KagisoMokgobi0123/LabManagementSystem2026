using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ReceivedReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;
        private readonly LaboratoryPdfService _pdf;

        public ReceivedReportsController(ApplicationDbContext db,
            UserManager<ApplicationUser> users, LaboratoryPdfService pdf)
        {
            _db = db;
            _users = users;
            _pdf = pdf;
        }

        [HttpGet]
        public async Task<IActionResult> Recent(CancellationToken ct)
        {
            var doctor = await _users.GetUserAsync(User);
            if (doctor == null) return Unauthorized();
            var reports = await (
                from sent in _db.TechnicianSentEmails.AsNoTracking()
                join request in _db.TestRequests.AsNoTracking()
                    on sent.TestRequestId equals request.TestRequestId
                where request.DoctorId == doctor.Id && sent.Status == "Sent" &&
                      sent.PdfAttachment != null && sent.SentAt != null
                orderby sent.SentAt descending
                select new
                {
                    id = sent.SentEmailId,
                    testRequestId = sent.TestRequestId,
                    testRequestTestTypeId = sent.TestRequestTestTypeId,
                    reportType = sent.ReportType,
                    sentAt = sent.SentAt
                })
                .Take(10).ToListAsync(ct);
            NoCache();
            return Json(reports);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id, CancellationToken ct)
        {
            var doctor = await _users.GetUserAsync(User);
            if (doctor == null) return Unauthorized();
            var report = await (
                from sent in _db.TechnicianSentEmails.AsNoTracking()
                join request in _db.TestRequests.AsNoTracking()
                    on sent.TestRequestId equals request.TestRequestId
                where sent.SentEmailId == id && request.DoctorId == doctor.Id &&
                      sent.Status == "Sent"
                select new { sent.PdfAttachment }).FirstOrDefaultAsync(ct);
            if (report?.PdfAttachment == null || report.PdfAttachment.Length == 0)
                return NotFound();
            NoCache();
            return File(report.PdfAttachment, "application/pdf");
        }

        // Separate portal-only feed; does not rely on SMTP or Sent status.
        [HttpGet]
        public async Task<IActionResult> PortalRecent(CancellationToken ct)
        {
            var doctor = await _users.GetUserAsync(User);
            if (doctor == null) return Unauthorized();
            var requests = await _db.TestRequests.AsNoTracking()
                .Where(q => q.DoctorId == doctor.Id && q.Status == "Completed")
                .Where(q => _db.TestRequestTestTypes.Any(t =>
                    t.TestRequestId == q.TestRequestId && t.Status == "Verified" &&
                    _db.TestResults.Any(r => r.TestRequestTestTypeId == t.TestRequestTestTypeId &&
                        r.ResultValue != null && r.ResultValue != "" &&
                        r.VerifiedByTechnicianId != null && r.VerifiedDateTime != null)))
                .OrderByDescending(q => q.TestRequestId)
                .Select(q => new {
                    id = q.TestRequestId,
                    testRequestId = q.TestRequestId,
                    reportType = "PartialVerified",
                    source = "Portal",
                    downloadUrl = "/Doctor/ReceivedReports/PortalDownload/" + q.TestRequestId
                })
                .Take(10).ToListAsync(ct);
            NoCache();
            return Json(requests);
        }

        [HttpGet]
        public async Task<IActionResult> PortalDownload(int id, CancellationToken ct)
        {
            var doctor = await _users.GetUserAsync(User);
            if (doctor == null) return Unauthorized();
            var request = await _db.TestRequests.AsNoTracking()
                .FirstOrDefaultAsync(q => q.TestRequestId == id &&
                    q.DoctorId == doctor.Id && q.Status == "Completed", ct);
            if (request == null) return NotFound();

            var tests = await _db.TestRequestTestTypes.AsNoTracking()
                .Where(t => t.TestRequestId == id)
                .ToListAsync(ct);
            if (tests.Count == 0) return NotFound();

            var testIds = tests.Select(t => t.TestRequestTestTypeId).ToList();
            var results = await _db.TestResults.AsNoTracking()
                .Include(r => r.VerifiedByTechnician)
                .Where(r => testIds.Contains(r.TestRequestTestTypeId))
                .ToListAsync(ct);

            var verified = results.Where(r =>
                tests.Any(t => t.TestRequestTestTypeId == r.TestRequestTestTypeId &&
                    t.Status == "Verified") &&
                !string.IsNullOrWhiteSpace(r.ResultValue) &&
                !string.IsNullOrWhiteSpace(r.VerifiedByTechnicianId) &&
                r.VerifiedDateTime.HasValue).ToList();

            if (verified.Count == 0) return NotFound("No verified results available.");

            var verifiedIds = verified.Select(r => r.TestRequestTestTypeId).ToHashSet();
            var missing = tests.Where(t => !verifiedIds.Contains(t.TestRequestTestTypeId))
                .Select(t => t.TestRequestTestTypeId).ToList();

            // A fully complete request belongs to the existing final-report workflow.
            if (missing.Count == 0)
                return Conflict("This request is complete. Use the verified final-report workflow.");

            // Use the patient record for the name, not user-supplied parameters.
            var patientName = await _db.Patients.AsNoTracking()
                .Where(p => p.PatientId == request.PatientId)
                .Select(p => p.Name + " " + p.Surname)
                .FirstOrDefaultAsync(ct) ?? "Patient record unavailable";

            var pdf = _pdf.GeneratePartialVerifiedReport(id, patientName,
                doctor.FullName, verified, missing);
            NoCache();
            return File(pdf, "application/pdf");
        }

        private void NoCache()
        {
            Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            Response.Headers["X-Content-Type-Options"] = "nosniff";
        }
    }
}
