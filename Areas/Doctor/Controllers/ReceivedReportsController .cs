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
                      sent.PdfAttachment != null && sent.SentAt != null &&
                      request.Status == "Completed" &&
                      _db.TestRequestTestTypes.Any(t => t.TestRequestId == request.TestRequestId && !t.IsDeleted) &&
                      !_db.TestRequestTestTypes.Any(t => t.TestRequestId == request.TestRequestId && !t.IsDeleted &&
                          (t.Status != "Verified" || !_db.TestResults.Any(r =>
                              r.TestRequestTestTypeId == t.TestRequestTestTypeId &&
                              r.ResultValue != null && r.ResultValue != "" &&
                              r.VerifiedByTechnicianId != null && r.VerifiedByTechnicianId != "" &&
                              r.VerifiedDateTime != null))) &&
                      sent.ReportType != "PartialVerified" && sent.ReportType != "Preliminary"
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
                      sent.Status == "Sent" && request.Status == "Completed" &&
                      sent.ReportType != "PartialVerified" && sent.ReportType != "Preliminary" &&
                      _db.TestRequestTestTypes.Any(t => t.TestRequestId == request.TestRequestId && !t.IsDeleted) &&
                      !_db.TestRequestTestTypes.Any(t => t.TestRequestId == request.TestRequestId && !t.IsDeleted &&
                          (t.Status != "Verified" || !_db.TestResults.Any(r =>
                              r.TestRequestTestTypeId == t.TestRequestTestTypeId &&
                              r.ResultValue != null && r.ResultValue != "" &&
                              r.VerifiedByTechnicianId != null && r.VerifiedByTechnicianId != "" &&
                              r.VerifiedDateTime != null)))
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
            if (doctor == null)
                return Unauthorized();

            // Get this doctor's latest completed requests.
            var requestIds = await _db.TestRequests
                .AsNoTracking()
                .Where(r => r.DoctorId == doctor.Id &&
                            r.Status == "Completed")
                .OrderByDescending(r => r.TestRequestId)
                .Select(r => r.TestRequestId)
                .Take(50)
                .ToListAsync(ct);

            if (requestIds.Count == 0)
            {
                NoCache();
                return Json(Array.Empty<object>());
            }

            // Load active tests only.
            var tests = await _db.TestRequestTestTypes
                .AsNoTracking()
                .Where(t => requestIds.Contains(t.TestRequestId)
                            && !t.IsDeleted)
                .Select(t => new
                {
                    t.TestRequestId,
                    t.TestRequestTestTypeId,
                    t.Status
                })
                .ToListAsync(ct);

            var testIds = tests
                .Select(t => t.TestRequestTestTypeId)
                .ToList();

            if (testIds.Count == 0)
            {
                NoCache();
                return Json(Array.Empty<object>());
            }

            // Get tests with a saved and verified result.
            var verifiedIds = await _db.TestResults
                .AsNoTracking()
                .Where(r =>
                    testIds.Contains(r.TestRequestTestTypeId) &&
                    r.ResultValue != null &&
                    r.ResultValue != "" &&
                    r.VerifiedByTechnicianId != null &&
                    r.VerifiedDateTime != null)
                .Select(r => r.TestRequestTestTypeId)
                .Distinct()
                .ToListAsync(ct);

            var verifiedSet = verifiedIds.ToHashSet();

            var eligibleIds = tests
                .GroupBy(t => t.TestRequestId)
                .Where(group =>
                    group.All(t =>
                        t.Status == "Verified" &&
                        verifiedSet.Contains(t.TestRequestTestTypeId)))
                .Select(group => group.Key)
                .OrderByDescending(id => id)
                .Take(10)
                .ToList();

            var reports = eligibleIds.Select(id => new
            {
                id,
                testRequestId = id,
                reportType = "FinalVerified",
                source = "Portal",
                downloadUrl =
                    "/Doctor/ReceivedReports/PortalDownload/" + id
            });

            NoCache();
            return Json(reports);
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

            // Ignore deactivated tests when evaluating final-report readiness.
            var tests = await _db.TestRequestTestTypes.AsNoTracking()
                .Where(t => t.TestRequestId == id && !t.IsDeleted)
                .ToListAsync(ct);
            if (tests.Count == 0) return NotFound();
            if (tests.Any(t => t.Status != "Verified"))
                return NotFound("A final report is not available until every active test is verified.");

            var testIds = tests.Select(t => t.TestRequestTestTypeId).ToList();
            var results = await _db.TestResults.AsNoTracking()
                .Include(r => r.VerifiedByTechnician)
                .Where(r => testIds.Contains(r.TestRequestTestTypeId))
                .ToListAsync(ct);

            var verified = results.Where(r =>
                !string.IsNullOrWhiteSpace(r.ResultValue) &&
                !string.IsNullOrWhiteSpace(r.VerifiedByTechnicianId) &&
                r.VerifiedDateTime.HasValue).ToList();

            // Every active test must have at least one verified result.
            if (tests.Any(t => !verified.Any(r =>
                    r.TestRequestTestTypeId == t.TestRequestTestTypeId)))
                return NotFound("The final report is not yet available.");

            // Use the patient record for the name, not user-supplied parameters.
            var patientName = await _db.Patients.AsNoTracking()
                .Where(p => p.PatientId == request.PatientId)
                .Select(p => p.Name + " " + p.Surname)
                .FirstOrDefaultAsync(ct) ?? "Patient record unavailable";

            var pdf = _pdf.GenerateLaboratoryReport(id, patientName,
                doctor.FullName, verified);
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
