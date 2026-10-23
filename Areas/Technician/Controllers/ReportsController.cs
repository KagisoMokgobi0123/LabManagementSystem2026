using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TechnicianReportRow
    {
        public int TestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string TestName { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime? CompletedUtc { get; set; }
        public string Status { get; set; } = "";
        public List<TechnicianReportResult> Results { get; set; } = new();
    }
    public class TechnicianReportResult
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string Flag { get; set; } = "";
        public bool IsAbnormal { get; set; }
        public DateTime? VerifiedUtc { get; set; }
        public string Notes { get; set; } = "";
    }
    public class TechnicianReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Generated { get; set; }
        public string TechnicianName { get; set; } = "";
        public DateTime GeneratedUtc { get; set; } = DateTime.UtcNow;
        public List<TechnicianReportRow> Tests { get; set; } = new();
        public int VerifiedCount => Tests.Count(t => t.Status == "Verified");
        public int CompletedCount => Tests.Count(t => t.Status == "Completed");
        public int AbnormalCount => Tests.SelectMany(t => t.Results).Count(r => r.IsAbnormal ||
            r.Flag == "High" || r.Flag == "Low" || r.Flag == "Abnormal");
    }

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
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var technician = await _userManager.GetUserAsync(User);
            if (technician == null) return Challenge();

            var model = new TechnicianReportViewModel
            {
                StartDate = startDate?.Date ?? new DateTime(2026, 10, 1),
                EndDate = endDate?.Date ?? new DateTime(2026, 10, 23),
                TechnicianName = technician.FullName ?? technician.UserName ?? "Technician"
            };
            if (!startDate.HasValue && !endDate.HasValue) return View("Index", model);
            if (!startDate.HasValue || !endDate.HasValue ||
                model.EndDate < model.StartDate || model.EndDate >= DateTime.MaxValue.Date)
            {
                ModelState.AddModelError("", "Please select a valid start and end date.");
                return View("Index", model);
            }

            // Database completion timestamps are UTC; the end date is inclusive.
            var from = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
            var until = DateTime.SpecifyKind(model.EndDate.AddDays(1), DateTimeKind.Utc);
            var tests = await _context.TestRequestTestTypes.AsNoTracking()
                .Include(t => t.TestType).ThenInclude(tt => tt!.TestCategory)
                .Include(t => t.TestRequest)
                .Where(t => t.TechnicianId == technician.Id && !t.IsDeleted &&
                    t.CompletionDateTime.HasValue &&
                    t.CompletionDateTime.Value >= from && t.CompletionDateTime.Value < until &&
                    (t.Status == "Completed" || t.Status == "Verified"))
                .OrderBy(t => t.CompletionDateTime)
                .ToListAsync();

            var ids = tests.Select(t => t.TestRequestTestTypeId).ToList();
            var results = await _context.TestResults.AsNoTracking()
                .Where(r => ids.Contains(r.TestRequestTestTypeId))
                .OrderBy(r => r.ResultName).ToListAsync();
            var byTest = results.ToLookup(r => r.TestRequestTestTypeId);
            model.Tests = tests.Select(t => new TechnicianReportRow
            {
                TestId = t.TestRequestTestTypeId,
                RequestNumber = t.TestRequest?.RequestNumber ?? "Not recorded",
                TestName = t.TestType?.Name ?? "Unknown test",
                Category = t.TestType?.TestCategory?.Description ?? "Uncategorized",
                CompletedUtc = t.CompletionDateTime,
                Status = t.Status,
                Results = byTest[t.TestRequestTestTypeId].Select(r => new TechnicianReportResult
                {
                    Name = r.ResultName,
                    Value = r.ResultValue,
                    Unit = r.MeasurementUnit ?? "",
                    Min = r.ReferenceMin,
                    Max = r.ReferenceMax,
                    Flag = r.ResultFlag ?? "Not evaluated",
                    IsAbnormal = r.IsAbnormal,
                    VerifiedUtc = r.VerifiedDateTime,
                    Notes = r.Notes ?? ""
                }).ToList()
            }).ToList();
            model.Generated = true;
            Response.Headers.CacheControl = "no-store";
            return View("Index", model);
        }
    }
}
