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

        public async Task<bool> PrepareReportForEmailAsync(
            int sentEmailId, CancellationToken cancellationToken = default)
        {
            var email = await _context.TechnicianSentEmails
                .FirstOrDefaultAsync(x => x.SentEmailId == sentEmailId &&
                                          x.Status == "Pending", cancellationToken);
            if (email == null)
                return Block(sentEmailId, "Pending email not found.");

            var request = await _context.TestRequests.AsNoTracking()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x => x.TestRequestId == email.TestRequestId,
                    cancellationToken);

            if (request == null || request.Patient == null || request.Doctor == null ||
                string.IsNullOrWhiteSpace(request.Doctor.Email))
                return Block(sentEmailId, "Request, patient or requesting doctor's email is missing.");

            email.RecipientEmail = request.Doctor.Email.Trim();

            if (email.ReportType == "Preliminary")
            {
                if (!email.TestRequestTestTypeId.HasValue)
                    return Block(sentEmailId, "Preliminary email has no individual test ID.");

                int testId = email.TestRequestTestTypeId.Value;
                var test = await _context.TestRequestTestTypes.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TestRequestTestTypeId == testId &&
                                              x.TestRequestId == request.TestRequestId &&
                                              !x.IsDeleted, cancellationToken);

                if (test == null || test.Status != "Completed")
                    return Block(sentEmailId, "Individual test is not currently Completed.");

                var results = await _context.TestResults.AsNoTracking()
                    .Where(x => x.TestRequestTestTypeId == testId)
                    .ToListAsync(cancellationToken);

                if (results.Count != 1)
                    return Block(sentEmailId, "Individual test must have exactly one result.");

                var result = results[0];
                if (string.IsNullOrWhiteSpace(result.ResultValue) ||
                    result.TechnicianId != test.TechnicianId ||
                    result.RequiresReview ||
                    !string.IsNullOrWhiteSpace(result.VerifiedByTechnicianId) ||
                    result.VerifiedDateTime.HasValue)
                    return Block(sentEmailId, "Result is invalid, under review or already verified.");

                email.Subject = $"PRELIMINARY - Unverified Laboratory Result - {request.RequestNumber}";
                email.Body =
                    "Dear Doctor,\n\n" +
                    "An individual laboratory test has been completed. " +
                    "Its preliminary result is attached.\n\n" +
                    "IMPORTANT: THIS RESULT HAS NOT BEEN INDEPENDENTLY VERIFIED. " +
                    "A final verified report will follow after verification.\n\n" +
                    "Kind regards,\nNMB-HLabSys Laboratory";
                email.PdfFileName = $"Preliminary_Test_{testId}_Request_{request.TestRequestId}.pdf";
                if (email.PdfAttachment == null || email.PdfAttachment.Length == 0)
                    email.PdfAttachment = _pdfService.GeneratePreliminaryReport(
                        request.TestRequestId, testId, request.Patient.FullName,
                        request.Doctor.FullName, result);
            }
            else if (email.ReportType == "Final")
            {
                if (email.TestRequestTestTypeId.HasValue)
                    return Block(sentEmailId, "Final report must not target an individual test.");

                if (request.Status != TestRequestStatus.Completed.ToString())
                    return Block(sentEmailId, "Request is not Completed.");

                var tests = await _context.TestRequestTestTypes.AsNoTracking()
                    .Where(x => x.TestRequestId == request.TestRequestId && !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (tests.Count == 0 || tests.Any(x => x.Status != "Verified"))
                    return Block(sentEmailId, "Not all active tests are Verified.");

                var testIds = tests.Select(x => x.TestRequestTestTypeId).ToList();
                var results = await _context.TestResults.AsNoTracking()
                    .Include(x => x.Technician)
                    .Include(x => x.VerifiedByTechnician)
                    .Where(x => testIds.Contains(x.TestRequestTestTypeId))
                    .OrderBy(x => x.TestRequestTestTypeId)
                    .ToListAsync(cancellationToken);

                if (results.Count != tests.Count)
                    return Block(sentEmailId, "Final report has an incorrect result count.");

                foreach (var test in tests)
                {
                    var matching = results.Where(x =>
                        x.TestRequestTestTypeId == test.TestRequestTestTypeId).ToList();
                    if (matching.Count != 1)
                        return Block(sentEmailId, "A test has missing or duplicate results.");

                    var result = matching[0];
                    if (string.IsNullOrWhiteSpace(result.ResultValue) ||
                        string.IsNullOrWhiteSpace(test.TechnicianId) ||
                        result.TechnicianId != test.TechnicianId ||
                        string.IsNullOrWhiteSpace(result.VerifiedByTechnicianId) ||
                        !result.VerifiedDateTime.HasValue ||
                        result.VerifiedByTechnicianId == result.TechnicianId ||
                        result.RequiresReview)
                        return Block(sentEmailId, "A result failed independent verification checks.");
                }

                email.Subject = $"Verified Laboratory Results - {request.RequestNumber}";
                email.Body =
                    "Dear Doctor,\n\n" +
                    "All active tests on your laboratory request have been independently " +
                    "verified. The combined final report is attached.\n\n" +
                    "Kind regards,\nNMB-HLabSys Laboratory";
                email.PdfFileName = $"LaboratoryReport_{request.TestRequestId}.pdf";
                if (email.PdfAttachment == null || email.PdfAttachment.Length == 0)
                    email.PdfAttachment = _pdfService.GenerateLaboratoryReport(
                        request.TestRequestId, request.Patient.FullName,
                        request.Doctor.FullName, results);
            }
            else
            {
                return Block(sentEmailId, $"Unknown report type '{email.ReportType}'.");
            }

            if (email.PdfAttachment == null || email.PdfAttachment.Length == 0)
                return Block(sentEmailId, "PDF generation returned no data.");

            email.ErrorMessage = null;
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Prepared {ReportType} report for email {EmailId}.",
                email.ReportType, sentEmailId);
            return true;
        }

        // Retain compatibility with callers that still use the old method.
        public async Task<bool> PrepareReportAsync(int testRequestId)
        {
            var emailId = await _context.TechnicianSentEmails.AsNoTracking()
                .Where(x => x.TestRequestId == testRequestId &&
                            x.ReportType == "Final" && x.Status == "Pending")
                .Select(x => x.SentEmailId)
                .FirstOrDefaultAsync();
            return emailId != 0 && await PrepareReportForEmailAsync(emailId);
        }

        private bool Block(int emailId, string reason)
        {
            _logger.LogWarning("Report preparation blocked for email {EmailId}: {Reason}",
                emailId, reason);
            return false;
        }
    }
}
