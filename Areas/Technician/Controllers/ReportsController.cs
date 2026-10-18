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

        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(DateTime startDate, DateTime endDate)
        {
            var technician = await _userManager.GetUserAsync(User);
            if (technician == null)
                return Challenge();

            if (startDate == default || endDate == default ||
                endDate.Date < startDate.Date ||
                endDate.Date >= DateTime.MaxValue.Date)
            {
                TempData["ErrorMessage"] = "Please select a valid date range.";
                return RedirectToAction(nameof(Index));
            }

            // CompletionDateTime is stored in UTC. Treat the selected dates as UTC dates.
            var from = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var until = DateTime.SpecifyKind(endDate.Date.AddDays(1), DateTimeKind.Utc);

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

            // Configure QuestPDF licensing once in Program.cs based on your eligibility.
            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(header =>
                    {
                        header.Item().Text("NMB-HLabSys")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                        header.Item().Text("Technician Completed Tests Report")
                            .FontSize(14).SemiBold();
                        header.Item().Text($"Technician: {technician.FullName}");
                        header.Item().Text($"Period (UTC): {from:dd MMM yyyy} - {endDate:dd MMM yyyy}");
                        header.Item().Text($"Generated (UTC): {DateTime.UtcNow:dd MMM yyyy HH:mm}");
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

            return File(pdf, "application/pdf",
                $"TechnicianReport_{from:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }
    }
}
