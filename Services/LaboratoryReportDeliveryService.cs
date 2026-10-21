
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

        public async Task<bool> PrepareReportAsync(int testRequestId)
        {
            // 1. Load the laboratory request.
            var request = await _context.TestRequests
                .AsNoTracking()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x =>
                    x.TestRequestId == testRequestId);

            if (request == null)
                return Block(testRequestId, "Request not found.");

            if (request.Status !=
                TestRequestStatus.Completed.ToString())
            {
                return Block(
                    testRequestId,
                    $"Request status is '{request.Status}', not Completed.");
            }

            if (request.Patient == null)
                return Block(testRequestId, "Patient record is missing.");

            if (request.Doctor == null)
                return Block(testRequestId, "Doctor record is missing.");

            if (string.IsNullOrWhiteSpace(request.Doctor.Email))
            {
                return Block(
                    testRequestId,
                    "Requesting doctor has no email address.");
            }

            // 2. Load every requested test.
            var requestedTests = await _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(x => x.TestRequestId == testRequestId)
                .ToListAsync();

            if (requestedTests.Count == 0)
                return Block(testRequestId, "No requested tests found.");

            foreach (var test in requestedTests)
            {
                if (test.Status != "Verified")
                {
                    return Block(
                        testRequestId,
                        $"Test {test.TestRequestTestTypeId} " +
                        $"has status '{test.Status}', not Verified.");
                }
            }

            // 3. Load saved results.
            var testIds = requestedTests
                .Select(x => x.TestRequestTestTypeId)
                .ToList();

            var results = await _context.TestResults
                .AsNoTracking()
                .Include(x => x.Technician)
                .Include(x => x.VerifiedByTechnician)
                .Where(x =>
                    testIds.Contains(x.TestRequestTestTypeId))
                .OrderBy(x => x.TestRequestTestTypeId)
                .ToListAsync();

            if (results.Count != requestedTests.Count)
            {
                return Block(
                    testRequestId,
                    $"Expected {requestedTests.Count} results, " +
                    $"but found {results.Count}.");
            }

            foreach (var test in requestedTests)
            {
                var matching = results
                    .Where(x =>
                        x.TestRequestTestTypeId ==
                        test.TestRequestTestTypeId)
                    .ToList();

                if (matching.Count != 1)
                {
                    return Block(
                        testRequestId,
                        $"Test {test.TestRequestTestTypeId} " +
                        $"has {matching.Count} saved results; expected 1.");
                }

                var result = matching[0];
                var testId = test.TestRequestTestTypeId;

                if (string.IsNullOrWhiteSpace(result.ResultValue))
                    return Block(testRequestId,
                        $"Test {testId}: Result value is missing.");

                if (string.IsNullOrWhiteSpace(
                    result.VerifiedByTechnicianId))
                {
                    return Block(testRequestId,
                        $"Test {testId}: Verifying technician is missing.");
                }

                if (!result.VerifiedDateTime.HasValue)
                    return Block(testRequestId,
                        $"Test {testId}: Verification date is missing.");

                if (result.RequiresReview)
                    return Block(testRequestId,
                        $"Test {testId}: Result still requires review.");

                if (string.IsNullOrWhiteSpace(test.TechnicianId))
                    return Block(testRequestId,
                        $"Test {testId}: Assigned technician is missing.");

                if (result.TechnicianId != test.TechnicianId)
                    return Block(testRequestId,
                        $"Test {testId}: Result technician does not match assigned technician.");

                if (result.VerifiedByTechnicianId ==
                    result.TechnicianId)
                {
                    return Block(testRequestId,
                        $"Test {testId}: Result was not independently verified.");
                }
            }

            // 4. Locate the pending email record.
            var email = await _context.TechnicianSentEmails
                .FirstOrDefaultAsync(x =>
                    x.TestRequestId == testRequestId &&
                    x.Status == "Pending");

            if (email == null)
                return Block(testRequestId,
                    "No Pending outgoing email record exists.");

            // 5. Generate the combined laboratory PDF.
            var pdfBytes = _pdfService.GenerateLaboratoryReport(
                testRequestId,
                request.Patient.FullName,
                request.Doctor.FullName,
                results);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return Block(testRequestId,
                    "PDF generation returned an empty document.");

            // 6. Prepare the outgoing email record.
            email.RecipientEmail = request.Doctor.Email;

            email.Subject =
                $"Verified Laboratory Results - {request.RequestNumber}";

            email.Body =
                "Your requested laboratory results have been " +
                "verified. Please find the laboratory report attached.";

            email.PdfFileName =
                $"LaboratoryReport_{request.RequestNumber}.pdf";

            email.PdfAttachment = pdfBytes;
            email.Status = "Pending";
            email.ErrorMessage = null;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Laboratory PDF prepared successfully for request {RequestId}.",
                testRequestId);

            return true;
        }

        private bool Block(int testRequestId, string reason)
        {
            _logger.LogWarning(
                "Laboratory report preparation blocked for request {RequestId}: {Reason}",
                testRequestId,
                reason);

            return false;
        }
    }
}
