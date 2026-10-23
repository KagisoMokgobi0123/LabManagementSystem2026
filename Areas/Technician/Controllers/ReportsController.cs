
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ReportsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(
            DateTime? startDate,
            DateTime? endDate)
        {
            if (!startDate.HasValue ||
                !endDate.HasValue ||
                startDate.Value == default ||
                endDate.Value == default ||
                endDate.Value.Date < startDate.Value.Date ||
                endDate.Value.Date >= DateTime.MaxValue.Date)
            {
                TempData["ErrorMessage"] =
                    "Please select a valid start and end date.";

                return RedirectToAction(nameof(Index));
            }

            var technician = await _userManager.GetUserAsync(User);

            if (technician == null)
                return Challenge();

            var from = DateTime.SpecifyKind(
                startDate.Value.Date,
                DateTimeKind.Utc);

            var until = DateTime.SpecifyKind(
                endDate.Value.Date.AddDays(1),
                DateTimeKind.Utc);

            try
            {
                // Technician activity is counted per individual test.
                // The parent request may still be In Progress while other
                // tests are awaiting completion or verification.
                // Diagnostic for test #1001. This is logged server-side only.
                // It distinguishes an outdated deployment from a filter mismatch.
                var test1001 = await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .Where(x => x.TestRequestTestTypeId == 1001)
                    .Select(x => new
                    {
                        x.TestRequestTestTypeId,
                        x.TechnicianId,
                        x.Status,
                        x.CompletionDateTime,
                        ParentStatus = x.TestRequest != null
                            ? x.TestRequest.Status : null
                    })
                    .FirstOrDefaultAsync();

                _logger.LogWarning(
                    "REPORT-V2-20261023: Test1001={@Test1001}; LoggedInTechnicianId={TechnicianId}; From={From}; Until={Until}",
                    test1001, technician.Id, from, until);

                var tests = await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)
                    .Include(x => x.TestRequest)
                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        x.TestRequest != null &&
                        x.CompletionDateTime.HasValue &&
                        x.CompletionDateTime.Value >= from &&
                        x.CompletionDateTime.Value < until &&
                        (x.Status == "Completed" ||
                         x.Status == "Verified"))
                    .OrderBy(x => x.CompletionDateTime)
                    .ToListAsync();

                _logger.LogWarning(
                    "REPORT-V2-20261023: Query returned {Count} matching tests for technician {TechnicianId}",
                    tests.Count, technician.Id);

                var testIds = tests
                    .Select(x => x.TestRequestTestTypeId)
                    .ToList();

                // Verify actual saved result records rather than
                // relying only on a test's status label.
                var verifiedResultIds = await _context.TestResults
                    .AsNoTracking()
                    .Where(r =>
                        testIds.Contains(r.TestRequestTestTypeId) &&
                        r.ResultValue != null &&
                        r.ResultValue != "" &&
                        r.VerifiedByTechnicianId != null &&
                        r.VerifiedByTechnicianId != "" &&
                        r.VerifiedDateTime.HasValue)
                    .Select(r => r.TestRequestTestTypeId)
                    .Distinct()
                    .ToListAsync();

                var verifiedIdSet = verifiedResultIds.ToHashSet();

                var verifiedCount = tests.Count(x =>
                    x.Status == "Verified" &&
                    verifiedIdSet.Contains(x.TestRequestTestTypeId));

                var completedCount = tests.Count(x =>
                    x.Status == "Completed");

                var missingVerificationCount = tests.Count(x =>
                    x.Status == "Verified" &&
                    !verifiedIdSet.Contains(x.TestRequestTestTypeId));

                var categories = tests
                    .GroupBy(x =>
                        x.TestType?.TestCategory?.Description
                        ?? "Uncategorized")
                    .OrderBy(x => x.Key)
                    .ToList();

                var generatedAt = DateTime.UtcNow;

                byte[] pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);

                        page.DefaultTextStyle(style =>
                            style.FontSize(10));

                        page.Header().Column(header =>
                        {
                            header.Spacing(5);

                            header.Item()
                                .Text("NMB-HLabSys")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Darken3);

                            header.Item()
                                .Text("Technician Completed Tests Report - V2")
                                .FontSize(14)
                                .SemiBold();

                            header.Item()
                                .Text($"Technician: {technician.FullName}");

                            header.Item()
                                .Text(
                                    $"Period (UTC): {from:dd MMM yyyy} - " +
                                    $"{endDate.Value:dd MMM yyyy}");

                            header.Item()
                                .Text(
                                    $"Generated (UTC): " +
                                    $"{generatedAt:dd MMM yyyy HH:mm}");

                            header.Item()
                                .Text(
                                    "Scope: Individual completed or verified tests performed by this technician | REPORT-V2-20261023")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken2);

                            header.Item()
                                .PaddingTop(10)
                                .LineHorizontal(1);
                        });

                        page.Content()
                            .PaddingVertical(15)
                            .Column(column =>
                            {
                                column.Spacing(12);

                                column.Item()
                                    .Text("REPORT SUMMARY")
                                    .Bold()
                                    .FontSize(13)
                                    .FontColor(Colors.Blue.Darken3);

                                column.Item()
                                    .Background(Colors.Grey.Lighten4)
                                    .Padding(12)
                                    .Column(summary =>
                                    {
                                        summary.Spacing(5);

                                        summary.Item()
                                            .Text(
                                                $"Total Tests: {tests.Count}");

                                        summary.Item()
                                            .Text(
                                                $"Completed (not verified): " +
                                                $"{completedCount}");

                                        summary.Item()
                                            .Text(
                                                $"Verified with recorded results: " +
                                                $"{verifiedCount}");

                                        summary.Item()
                                            .Text(
                                                $"Marked Verified without valid " +
                                                $"verification record: " +
                                                $"{missingVerificationCount}");
                                    });

                                if (tests.Count == 0)
                                {
                                    column.Item()
                                        .Text(
                                            "REPORT-V2-20261023: No completed or verified tests performed by " +
                                            "this technician were found for the selected date range.");
                                }

                                foreach (var category in categories)
                                {
                                    column.Item()
                                        .Text(
                                            $"{category.Key} " +
                                            $"({category.Count()} tests)")
                                        .Bold()
                                        .FontSize(12)
                                        .FontColor(Colors.Blue.Darken2);

                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(cols =>
                                        {
                                            cols.RelativeColumn(3);
                                            cols.RelativeColumn(2);
                                            cols.RelativeColumn(2);
                                            cols.RelativeColumn(2);
                                        });

                                        table.Header(header =>
                                        {
                                            foreach (var heading in new[]
                                            {
                                            "Test Type",
                                            "Request #",
                                            "Completed (UTC)",
                                            "Verification"
                                        })
                                            {
                                                header.Cell()
                                                    .Background(
                                                        Colors.Blue.Lighten4)
                                                    .Padding(5)
                                                    .Text(heading)
                                                    .Bold()
                                                    .FontSize(8);
                                            }
                                        });

                                        foreach (var test in category)
                                        {
                                            string verificationStatus;

                                            if (test.Status == "Verified")
                                            {
                                                verificationStatus =
                                                    verifiedIdSet.Contains(
                                                        test.TestRequestTestTypeId)
                                                    ? "Verified"
                                                    : "Missing verification";
                                            }
                                            else
                                            {
                                                verificationStatus =
                                                    "Completed";
                                            }

                                            var values = new[]
                                            {
                                            test.TestType?.Name ?? "Unknown",
                                            test.TestRequest?.RequestNumber
                                                ?? "N/A",
                                            test.CompletionDateTime?
                                                .ToString("dd MMM yyyy HH:mm")
                                                ?? "N/A",
                                            verificationStatus
                                        };

                                            foreach (var value in values)
                                            {
                                                table.Cell()
                                                    .BorderBottom(1)
                                                    .BorderColor(
                                                        Colors.Grey.Lighten2)
                                                    .Padding(5)
                                                    .Text(value)
                                                    .FontSize(8);
                                            }
                                        }
                                    });
                                }

                                column.Item()
                                    .PaddingTop(12)
                                    .Text(
                                        "This document is a technician activity " +
                                        "summary, not a patient laboratory " +
                                        "results report. Verification is " +
                                        "identified from recorded result " +
                                        "metadata and test status.")
                                    .Italic()
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Darken2);
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("NMB-HLabSys | Page ");
                                text.CurrentPageNumber();
                                text.Span(" of ");
                                text.TotalPages();
                            });
                    });
                }).GeneratePdf();

                _logger.LogInformation(
                    "Technician activity PDF generated for " +
                    "{TechnicianId}: {Total} tests, " +
                    "{Verified} verified, {Missing} missing " +
                    "verification records, {From} to {End}",
                    technician.Id,
                    tests.Count,
                    verifiedCount,
                    missingVerificationCount,
                    from,
                    endDate.Value.Date);

                Response.Headers.CacheControl = "no-store";
                Response.Headers["X-Content-Type-Options"] =
                    "nosniff";

                return File(
                    pdf,
                    "application/pdf",
                    $"TechnicianReport_{from:yyyyMMdd}_" +
                    $"{endDate.Value:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to generate technician PDF for " +
                    "{TechnicianId}, {Start} to {End}",
                    technician.Id,
                    from,
                    endDate.Value.Date);

                TempData["ErrorMessage"] =
                    "The PDF report could not be generated. " +
                    "Please check the application logs.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
