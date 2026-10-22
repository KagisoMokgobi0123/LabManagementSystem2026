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


        // A small visual representation of the relationship between test processed and categories.

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


        // To be printed report, in a PDF format
        [HttpGet]
        public async Task<IActionResult> GeneratePdf(DateTime startDate, DateTime endDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (startDate == default ||
                endDate == default ||
                startDate.Date > endDate.Date)
            {
                TempData["ErrorMessage"] =
                    "Please select a valid reporting date range.";

                return RedirectToAction(nameof(Index));
            }

      

            var managerName = currentUser?.FullName ?? "Unknown Manager";

            var managerEmail = currentUser?.Email ?? "No email available";

            var report = await BuildReportData(startDate.Date,endDate.Date);

            var totalTrackedTests =
                report.TotalCompletedTests + report.TotalInProgressTests;

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "assets","img",
                "logo.png");


            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.MarginTop(25);
                    page.MarginBottom(30);
                    page.MarginLeft(35);
                    page.MarginRight(35);

                    page.DefaultTextStyle(x =>
                        x.FontFamily("Lato")
                         .FontSize(9)
                         .FontColor(Colors.Grey.Darken3));

                    //The top of the report look

                    page.Header()
                        .Column(header =>
                        {
                            header.Spacing(0);

                            header.Item()
                                .Background(Colors.Blue.Darken4)
                                .PaddingHorizontal(18)
                                .PaddingVertical(14)
                                .Row(row =>
                                {
                                    row.ConstantItem(125)
                                        .Height(58)
                                        .AlignMiddle()
                                        .Image(logoPath)
                                        .FitArea();

                                    row.ConstantItem(15);

                                    // Organisation information
                                    row.RelativeItem()
                                        .AlignMiddle()
                                        .Column(info =>
                                        {
                                            info.Item()
                                                .Text("NMB HEALTH LABORATORY SYSTEM")
                                                .Bold()
                                                .FontSize(15)
                                                .FontColor(Colors.White);

                                            info.Item()
                                                .PaddingTop(3)
                                                .Text("Management & Laboratory Services")
                                                .FontSize(8)
                                                .FontColor(Colors.Grey.Lighten2);
                                        });

                                    // Report information
                                    row.ConstantItem(165)
                                        .AlignMiddle()
                                        .Column(info =>
                                        {
                                            info.Item()
                                                .AlignRight()
                                                .Text("MANAGEMENT REPORT")
                                                .Bold()
                                                .FontSize(10)
                                                .FontColor(Colors.White);

                                            info.Item()
                                                .PaddingTop(3)
                                                .AlignRight()
                                                .Text($"Generated: {DateTime.Now:dd MMM yyyy}")
                                                .FontSize(7.5f)
                                                .FontColor(Colors.Grey.Lighten2);

                                            info.Item()
                                                .AlignRight()
                                                .Text($"Time: {DateTime.Now:HH:mm}")
                                                .FontSize(7.5f)
                                                .FontColor(Colors.Grey.Lighten2);
                                        });
                                });


                            // Report title area
                            header.Item()
                                .PaddingTop(13)
                                .PaddingBottom(8)
                                .Column(title =>
                                {
                                    title.Item()
                                        .Text("MANAGEMENT TEST PERFORMANCE REPORT BY CATEGORY")
                                        .Bold()
                                        .FontSize(16)
                                        .FontColor(Colors.Blue.Darken4);

                                    title.Item()
                                        .PaddingTop(4)
                                        .Text(
                                            $"Reporting Period: {report.StartDate:dd MMMM yyyy} - " +
                                            $"{report.EndDate:dd MMMM yyyy}")
                                        .FontSize(9)
                                        .FontColor(Colors.Grey.Darken1);

                                    title.Item()
                                        .PaddingTop(7)
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Column(manager =>
                                                {
                                                    manager.Item()
                                                        .Text("REPORT PREPARED BY")
                                                        .Bold()
                                                        .FontSize(7)
                                                        .FontColor(Colors.Grey.Darken1);

                                                    manager.Item()
                                                        .PaddingTop(2)
                                                        .Text(managerName)
                                                        .Bold()
                                                        .FontSize(9)
                                                        .FontColor(Colors.Blue.Darken3);

                                                    manager.Item()
                                                        .PaddingTop(1)
                                                        .Text(managerEmail)
                                                        .FontSize(7.5f)
                                                        .FontColor(Colors.Grey.Darken1);
                                                });

                                            row.ConstantItem(1)
                                                .Background(Colors.Grey.Lighten1);

                                            row.RelativeItem()
                                                .PaddingLeft(12)
                                                .Column(details =>
                                                {
                                                    details.Item()
                                                        .Text("REPORT TYPE")
                                                        .Bold()
                                                        .FontSize(7)
                                                        .FontColor(Colors.Grey.Darken1);

                                                    details.Item()
                                                        .PaddingTop(2)
                                                        .Text("Management Test Performance")
                                                        .FontSize(8)
                                                        .FontColor(Colors.Grey.Darken2);

                                                    details.Item()
                                                        .PaddingTop(1)
                                                        .Text(
                                                            $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}")
                                                        .FontSize(7.5f)
                                                        .FontColor(Colors.Grey.Darken1);
                                                });
                                        });

                                    title.Item()
                                        .PaddingTop(6)
                                        .Text("Completed tests are grouped by completion date")
                                        .FontSize(7.5f)
                                        .FontColor(Colors.Grey.Darken1);
                                });

                            header.Item()
                                .Height(1)
                                .Background(Colors.Grey.Lighten1);
                        });


                    //The report Details and Table

                    page.Content()
                        .PaddingTop(14)
                        .Column(column =>
                        {
                            column.Spacing(12);


                            // Small Summary in boxes/div format

                            column.Item()
                                .Text("REPORT SUMMARY")
                                .Bold()
                                .FontSize(11)
                                .FontColor(Colors.Blue.Darken4);

                            column.Item()
                                .Row(row =>
                                {
                                    row.ConstantItem(8);

                                    // Categories
                                    row.RelativeItem()
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Background(Colors.White)
                                        .Padding(11)
                                        .Column(summary =>
                                        {
                                            summary.Item()
                                                .Text("TEST CATEGORIES")
                                                .Bold()
                                                .FontSize(7.5f)
                                                .FontColor(Colors.Grey.Darken1);

                                            summary.Item()
                                                .PaddingTop(4)
                                                .Text(
                                                    report.TotalCategories.ToString("N0"))
                                                .Bold()
                                                .FontSize(20)
                                                .FontColor(Colors.Blue.Darken2);

                                            summary.Item()
                                                .PaddingTop(2)
                                                .Text("Categories represented")
                                                .FontSize(7)
                                                .FontColor(Colors.Grey.Darken1);
                                        });
                                });


                            // break down in table format

                            column.Item()
                                .PaddingTop(3)
                                .Text("CATEGORY BREAKDOWN")
                                .Bold()
                                .FontSize(11)
                                .FontColor(Colors.Blue.Darken4);

                            column.Item()
                                .Text(
                                    "Summary of laboratory testing activity by test category " +
                                    "for the selected reporting period.")
                                .FontSize(7.5f)
                                .FontColor(Colors.Grey.Darken1);


                            if (report.CategoryBreakdown.Count == 0)
                            {
                                column.Item()
                                    .Padding(20)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten4)
                                    .AlignCenter()
                                    .Column(empty =>
                                    {
                                        empty.Item()
                                            .Text("NO TEST RECORDS FOUND")
                                            .Bold()
                                            .FontSize(11)
                                            .FontColor(Colors.Blue.Darken3);

                                        empty.Item()
                                            .PaddingTop(5)
                                            .Text(
                                                "No completed or in-progress tests were " +
                                                "recorded during the selected reporting period.")
                                            .FontSize(8)
                                            .FontColor(Colors.Grey.Darken1);
                                    });
                            }
                            else
                            {
                                column.Item()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3.2f);
                                            columns.RelativeColumn(1.2f);
                                            columns.RelativeColumn(1.2f);
                                            columns.RelativeColumn(1.2f);
                                            columns.RelativeColumn(1.2f);
                                        });


                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("TEST CATEGORY");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("COMPLETED");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("IN PROGRESS");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("TOTAL");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("% TOTAL");
                                        });


                                        //body of the table

                                        foreach (var category in report.CategoryBreakdown)
                                        {
                                            table.Cell()
                                                .Element(BodyCell)
                                                .Text(category.CategoryName);

                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    category.CompletedTests.ToString("N0"));

                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    category.InProgressTests.ToString("N0"));

                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    category.TotalTests.ToString("N0"));

                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    $"{category.Percentage:F1}%");
                                        }


                                        //Rows

                                        table.Cell()
                                            .Element(TotalCell)
                                            .Text("TOTAL");

                                        table.Cell()
                                            .Element(TotalCell)
                                            .AlignRight()
                                            .Text(
                                                report.TotalCompletedTests.ToString("N0"));

                                        table.Cell()
                                            .Element(TotalCell)
                                            .AlignRight()
                                            .Text(
                                                report.TotalInProgressTests.ToString("N0"));

                                        table.Cell()
                                            .Element(TotalCell)
                                            .AlignRight()
                                            .Text(
                                                totalTrackedTests.ToString("N0"));

                                        table.Cell()
                                            .Element(TotalCell)
                                            .AlignRight()
                                            .Text(
                                                totalTrackedTests > 0
                                                    ? "100.0%"
                                                    : "0.0%");
                                    });
                            }




                        });


                    // Footer of The report for purpose of having more pages of the report

                    page.Footer()
                        .PaddingTop(8)
                        .Column(footer =>
                        {
                            footer.Item()
                                .Height(1)
                                .Background(Colors.Grey.Lighten1);

                            footer.Item()
                                .PaddingTop(6)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("NMB Health Laboratory System")
                                        .FontSize(7)
                                        .FontColor(Colors.Grey.Darken1);

                                    row.RelativeItem()
                                        .AlignCenter()
                                        .Text("CONFIDENTIAL MANAGEMENT REPORT")
                                        .Bold()
                                        .FontSize(6.5f)
                                        .FontColor(Colors.Grey.Darken1);

                                    row.RelativeItem()
                                        .AlignRight()
                                        .Text(text =>
                                        {
                                            text.Span("Page ")
                                                .FontSize(7)
                                                .FontColor(Colors.Grey.Darken1);

                                            text.CurrentPageNumber()
                                                .FontSize(7)
                                                .FontColor(Colors.Grey.Darken1);

                                            text.Span(" of ")
                                                .FontSize(7)
                                                .FontColor(Colors.Grey.Darken1);

                                            text.TotalPages()
                                                .FontSize(7)
                                                .FontColor(Colors.Grey.Darken1);
                                        });
                                });

                            footer.Item()
                                .PaddingTop(3)
                                .AlignCenter()
                                .Text("@ 2026 NMB-HLabSys")
                                .FontSize(6.5f)
                                .FontColor(Colors.Grey.Darken1);
                        });
                });
            })
            .GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                $"TestPerformanceReport_{report.StartDate:yyyyMMdd}_" +
                $"{report.EndDate:yyyyMMdd}.pdf");


            // style of The tables

            static IContainer HeaderCell(IContainer container)
            {
                return container
                    .Background(Colors.Blue.Darken4)
                    .BorderBottom(1)
                    .BorderColor(Colors.Blue.Darken3)
                    .PaddingVertical(7)
                    .PaddingHorizontal(7)
                    .DefaultTextStyle(text =>
                        text.Bold()
                            .FontSize(7.5f)
                            .FontColor(Colors.White));
            }

            static IContainer BodyCell(IContainer container)
            {
                return container
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(7)
                    .PaddingHorizontal(7)
                    .DefaultTextStyle(text =>
                        text.FontSize(8)
                            .FontColor(Colors.Grey.Darken3));
            }

            static IContainer TotalCell(IContainer container)
            {
                return container
                    .Background(Colors.Grey.Lighten3)
                    .BorderTop(1)
                    .BorderColor(Colors.Blue.Darken2)
                    .PaddingVertical(8)
                    .PaddingHorizontal(7)
                    .DefaultTextStyle(text =>
                        text.Bold()
                            .FontSize(8)
                            .FontColor(Colors.Blue.Darken4));
            }
        }

        // generate the report according to the selected dates.

        private async Task<ManagementReportViewModel> BuildReportData(
           DateTime startDate,
           DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            var endExclusive = end.AddDays(1);

            // select according to the requirements.
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

            //calculation to display all total completed and in progress.

            var totalCompletedTests = await reportTests
                .CountAsync(x => x.Status == "Completed");

            var totalInProgressTests = await reportTests
                .CountAsync(x => x.Status == "In Progress");

            var totalEligibleTests =
                totalCompletedTests + totalInProgressTests;

            //number of categories

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

            //show total in percentages

            foreach (var category in categoryTotals)
            {
                category.Percentage = category.TotalTests == 0
                    ? 0
                    : (decimal)category.CompletedTests /
                      category.TotalTests * 100;
            }

           

            var topCategories = categoryTotals
                .Take(TopCategoryLimit)
                .ToList();

            var topCategoryIds = topCategories
                .Select(x => x.TestCategoryId)
                .ToList();

            //it will only count the top 10 categories.

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

            // number of Remaining categories

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

            //Including date labeling.

            var chartLabels = new List<string>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                chartLabels.Add(date.ToString("dd MMM"));
            }

            // show the top categories within the chart

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

            // Other categories

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

            // return the report

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


        // how the table should be like int the report

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