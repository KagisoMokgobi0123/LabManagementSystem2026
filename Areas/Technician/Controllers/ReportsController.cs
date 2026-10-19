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
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue ||
                startDate.Value == default || endDate.Value == default ||
                endDate.Value.Date < startDate.Value.Date ||
                endDate.Value.Date >= DateTime.MaxValue.Date)
            {
                TempData["ErrorMessage"] = "Please select a valid start and end date.";
                return RedirectToAction(nameof(Index));
            }

            var technician = await _userManager.GetUserAsync(User);
            if (technician == null)
                return Challenge();

            // CompletionDateTime values are stored in UTC. The end date is inclusive.
            var from = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
            var until = DateTime.SpecifyKind(endDate.Value.Date.AddDays(1), DateTimeKind.Utc);

            try
            {
                var tests = await _context.TestRequestTestTypes
                    .AsNoTracking()
                    .Include(x => x.TestType)
                        .ThenInclude(t => t!.TestCategory)
                    .Include(x => x.TestRequest)
                    .Where(x =>
                        x.TechnicianId == technician.Id &&
                        x.CompletionDateTime.HasValue &&
                        x.CompletionDateTime.Value >= from &&
                        x.CompletionDateTime.Value < until &&
                        (x.Status == "Completed" || x.Status == "Verified"))
                    .OrderBy(x => x.CompletionDateTime)
                    .ToListAsync();

                var categories = tests
                    .GroupBy(x => x.TestType?.TestCategory?.Description ?? "Uncategorized")
                    .OrderBy(x => x.Key)
                    .ToList();

                var generatedAt = DateTime.UtcNow;

                // Set QuestPDF.Settings.License in Program.cs once, according to eligibility.
                byte[] pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.DefaultTextStyle(style => style.FontSize(10));

                        page.Header().Column(header =>
                        {
                            header.Item().Text("NMB-HLabSys")
                                .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                            header.Item().Text("Technician Completed Tests Report")
                                .FontSize(14).SemiBold();
                            header.Item().Text($"Technician: {technician.FullName}");
                            header.Item().Text($"Period (UTC): {from:dd MMM yyyy} - {endDate.Value:dd MMM yyyy}");
                            header.Item().Text($"Generated (UTC): {generatedAt:dd MMM yyyy HH:mm}");
                            header.Item().PaddingTop(10).LineHorizontal(1);
                        });

                        page.Content().PaddingVertical(15).Column(column =>
                        {
                            column.Spacing(12);
                            column.Item().Text($"Total Completed Tests: {tests.Count}")
                                .Bold().FontSize(13);

                            if (tests.Count == 0)
                            {
                                column.Item().Text("No completed tests were found for the selected date range.");
                            }

                            foreach (var category in categories)
                            {
                                column.Item().Text($"{category.Key} ({category.Count()} tests)")
                                    .Bold().FontSize(12).FontColor(Colors.Blue.Darken2);

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
                                        foreach (var heading in new[] { "Test Type", "Request #", "Completed (UTC)", "Status" })
                                        {
                                            header.Cell().Background(Colors.Blue.Lighten4)
                                                .Padding(5).Text(heading).Bold();
                                        }
                                    });

                                    foreach (var test in category)
                                    {
                                        var values = new[]
                                        {
                                            test.TestType?.Name ?? "Unknown",
                                            test.TestRequest?.RequestNumber ?? "N/A",
                                            test.CompletionDateTime?.ToString("dd MMM yyyy HH:mm") ?? "N/A",
                                            test.Status ?? "Unknown"
                                        };

                                        foreach (var value in values)
                                        {
                                            table.Cell().BorderBottom(1)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Padding(5).Text(value);
                                        }
                                    }
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("NMB-HLabSys | Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf();

                _logger.LogInformation(
                    "Technician report generated for user {TechnicianId}: {Count} tests, {From} to {End}",
                    technician.Id, tests.Count, from, endDate.Value.Date);

                return File(pdf, "application/pdf",
                    $"TechnicianReport_{from:yyyyMMdd}_{endDate.Value:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to generate technician PDF for {TechnicianId}, {Start} to {End}",
                    technician.Id, from, endDate.Value.Date);
                TempData["ErrorMessage"] = "The PDF report could not be generated. Please check the application logs for details.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
