using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        static ReportsController()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<PatientModel?> GetPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.DateFrom = (dateFrom ?? DateTime.Now.AddMonths(-3)).ToString("yyyy-MM-dd");
            ViewBag.DateTo = (dateTo ?? DateTime.Now).ToString("yyyy-MM-dd");

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                var rows = await LoadRows(patient, dateFrom.Value, dateTo.Value);
                ViewBag.Results = rows;
                ViewBag.HasPreview = true;
                ViewBag.TotalCount = rows.Count;
                ViewBag.AbnormalCount = rows.Count(r => r.IsAbnormal);
            }
            else
            {
                ViewBag.HasPreview = false;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Download(DateTime dateFrom, DateTime dateTo)
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            if (dateFrom.Date > dateTo.Date)
            {
                TempData["Error"] = "Start date must be on or before end date.";
                return RedirectToAction(nameof(Index));
            }

            var rows = await LoadRows(patient, dateFrom, dateTo);
            if (rows.Count == 0)
            {
                TempData["Error"] = "No released results found between "
                    + dateFrom.ToString("dd MMM yyyy") + " and " + dateTo.ToString("dd MMM yyyy") + ".";
                return RedirectToAction(nameof(Index));
            }

            var start = dateFrom.Date;
            var generatedOn = DateTime.Now;
            var abnormalCount = rows.Count(r => r.IsAbnormal);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(t => t.FontSize(9.5f));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("NMB-HLabSys Haematology Laboratory")
                            .FontSize(16).Bold().FontColor(Colors.BlueGrey.Darken3);
                        col.Item().Text("Patient Laboratory Results Report")
                            .FontSize(11).FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(4);

                        col.Item().Text("Patient: " + patient.Name + " " + patient.Surname).Bold();
                        col.Item().Text("ID number: " + patient.IDNumber + "     Date of birth: " + patient.DateOfBirth.ToString("dd MMM yyyy"));
                        col.Item().Text("Period: " + start.ToString("dd MMM yyyy") + " to " + dateTo.Date.ToString("dd MMM yyyy")
                            + "     Generated: " + generatedOn.ToString("dd MMM yyyy HH:mm"))
                            .FontColor(Colors.Grey.Darken2);
                        col.Item().Text("Total results: " + rows.Count + "     Abnormal results: " + abnormalCount).Bold();

                        foreach (var g in rows.GroupBy(r => r.Category).OrderBy(g => g.Key))
                        {
                            col.Item().PaddingTop(14).Text(g.Key)
                                .FontSize(12).Bold().FontColor(Colors.BlueGrey.Darken3);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(66);
                                    c.ConstantColumn(78);
                                    c.RelativeColumn(3);
                                    c.RelativeColumn(1.3f);
                                    c.RelativeColumn(1.3f);
                                    c.RelativeColumn(1.8f);
                                    c.ConstantColumn(58);
                                });

                                table.Header(h =>
                                {
                                    foreach (var title in new[] { "Date", "Request", "Test", "Result", "Unit", "Normal range", "Flag" })
                                    {
                                        h.Cell().Element(HeaderCell).Text(title).FontColor(Colors.White).Bold();
                                    }
                                });

                                foreach (var r in g.OrderBy(x => x.When))
                                {
                                    table.Cell().Element(BodyCell).Text(r.When.ToString("dd MMM yyyy"));
                                    table.Cell().Element(BodyCell).Text(r.RequestNumber);
                                    table.Cell().Element(BodyCell).Text(r.TestName);

                                    if (r.IsAbnormal)
                                        table.Cell().Element(BodyCell).Text(r.Value).Bold().FontColor(Colors.Red.Darken2);
                                    else
                                        table.Cell().Element(BodyCell).Text(r.Value);

                                    table.Cell().Element(BodyCell).Text(r.Unit);
                                    table.Cell().Element(BodyCell).Text(r.NormalRange);

                                    if (r.IsAbnormal)
                                        table.Cell().Element(BodyCell).Text("Abnormal").Bold().FontColor(Colors.Red.Darken2);
                                    else
                                        table.Cell().Element(BodyCell).Text("Normal");
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.DefaultTextStyle(s => s.FontSize(8.5f).FontColor(Colors.Grey.Darken1));
                        t.Span("Page ");
                        t.CurrentPageNumber();
                        t.Span(" of ");
                        t.TotalPages();
                    });
                });
            });

            var bytes = document.GeneratePdf();
            var fileName = "LabReport_" + start.ToString("yyyyMMdd") + "_" + dateTo.Date.ToString("yyyyMMdd") + ".pdf";
            return File(bytes, "application/pdf", fileName);
        }

        private async Task<List<ReportRow>> LoadRows(PatientModel patient, DateTime dateFrom, DateTime dateTo)
        {
            var start = dateFrom.Date;
            var end = dateTo.Date.AddDays(1);

            var raw = await (from i in _context.TestRequestTestTypes
                             join r in _context.TestRequests on i.TestRequestId equals r.TestRequestId
                             join t in _context.TestTypes on i.TestTypeId equals t.TestTypeId
                             where r.PatientId == patient.UserId
                                   && r.Status == "ReleasedByDoctor"
                                   && i.ResultValue != null
                             select new
                             {
                                 When = i.CompletionDateTime ?? r.RequestDate,
                                 r.RequestNumber,
                                 TestName = t.Name,
                                 Value = i.ResultValue,
                                 i.IsAbnormal,
                                 t.NormalMin,
                                 t.NormalMax,
                                 t.TestCategoryId,
                                 t.MeasurementUnit
                             }).ToListAsync();

            var filtered = raw.Where(x => x.When >= start && x.When < end).ToList();

            if (filtered.Count == 0) return new List<ReportRow>();

            var categories = await _context.TestCategories.ToListAsync();
            var catMap = categories.ToDictionary(c => c.TestCategoryId, c => c.Description ?? "Other");

            var units = await _context.MeasurementUnits.ToListAsync();
            var unitMap = units.ToDictionary(u => u.MeasurementUnitId, u => u.Unit ?? "");

            return filtered.Select(x => new ReportRow
            {
                When = x.When,
                RequestNumber = x.RequestNumber ?? "",
                TestName = x.TestName ?? "",
                Value = x.Value.HasValue ? x.Value.Value.ToString("0.##") : "-",
                Unit = unitMap.ContainsKey(x.MeasurementUnit) ? unitMap[x.MeasurementUnit] : "",
                NormalRange = (x.NormalMin == 0 && x.NormalMax == 0)
                    ? "-"
                    : x.NormalMin.ToString("0.##") + " - " + x.NormalMax.ToString("0.##"),
                IsAbnormal = x.IsAbnormal,
                Category = catMap.ContainsKey(x.TestCategoryId) ? catMap[x.TestCategoryId] : "Other"
            }).ToList();
        }

        private static IContainer HeaderCell(IContainer container) =>
            container.Background(Colors.BlueGrey.Darken3).Padding(5);

        private static IContainer BodyCell(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }

    public class ReportRow
    {
        public DateTime When { get; set; }
        public string RequestNumber { get; set; } = "";
        public string TestName { get; set; } = "";
        public string Value { get; set; } = "";
        public string Unit { get; set; } = "";
        public string NormalRange { get; set; } = "";
        public bool IsAbnormal { get; set; }
        public string Category { get; set; } = "";
    }
}