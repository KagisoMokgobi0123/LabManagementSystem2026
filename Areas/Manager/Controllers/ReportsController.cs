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
            var manager = await _userManager.GetUserAsync(User);

            if (manager == null)
            {
                return Challenge();
            }

            if (startDate == default ||
                endDate == default ||
                startDate.Date > endDate.Date)
            {
                TempData["ErrorMessage"] =
                    "Please select a valid reporting date range.";

                return RedirectToAction(nameof(Index));
            }


            var managerName = manager.FullName;
            var managerEmail = manager.Email ?? "Not provided";
            var managerPhone = manager.PhoneNumber ?? "Not provided";
            var laboratoryName = _configuration["EmailSettings:FromName"] ?? "NMB Health Laboratory System";

            var generatedAt = DateTime.Now;

            var report = await BuildReportData(
                startDate.Date,
                endDate.Date);

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(38);
                    page.MarginVertical(32);

                    page.DefaultTextStyle(x => x
                        .FontFamily("Lato")
                        .FontSize(9));

                    // ====================================================
                    // HEADER
                    // ====================================================

                    page.Header().Column(header =>
                    {
                        header.Spacing(4);

                        header.Item()
                            .Text(laboratoryName.ToUpperInvariant())
                            .FontSize(17)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        header.Item()
                            .Text("MANAGEMENT REPORT")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Grey.Darken2);

                        header.Item()
                            .PaddingTop(5)
                            .LineHorizontal(2)
                            .LineColor(Colors.Blue.Darken2);

                        header.Item()
                            .PaddingTop(5)
                            .Row(row =>
                            {
                                row.RelativeItem().Column(left =>
                                {
                                    left.Item()
                                        .Text("Prepared by")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Darken1);

                                    left.Item()
                                        .Text(managerName)
                                        .Bold()
                                        .FontSize(10);
                                });

                                row.RelativeItem().AlignRight().Column(right =>
                                {
                                    right.Item()
                                        .Text("REPORT REFERENCE")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Darken1);

                                    right.Item()
                                        .Text(
                                            $"MGT-{report.StartDate:yyyyMMdd}-" +
                                            $"{report.EndDate:yyyyMMdd}")
                                        .Bold()
                                        .FontSize(9);
                                });
                            });

                        header.Item()
                            .PaddingTop(4)
                            .Text(
                                $"Manager email: {managerEmail}  |  " +
                                $"Telephone: {managerPhone}")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken2);
                    });


                    // ------------------------------------------------
                    // CONTENT
                    // ------------------------------------------------
                    page.Content()
                                    .PaddingTop(20)
                                    .PaddingBottom(12)
                                    .Column(content =>
                                    {
                                        content.Spacing(16);

                                        // REPORT TITLE AND DATE RANGE
                                        content.Item().Column(title =>
                                        {
                                            title.Spacing(5);

                                            title.Item()
                                                .Text("Test Performance Report")
                                                .FontSize(19)
                                                .Bold()
                                                .FontColor(Colors.Blue.Darken4);

                                            title.Item()
                                                .Text(
                                                    $"Reporting period: " +
                                                    $"{report.StartDate:dd MMMM yyyy} " +
                                                    $"to {report.EndDate:dd MMMM yyyy}")
                                                .FontSize(10)
                                                .FontColor(Colors.Grey.Darken2);

                                            title.Item()
                                                .Text(
                                                    $"Generated on " +
                                                    $"{generatedAt:dd MMMM yyyy 'at' HH:mm}")
                                                .FontSize(8)
                                                .FontColor(Colors.Grey.Darken1);
                                        });

                                        // SUMMARY
                                        content.Item()
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Background(Colors.Grey.Lighten5)
                                            .Padding(14)
                                            .Row(row =>
                                            {
                                                row.RelativeItem().Column(summary =>
                                                {
                                                    summary.Item()
                                                        .Text("TOTAL COMPLETED TESTS")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(Colors.Grey.Darken2);

                                                    summary.Item()
                                                        .PaddingTop(5)
                                                        .Text(
                                                            report.TotalCompletedTests
                                                                .ToString("N0"))
                                                        .FontSize(25)
                                                        .Bold()
                                                        .FontColor(Colors.Blue.Darken3);
                                                });

                                                row.ConstantItem(1)
                                                    .Background(Colors.Grey.Lighten1);

                                                row.RelativeItem().PaddingLeft(15).Column(summary =>
                                                {
                                                    summary.Item()
                                                        .Text("TEST CATEGORIES")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(Colors.Grey.Darken2);

                                                    summary.Item()
                                                        .PaddingTop(5)
                                                        .Text(
                                                            report.TotalCategories
                                                                .ToString("N0"))
                                                        .FontSize(25)
                                                        .Bold()
                                                        .FontColor(Colors.Blue.Darken3);
                                                });
                                            });

                                        // CATEGORY BREAKDOWN HEADING
                                        content.Item().Column(section =>
                                        {
                                            section.Spacing(5);

                                            section.Item()
                                                .Text("Category Breakdown")
                                                .FontSize(13)
                                                .Bold()
                                                .FontColor(Colors.Blue.Darken3);

                                            section.Item()
                                                .Text(
                                                    "Distribution of completed tests by " +
                                                    "laboratory test category.")
                                                .FontSize(9)
                                                .FontColor(Colors.Grey.Darken2);
                                        });

                                        // CATEGORY TABLE
                                        if (report.CategoryBreakdown.Count == 0)
                                        {
                                            content.Item()
                                                .Border(1)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Padding(18)
                                                .AlignCenter()
                                                .Text(
                                                    "No completed tests were recorded " +
                                                    "during the selected reporting period.")
                                                .FontColor(Colors.Grey.Darken1);
                                        }
                                        else
                                        {
                                            content.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(columns =>
                                                {
                                                    columns.ConstantColumn(35);
                                                    columns.RelativeColumn(4);
                                                    columns.RelativeColumn(1.5f);
                                                    columns.RelativeColumn(1.5f);
                                                });

                                                table.Header(header =>
                                                {
                                                    header.Cell()
                                                        .Element(HeaderCell)
                                                        .AlignCenter()
                                                        .Text("No.");

                                                    header.Cell()
                                                        .Element(HeaderCell)
                                                        .Text("Test Category");

                                                    header.Cell()
                                                        .Element(HeaderCell)
                                                        .AlignRight()
                                                        .Text("Tests");

                                                    header.Cell()
                                                        .Element(HeaderCell)
                                                        .AlignRight()
                                                        .Text("Percentage");
                                                });

                                                var rowNumber = 0;

                                                foreach (var category in
                                                    report.CategoryBreakdown)
                                                {
                                                    rowNumber++;

                                                    table.Cell()
                                                        .Element(BodyCell)
                                                        .AlignCenter()
                                                        .Text(rowNumber.ToString());

                                                    table.Cell()
                                                        .Element(BodyCell)
                                                        .Text(category.CategoryName);

                                                    table.Cell()
                                                        .Element(BodyCell)
                                                        .AlignRight()
                                                        .Text(
                                                            category.TotalTests.ToString("N0"));

                                                    table.Cell()
                                                        .Element(BodyCell)
                                                        .AlignRight()
                                                        .Text(
                                                            $"{category.Percentage:F2}%");
                                                }

                                                table.Cell()
                                                    .Element(TotalCell)
                                                    .Text("");

                                                table.Cell()
                                                    .Element(TotalCell)
                                                    .Text("TOTAL COMPLETED TESTS");

                                                table.Cell()
                                                    .Element(TotalCell)
                                                    .AlignRight()
                                                    .Text(
                                                        report.TotalCompletedTests
                                                            .ToString("N0"));

                                                table.Cell()
                                                    .Element(TotalCell)
                                                    .AlignRight()
                                                    .Text(
                                                        report.TotalCompletedTests > 0
                                                            ? "100.00%"
                                                            : "0.00%");
                                            });
                                        }

                                        // REPORT NOTES
                                        content.Item()
                                            .PaddingTop(5)
                                            .BorderTop(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingTop(8)
                                            .Column(notes =>
                                            {
                                                notes.Spacing(4);

                                                notes.Item()
                                                    .Text("Report notes")
                                                    .Bold()
                                                    .FontSize(9);

                                                notes.Item()
                                                    .Text(
                                                        "This report summarizes tests marked " +
                                                        $"'{CompletedStatus}' based on their " +
                                                        "recorded completion dates within the " +
                                                        "selected reporting period.")
                                                    .FontSize(8)
                                                    .FontColor(Colors.Grey.Darken2);

                                                notes.Item()
                                                    .Text(
                                                        "This document is intended for " +
                                                        "laboratory management and operational " +
                                                        "review.")
                                                    .FontSize(8)
                                                    .FontColor(Colors.Grey.Darken2);
                                            });
                                    });

                    // ====================================================
                    // FOOTER
                    // ====================================================

                    page.Footer()
                        .PaddingTop(8)
                        .BorderTop(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(footer =>
                            {
                                footer.Item()
                                    .Text(laboratoryName)
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.Grey.Darken2);

                                footer.Item()
                                    .Text("Confidential management report")
                                    .FontSize(7)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.RelativeItem()
                                .AlignRight()
                                .AlignMiddle()
                                .Text(text =>
                                {
                                    text.Span("Page ")
                                        .FontSize(8);

                                    text.CurrentPageNumber();

                                    text.Span(" of ")
                                        .FontSize(8);

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
            var endExclusive = endDate.Date.AddDays(1);

            // --------------------------------------------------------
            // COMPLETED TESTS
            // --------------------------------------------------------

            var completedTests = _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(x =>
                    x.Status == CompletedStatus &&
                    x.CompletionDateTime.HasValue &&
                    x.CompletionDateTime.Value >= start &&
                    x.CompletionDateTime.Value < endExclusive &&
                    x.TestType != null &&
                    x.TestType.TestCategory != null);

            // --------------------------------------------------------
            // TOTAL COMPLETED TESTS
            // --------------------------------------------------------

            var totalCompletedTests =
                await completedTests.CountAsync();

            // --------------------------------------------------------
            // CATEGORY TOTALS
            // --------------------------------------------------------

            var categoryTotals = await completedTests
                .GroupBy(x => new
                {
                    x.TestType!.TestCategoryId,
                    CategoryName =
                        x.TestType.TestCategory!.Description
                })
                .Select(g => new CategoryReportItem
                {
                    TestCategoryId = g.Key.TestCategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalTests = g.Count()
                })
                .OrderByDescending(x => x.TotalTests)
                .ThenBy(x => x.CategoryName)
                .ToListAsync();

            foreach (var category in categoryTotals)
            {
                category.Percentage =
                    totalCompletedTests == 0
                        ? 0
                        : (decimal)category.TotalTests /
                          totalCompletedTests * 100;
            }

            // --------------------------------------------------------
            // TOP CATEGORIES
            // --------------------------------------------------------

            var topCategories = categoryTotals
                .Take(TopCategoryLimit)
                .ToList();

            var topCategoryIds = topCategories
                .Select(x => x.TestCategoryId)
                .ToList();

            // --------------------------------------------------------
            // DAILY COUNTS FOR TOP CATEGORIES
            // --------------------------------------------------------

            var dailyCounts = await completedTests
                .Where(x =>
                    topCategoryIds.Contains(
                        x.TestType!.TestCategoryId))
                .GroupBy(x => new
                {
                    Date = x.CompletionDateTime!.Value.Date,
                    x.TestType!.TestCategoryId
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
                otherDailyCounts = await completedTests
                    .Where(x =>
                        otherCategoryIds.Contains(
                            x.TestType!.TestCategoryId))
                    .GroupBy(x => x.CompletionDateTime!.Value.Date)
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

            for (var date = start; date <= endDate.Date;
                 date = date.AddDays(1))
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

                for (var date = start; date <= endDate.Date;
                     date = date.AddDays(1))
                {
                    series.Data.Add(
                        countsByDate.TryGetValue(date, out var count)
                            ? count
                            : 0);
                }

                chartSeries.Add(series);
            }

            // --------------------------------------------------------
            // OTHER CHART SERIES
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

                for (var date = start; date <= endDate.Date;
                     date = date.AddDays(1))
                {
                    otherSeries.Data.Add(
                        otherCountsByDate.TryGetValue(date, out var count)
                            ? count
                            : 0);
                }

                chartSeries.Add(otherSeries);
            }

            return new ManagementReportViewModel
            {
                StartDate = start,
                EndDate = endDate.Date,
                TotalCompletedTests = totalCompletedTests,
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