
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Services
{
    public class LaboratoryReportDeliveryService
    {
        private readonly ApplicationDbContext _context;
        private readonly LaboratoryPdfService _pdfService;

        public LaboratoryReportDeliveryService(
            ApplicationDbContext context,
            LaboratoryPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<bool> PrepareReportAsync(
            int testRequestId)
        {
            // ==========================================
            // 1. LOAD REQUEST, PATIENT AND DOCTOR
            // ==========================================

            var request = await _context.TestRequests
                .AsNoTracking()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestId == testRequestId);

            if (request == null ||
                request.Status !=
                    TestRequestStatus.Completed.ToString())
            {
                return false;
            }

            if (request.Doctor == null ||
                string.IsNullOrWhiteSpace(request.Doctor.Email) ||
                request.Patient == null)
            {
                return false;
            }

            // ==========================================
            // 2. LOAD ALL REQUESTED TESTS
            // ==========================================

            var requestedTests =
                await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .Where(x =>
                        x.TestRequestId == testRequestId)
                    .ToListAsync();

            if (requestedTests.Count == 0 ||
                requestedTests.Any(x =>
                    x.Status != "Verified"))
            {
                return false;
            }

            // ==========================================
            // 3. LOAD ALL VERIFIED RESULTS
            // ==========================================

            var testIds = requestedTests
                .Select(x => x.TestRequestTestTypeId)
                .ToList();

            var results = await _context.TestResults
                .AsNoTracking()
                .Include(x => x.Technician)
                .Include(x => x.VerifiedByTechnician)
                .Where(x =>
                    testIds.Contains(
                        x.TestRequestTestTypeId))
                .OrderBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            // Each requested test must have exactly one
            // saved, independently verified result.
            if (results.Count != requestedTests.Count)
                return false;

            foreach (var test in requestedTests)
            {
                var matching = results
                    .Where(x =>
                        x.TestRequestTestTypeId ==
                        test.TestRequestTestTypeId)
                    .ToList();

                if (matching.Count != 1)
                    return false;

                var result = matching[0];

                if (string.IsNullOrWhiteSpace(result.ResultValue) ||
                    string.IsNullOrWhiteSpace(
                        result.VerifiedByTechnicianId) ||
                    !result.VerifiedDateTime.HasValue ||
                    result.RequiresReview ||
                    string.IsNullOrWhiteSpace(test.TechnicianId) ||
                    result.TechnicianId != test.TechnicianId ||
                    result.VerifiedByTechnicianId ==
                        result.TechnicianId)
                {
                    return false;
                }
            }

            // ==========================================
            // 4. LOCATE PENDING EMAIL
            // ==========================================

            var email = await _context.TechnicianSentEmails
                .FirstOrDefaultAsync(x =>
                    x.TestRequestId == testRequestId &&
                    x.Status == "Pending");

            if (email == null)
                return false;

            // ==========================================
            // 5. GENERATE VERIFIED PDF
            // ==========================================

            var patientName =
                request.Patient.FullName;

            var doctorName =
                request.Doctor.FullName;

            var pdfBytes =
                _pdfService.GenerateLaboratoryReport(
                    testRequestId,
                    patientName,
                    doctorName,
                    results);

            // ==========================================
            // 6. UPDATE OUTGOING EMAIL RECORD
            // ==========================================

            email.RecipientEmail = request.Doctor.Email;

            email.Subject =
                $"Verified Laboratory Results - {request.RequestNumber}";

            email.Body =
                "Your requested laboratory results have been " +
                "verified. Please find the laboratory report attached.";

            email.PdfFileName =
                $"LaboratoryReport_{request.RequestNumber}.pdf";

            email.PdfAttachment = pdfBytes;

            // Still pending. SMTP delivery is not enabled.
            email.Status = "Pending";

            email.ErrorMessage = null;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
