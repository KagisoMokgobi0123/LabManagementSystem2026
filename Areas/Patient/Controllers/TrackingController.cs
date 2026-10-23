using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    public record TestTypeOption(int Id, string Name, int Points);

    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<PatientModel?> GetPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IActionResult> Index()
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var pairs = await (from i in _context.TestRequestTestTypes
                               join r in _context.TestRequests on i.TestRequestId equals r.TestRequestId
                               join t in _context.TestTypes on i.TestTypeId equals t.TestTypeId
                               where r.PatientId == patient.UserId
                                     && r.Status == "ReleasedByDoctor"
                                     && i.ResultValue != null
                               select new { t.TestTypeId, t.Name }).ToListAsync();

            var options = pairs
                .GroupBy(p => new { p.TestTypeId, p.Name })
                .Select(g => new TestTypeOption(g.Key.TestTypeId, g.Key.Name, g.Count()))
                .OrderBy(o => o.Name)
                .ToList();

            ViewBag.TestTypes = options;
            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Data(int testTypeId, DateTime? dateFrom, DateTime? dateTo)
        {
            var patient = await GetPatient();
            if (patient == null) return Unauthorized();

            var type = await _context.TestTypes.FirstOrDefaultAsync(t => t.TestTypeId == testTypeId);
            if (type == null) return NotFound();

            var unit = await _context.MeasurementUnits
                .Where(u => u.MeasurementUnitId == type.MeasurementUnit)
                .Select(u => u.Unit)
                .FirstOrDefaultAsync();

            var raw = await (from i in _context.TestRequestTestTypes
                             join r in _context.TestRequests on i.TestRequestId equals r.TestRequestId
                             where r.PatientId == patient.UserId
                                   && r.Status == "ReleasedByDoctor"
                                   && i.TestTypeId == testTypeId
                                   && i.ResultValue != null
                             select new
                             {
                                 When = i.CompletionDateTime ?? r.RequestDate,
                                 r.RequestNumber,
                                 i.ResultValue,
                                 i.IsAbnormal
                             }).ToListAsync();

            var rows = raw.AsEnumerable();
            if (dateFrom.HasValue) rows = rows.Where(x => x.When >= dateFrom.Value.Date);
            if (dateTo.HasValue) rows = rows.Where(x => x.When < dateTo.Value.Date.AddDays(1));

            var points = rows
                .OrderBy(x => x.When)
                .Select(x => new
                {
                    date = x.When.ToString("dd MMM yyyy"),
                    iso = x.When.ToString("yyyy-MM-dd"),
                    value = x.ResultValue,
                    abnormal = x.IsAbnormal,
                    requestNumber = x.RequestNumber
                })
                .ToList();

            var hasRange = type.NormalMin != 0 || type.NormalMax != 0;

            return Json(new
            {
                testName = type.Name,
                unit = unit ?? "",
                min = type.NormalMin,
                max = type.NormalMax,
                hasRange,
                points
            });
        }
    }
}