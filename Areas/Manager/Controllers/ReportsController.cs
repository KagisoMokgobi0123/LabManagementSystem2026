using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Composition;
using System.Configuration;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int TopCategoryLimit = 10;
        private const string CompletedStatus = "Completed";
        private readonly IConfiguration _configuration;

        public ReportsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate)
        {
            var today = DateTime.Today;

            var start = (startDate ??
                new DateTime(today.Year, today.Month, 1)).Date;

            var end = (endDate ?? today).Date;

            if (start > end)
            {
                TempData["ErrorMessage"] =
                    "The start date cannot be after the end date.";

                start = new DateTime(today.Year, today.Month, 1);
                end = today;
            }

            var report = await BuildReportData(start, end);

            return View(report);
        }


        // ============================================================
        // REPORT DATA FOR THE GRAPH
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetReportData(
            DateTime startDate,
            DateTime endDate)
        {
            if (startDate == default ||
                endDate == default ||
                startDate.Date > endDate.Date)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please provide a valid date range."
                });
            }

            var report = await BuildReportData(
                startDate.Date,
                endDate.Date);

            return Json(new
            {
                success = true,
                report.StartDate,
                report.EndDate,
                report.TotalCompletedTests,
                report.TotalCategories,
                report.CategoryBreakdown,
                report.ChartLabels,
                report.ChartSeries
            });
        }


        // ============================================================
        // GENERATE PDF
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GeneratePdf(
    DateTime startDate,
    DateTime endDate)
        {
            if (startDate == default ||
                endDate == default ||
                startDate.Date > endDate.Date)
            {
                TempData["ErrorMessage"] =
                    "Please select a valid reporting date range.";

                return RedirectToAction(nameof(Index));
            }

            var report = await BuildReportData(
                startDate.Date,
                endDate.Date);

            var totalTrackedTests =
                report.TotalCompletedTests + report.TotalInProgressTests;

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);

                    // Use Lato because it is registered in your application.
                    page.DefaultTextStyle(x =>
                        x.FontFamily("Lato").FontSize(9));

                    // HEADER
                    page.Header().Column(column =>
                    {
                        column.Item()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .Bold()
                            .FontSize(17)
                            .FontColor(Colors.Blue.Darken2);

                        column.Item()
                            .Text("Management Test Performance Report")
                            .SemiBold()
                            .FontSize(13);

                        column.Item()
                            .PaddingTop(5)
                            .Text(
                                $"Reporting period: {report.StartDate:dd MMM yyyy} - " +
                                $"{report.EndDate:dd MMM yyyy}");

                        column.Item()
                            .Text(
                                $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");

                        column.Item()
                            .PaddingTop(4)
                            .Text(
                                "Completed tests are grouped by completion date; " +
                                "in-progress tests are grouped by start date.")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(12);

                            column.Item()
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(12)
                                .Row(row =>
                                {
                                    row.RelativeItem().Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("Completed Tests")
                                            .FontSize(9);

                                        summary.Item()
                                            .Text(
                                                report.TotalCompletedTests.ToString("N0"))
                                            .Bold()
                                            .FontSize(20)
                                            .FontColor(Colors.Green.Darken2);
                                    });

                                    row.RelativeItem().Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("In Progress")
                                            .FontSize(9);

                                        summary.Item()
                                            .Text(
                                                report.TotalInProgressTests.ToString("N0"))
                                            .Bold()
                                            .FontSize(20)
                                            .FontColor(Colors.Orange.Darken2);
                                    });

                                    row.RelativeItem().Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("Categories")
                                            .FontSize(9);

                                        summary.Item()
                                            .Text(report.TotalCategories.ToString("N0"))
                                            .Bold()
                                            .FontSize(20)
                                            .FontColor(Colors.Blue.Darken2);
                                    });
                                });

                            column.Item()
                                .Text("Category Breakdown")
                                .Bold()
                                .FontSize(13);

                            if (report.CategoryBreakdown.Count == 0)
                            {
                                column.Item()
                                    .Padding(15)
                                    .Text(
                                        "No completed or in-progress tests were " +
                                        "recorded during the selected period.");
                            }
                            else
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1.2f);
                                        columns.RelativeColumn(1.2f);
                                        columns.RelativeColumn(1.1f);
                                        columns.RelativeColumn(1.2f);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell()
                                            .Element(HeaderCell)
                                            .Text("Test Category");

                                        header.Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("Completed");

                                        header.Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("In Progress");

                                        header.Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("Total");

                                        header.Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("% Total");
                                    });

                                    foreach (var category in report.CategoryBreakdown)
                                    {
                                        table.Cell()
                                            .Element(BodyCell)
                                            .Text(category.CategoryName);

                                        table.Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(category.CompletedTests.ToString("N0"));

                                        table.Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(category.InProgressTests.ToString("N0"));

                                        table.Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(category.TotalTests.ToString("N0"));

                                        table.Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text($"{category.Percentage:F1}%");
                                    }

                                    table.Cell()
                                        .Element(TotalCell)
                                        .Text("TOTAL");

                                    table.Cell()
                                        .Element(TotalCell)
                                        .AlignRight()
                                        .Text(report.TotalCompletedTests.ToString("N0"));

                                    table.Cell()
                                        .Element(TotalCell)
                                        .AlignRight()
                                        .Text(report.TotalInProgressTests.ToString("N0"));

                                    table.Cell()
                                        .Element(TotalCell)
                                        .AlignRight()
                                        .Text(totalTrackedTests.ToString("N0"));

                                    table.Cell()
                                        .Element(TotalCell)
                                        .AlignRight()
                                        .Text(totalTrackedTests > 0 ? "100.0%" : "0.0%");
                                });
                            }
                        });

                    // FOOTER
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem()
                            .Text("NMB Health Laboratory System")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);

                        row.RelativeItem()
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" of ");
                                text.TotalPages();
                            });
                    });
                });
            }).GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                $"TestPerformanceReport_{report.StartDate:yyyyMMdd}_" +
                $"{report.EndDate:yyyyMMdd}.pdf");
        }

        // ============================================================
        // BUILD REPORT DATA
        // ============================================================

        private async Task<ManagementReportViewModel> BuildReportData(
           DateTime startDate,
           DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            var endExclusive = end.AddDays(1);

            // --------------------------------------------------------
            // TEST ASSIGNMENTS STARTED WITHIN THE SELECTED PERIOD
            // Include only Completed and In Progress.
            // --------------------------------------------------------
            var reportTests = _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(x =>
                    (x.Status == "Completed" ||
                     x.Status == "In Progress") &&
                    x.StartDateTime.HasValue &&
                    x.StartDateTime.Value >= start &&
                    x.StartDateTime.Value < endExclusive &&
                    x.TestType != null &&
                    x.TestType.TestCategory != null);

            // --------------------------------------------------------
            // TOTALS BY STATUS
            // --------------------------------------------------------

            var totalCompletedTests = await reportTests
                .CountAsync(x => x.Status == "Completed");

            var totalInProgressTests = await reportTests
                .CountAsync(x => x.Status == "In Progress");

            var totalEligibleTests =
                totalCompletedTests + totalInProgressTests;

            // --------------------------------------------------------
            // CATEGORY TOTALS BY STATUS
            // --------------------------------------------------------

            var categoryTotals = await reportTests
                .GroupBy(x => new
                {
                    x.TestType!.TestCategoryId,
                    CategoryName = x.TestType.TestCategory!.Description
                })
                .Select(g => new CategoryReportItem
                {
                    TestCategoryId = g.Key.TestCategoryId,
                    CategoryName = g.Key.CategoryName,

                    CompletedTests = g.Count(
                        x => x.Status == "Completed"),

                    InProgressTests = g.Count(
                        x => x.Status == "In Progress")
                })
                .OrderByDescending(x => x.CompletedTests + x.InProgressTests)
                .ThenBy(x => x.CategoryName)
                .ToListAsync();

            // --------------------------------------------------------
            // CATEGORY COMPLETION PERCENTAGES
            // Completed / (Completed + In Progress)
            // --------------------------------------------------------

            foreach (var category in categoryTotals)
            {
                category.Percentage = category.TotalTests == 0
                    ? 0
                    : (decimal)category.CompletedTests /
                      category.TotalTests * 100;
            }

            // --------------------------------------------------------
            // TOP CATEGORIES FOR THE CHART
            // --------------------------------------------------------

            var topCategories = categoryTotals
                .Take(TopCategoryLimit)
                .ToList();

            var topCategoryIds = topCategories
                .Select(x => x.TestCategoryId)
                .ToList();

            // --------------------------------------------------------
            // DAILY COUNTS FOR TOP CATEGORIES
            // Count both statuses using StartDateTime.
            // --------------------------------------------------------

            var dailyCounts = await reportTests
                .Where(x =>
                    topCategoryIds.Contains(
                        x.TestType!.TestCategoryId))
                .GroupBy(x => new
                {
                    Date = x.StartDateTime!.Value.Date,
                    x.TestType!.TestCategoryId,
                    x.Status
                })
                .Select(g => new DailyCategoryCount
                {
                    Date = g.Key.Date,
                    TestCategoryId = g.Key.TestCategoryId,
                    TotalTests = g.Count()
                })
                .ToListAsync();

            // --------------------------------------------------------
            // REMAINING CATEGORIES
            // --------------------------------------------------------

            var otherCategoryIds = categoryTotals
                .Skip(TopCategoryLimit)
                .Select(x => x.TestCategoryId)
                .ToList();

            var otherDailyCounts = new List<DailyCategoryCount>();

            if (otherCategoryIds.Count > 0)
            {
                otherDailyCounts = await reportTests
                    .Where(x =>
                        otherCategoryIds.Contains(
                            x.TestType!.TestCategoryId))
                    .GroupBy(x => x.StartDateTime!.Value.Date)
                    .Select(g => new DailyCategoryCount
                    {
                        Date = g.Key,
                        TestCategoryId = 0,
                        TotalTests = g.Count()
                    })
                    .ToListAsync();
            }

            // --------------------------------------------------------
            // DATE LABELS
            // --------------------------------------------------------

            var chartLabels = new List<string>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                chartLabels.Add(date.ToString("dd MMM"));
            }

            // --------------------------------------------------------
            // TOP CATEGORY CHART SERIES
            // --------------------------------------------------------

            var chartSeries = new List<ChartSeriesViewModel>();

            foreach (var category in topCategories)
            {
                var countsByDate = dailyCounts
                    .Where(x =>
                        x.TestCategoryId == category.TestCategoryId)
                    .GroupBy(x => x.Date.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x => x.TotalTests));

                var series = new ChartSeriesViewModel
                {
                    Label = category.CategoryName
                };

                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    series.Data.Add(
                        countsByDate.TryGetValue(date, out var count)
                            ? count
                            : 0);
                }

                chartSeries.Add(series);
            }

            // --------------------------------------------------------
            // OTHER CATEGORY CHART SERIES
            // --------------------------------------------------------

            if (otherCategoryIds.Count > 0)
            {
                var otherCountsByDate = otherDailyCounts
                    .GroupBy(x => x.Date.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x => x.TotalTests));

                var otherSeries = new ChartSeriesViewModel
                {
                    Label = "Other"
                };

                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    otherSeries.Data.Add(
                        otherCountsByDate.TryGetValue(date, out var count)
                            ? count
                            : 0);
                }

                chartSeries.Add(otherSeries);
            }

            // --------------------------------------------------------
            // RETURN REPORT
            // --------------------------------------------------------

            return new ManagementReportViewModel
            {
                StartDate = start,
                EndDate = end,

                TotalCompletedTests = totalCompletedTests,
                TotalInProgressTests = totalInProgressTests,

                TotalCategories = categoryTotals.Count,
                CategoryBreakdown = categoryTotals,

                ChartLabels = chartLabels,
                ChartSeries = chartSeries
            };
        }


        // ============================================================
        // PDF TABLE FORMATTING
        // ============================================================

        private static IContainer HeaderCell(IContainer container)
        {
            return container
                .Background(Colors.Blue.Darken2)
                .DefaultTextStyle(
                    x => x.FontColor(Colors.White).Bold())
                .Padding(6);
        }

        private static IContainer BodyCell(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6);
        }

        private static IContainer TotalCell(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .DefaultTextStyle(x => x.Bold())
                .Padding(6);
        }



   

    }
}