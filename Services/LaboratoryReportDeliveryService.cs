
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Services
{
    public class LaboratoryReportDeliveryService
    {
        private readonly ApplicationDbContext _context;
        private readonly LaboratoryPdfService _pdfService;
        private readonly ILogger<LaboratoryReportDeliveryService> _logger;

        public LaboratoryReportDeliveryService(
            ApplicationDbContext context,
            LaboratoryPdfService pdfService,
            ILogger<LaboratoryReportDeliveryService> logger)
        {
            _context = context;
            _pdfService = pdfService;
            _logger = logger;
        }

        // ==========================================
        // PREPARE REPORT FOR CLAIMED OR PENDING EMAIL
        // ==========================================

        public async Task<bool> PrepareReportForEmailAsync(
            int sentEmailId,
            CancellationToken cancellationToken = default)
        {
            var email = await _context.TechnicianSentEmails
                .FirstOrDefaultAsync(x =>
                    x.SentEmailId == sentEmailId &&
                    (x.Status == "Pending" ||
                     x.Status == "Processing"),
                    cancellationToken);

            if (email == null)
            {
                return Block(
                    sentEmailId,
                    "Pending or processing email not found.");
            }

            var request = await _context.TestRequests
                .AsNoTracking()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestId == email.TestRequestId,
                    cancellationToken);

            if (request == null ||
                request.Patient == null ||
                request.Doctor == null ||
                string.IsNullOrWhiteSpace(request.Doctor.Email))
            {
                return Block(
                    sentEmailId,
                    "Request, patient or requesting doctor's email is missing.");
            }

            // The requesting doctor is the only intended recipient.
            email.RecipientEmail =
                request.Doctor.Email.Trim();

            // ==========================================
            // 1. PRELIMINARY INDIVIDUAL REPORT
            // ==========================================

            if (email.ReportType == "Preliminary")
            {
                if (!email.TestRequestTestTypeId.HasValue)
                {
                    return Block(
                        sentEmailId,
                        "Preliminary email has no individual test ID.");
                }

                int testId =
                    email.TestRequestTestTypeId.Value;

                var test = await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.TestRequestTestTypeId == testId &&
                        x.TestRequestId == request.TestRequestId &&
                        !x.IsDeleted,
                        cancellationToken);

                if (test == null ||
                    test.Status != "Completed")
                {
                    return Block(
                        sentEmailId,
                        "Individual test is not currently Completed.");
                }

                var results = await _context.TestResults
                    .AsNoTracking()
                    .Where(x =>
                        x.TestRequestTestTypeId == testId)
                    .ToListAsync(cancellationToken);

                // Preserve the existing single-result preliminary
                // PDF contract. Multi-measurement preliminary reports
                // require a separate PDF method update.
                if (results.Count != 1)
                {
                    return Block(
                        sentEmailId,
                        "Individual preliminary report requires exactly one result.");
                }

                var result = results[0];

                if (string.IsNullOrWhiteSpace(result.ResultValue) ||
                    IsDemoResult(result.ResultValue) ||
                    string.IsNullOrWhiteSpace(test.TechnicianId) ||
                    result.TechnicianId != test.TechnicianId ||
                    result.RequiresReview ||
                    !string.IsNullOrWhiteSpace(
                        result.VerifiedByTechnicianId) ||
                    result.VerifiedDateTime.HasValue)
                {
                    return Block(
                        sentEmailId,
                        "Result is invalid, under review or already verified.");
                }

                email.Subject =
                    $"PRELIMINARY - Unverified Laboratory Result - {request.RequestNumber}";

                email.Body =
                    "Dear Doctor,\n\n" +
                    "An individual laboratory test has been completed. " +
                    "Its preliminary result is attached.\n\n" +
                    "IMPORTANT: THIS RESULT HAS NOT BEEN INDEPENDENTLY VERIFIED. " +
                    "A verified report may follow after independent verification.\n\n" +
                    "Kind regards,\nNMB-HLabSys Laboratory";

                email.PdfFileName =
                    $"Preliminary_Test_{testId}_Request_{request.TestRequestId}.pdf";

                email.PdfAttachment =
                    _pdfService.GeneratePreliminaryReport(
                        request.TestRequestId,
                        testId,
                        request.Patient.FullName,
                        request.Doctor.FullName,
                        result);
            }

            // ==========================================
            // 2. FINAL COMBINED VERIFIED REPORT
            // ==========================================

            else if (email.ReportType == "Final")
            {
                if (email.TestRequestTestTypeId.HasValue)
                {
                    return Block(
                        sentEmailId,
                        "Combined report must not target an individual test.");
                }

                // Demo requests must never be released as genuine patient reports.
                if (request.RequestNumber.StartsWith("NMB-REQ-DEMO-", StringComparison.OrdinalIgnoreCase))
                {
                    return Block(sentEmailId, "Demonstration requests cannot be emailed as clinical reports.");
                }

                if (request.Status !=
                    TestRequestStatus.Completed.ToString())
                {
                    return Block(
                        sentEmailId,
                        "Request is not Completed.");
                }

                // ==========================================
                // LOAD ALL ACTIVE TESTS
                // ==========================================

                var tests = await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .Where(x =>
                        x.TestRequestId ==
                            request.TestRequestId &&
                        !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                // STRICT RULE:
                // Every active test must be Verified.
                if (tests.Count == 0 ||
                    tests.Any(x => x.Status != "Verified"))
                {
                    return Block(
                        sentEmailId,
                        "All active tests must be independently verified before preparing a final report.");
                }

                // ==========================================
                // LOAD ALL RESULT MEASUREMENTS
                // ==========================================

                var testIds = tests
                    .Select(x => x.TestRequestTestTypeId)
                    .ToList();

                var results = await _context.TestResults
                    .AsNoTracking()
                    .Include(x => x.Technician)
                    .Include(x => x.VerifiedByTechnician)
                    .Where(x =>
                        testIds.Contains(
                            x.TestRequestTestTypeId))
                    .OrderBy(x =>
                        x.TestRequestTestTypeId)
                    .ThenBy(x => x.TestResultId)
                    .ToListAsync(cancellationToken);

                // ==========================================
                // VALIDATE EACH TEST AND MEASUREMENT
                // ==========================================

                foreach (var test in tests)
                {
                    var measurements = results
                        .Where(x =>
                            x.TestRequestTestTypeId ==
                            test.TestRequestTestTypeId)
                        .ToList();

                    if (measurements.Count == 0)
                    {
                        return Block(
                            sentEmailId,
                            $"Test {test.TestRequestTestTypeId} has no saved measurements.");
                    }

                    if (string.IsNullOrWhiteSpace(
                        test.TechnicianId))
                    {
                        return Block(
                            sentEmailId,
                            $"Test {test.TestRequestTestTypeId} has no assigned technician.");
                    }

                    foreach (var result in measurements)
                    {
                        if (string.IsNullOrWhiteSpace(
                                result.ResultValue) ||
                            IsDemoResult(result.ResultValue))
                        {
                            return Block(
                                sentEmailId,
                                $"Test {test.TestRequestTestTypeId} contains an invalid measurement.");
                        }

                        if (result.TechnicianId !=
                            test.TechnicianId)
                        {
                            return Block(
                                sentEmailId,
                                $"Test {test.TestRequestTestTypeId} contains a result from a different technician.");
                        }

                        if (result.RequiresReview)
                        {
                            return Block(
                                sentEmailId,
                                $"Test {test.TestRequestTestTypeId} still requires review.");
                        }

                        if (string.IsNullOrWhiteSpace(
                                result.VerifiedByTechnicianId) ||
                            !result.VerifiedDateTime.HasValue)
                        {
                            return Block(
                                sentEmailId,
                                $"Test {test.TestRequestTestTypeId} contains an unverified measurement.");
                        }

                        if (result.VerifiedByTechnicianId ==
                            result.TechnicianId)
                        {
                            return Block(
                                sentEmailId,
                                $"Test {test.TestRequestTestTypeId} contains a self-verified measurement.");
                        }
                    }
                }

                // ==========================================
                // BUILD FINAL VERIFIED REPORT
                // ==========================================

                email.Subject =
                    $"Verified Laboratory Results - {request.RequestNumber}";

                email.Body =
                    "Dear Doctor,\n\n" +
                    "All active tests on your laboratory request " +
                    "have been independently verified.\n\n" +
                    "The combined final laboratory report is attached.\n\n" +
                    "Kind regards,\nNMB-HLabSys Laboratory";

                email.PdfFileName =
                    $"LaboratoryReport_{request.TestRequestId}.pdf";

                email.PdfAttachment =
                    _pdfService.GenerateLaboratoryReport(
                        request.TestRequestId,
                        request.Patient.FullName,
                        request.Doctor.FullName,
                        results);
            }
            else
            {
                return Block(
                    sentEmailId,
                    $"Unknown report type '{email.ReportType}'.");
            }

            // ==========================================
            // 3. VALIDATE GENERATED PDF
            // ==========================================

            if (email.PdfAttachment == null ||
                email.PdfAttachment.Length == 0)
            {
                return Block(
                    sentEmailId,
                    "PDF generation returned no data.");
            }

            // ==========================================
            // 4. SAVE PREPARED EMAIL
            // ==========================================

            email.ErrorMessage = null;

            await _context.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "Prepared {ReportType} report for email {EmailId}.",
                email.ReportType,
                sentEmailId);

            return true;
        }

        // ==========================================
        // PREPARE FINAL REPORT BY REQUEST ID
        // ==========================================

        public async Task<bool> PrepareReportAsync(
            int testRequestId)
        {
            var emailId = await _context.TechnicianSentEmails
                .AsNoTracking()
                .Where(x =>
                    x.TestRequestId == testRequestId &&
                    x.ReportType == "Final" &&
                    x.Status == "Pending")
                .OrderBy(x => x.SentEmailId)
                .Select(x => x.SentEmailId)
                .FirstOrDefaultAsync();

            return emailId != 0 &&
                await PrepareReportForEmailAsync(emailId);
        }

        // ==========================================
        // HELPER: DEMONSTRATION RESULT DETECTION
        // ==========================================

        private static bool IsDemoResult(string value)
        {
            var normalized = value.Trim();
            return normalized.Contains("DEMO - NO CLINICAL RESULT", StringComparison.OrdinalIgnoreCase)
                || normalized.Contains("SIMULATED TRAINING DATA", StringComparison.OrdinalIgnoreCase)
                || normalized.Contains("NOT FOR CLINICAL USE", StringComparison.OrdinalIgnoreCase);
        }

        // ==========================================
        // HELPER: BLOCK INVALID REPORT PREPARATION
        // ==========================================

        private bool Block(
            int emailId,
            string reason)
        {
            _logger.LogWarning(
                "Report preparation blocked for email {EmailId}: {Reason}",
                emailId,
                reason);

            return false;
        }
    }
}
