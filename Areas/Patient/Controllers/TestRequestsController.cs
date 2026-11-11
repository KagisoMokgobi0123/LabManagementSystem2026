using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class TestRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestRequestsController(ApplicationDbContext context)
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

            ViewBag.Heading = "My Test Requests";
            ViewBag.Subheading = "Every laboratory request your doctors have made for you. Select View details to see the tests, progress and results.";
            ViewBag.Empty = "Your doctor will submit test requests on your behalf. They will appear here as soon as they are submitted.";

            var rows = await RequestQueries.LoadRowsAsync(_context, patient.UserId, null);
            return View(rows);
        }

        public async Task<IActionResult> Active()
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            ViewBag.Heading = "Active Tests";
            ViewBag.Subheading = "Requests the laboratory is still working on. They move to My Results once your doctor releases them.";
            ViewBag.Empty = "You have no requests in progress right now.";

            var statuses = new[] { "Submitted", "Samples Received", "InProgress" };
            var rows = await RequestQueries.LoadRowsAsync(_context, patient.UserId, statuses);
            return View("Index", rows);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var request = await _context.TestRequests
                .FirstOrDefaultAsync(x => x.TestRequestId == id && x.PatientId == patient.UserId);
            if (request == null) return NotFound();

            var released = request.Status == "ReleasedByDoctor";
            var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.DoctorId);

            var items = await (from i in _context.TestRequestTestTypes
                               join t in _context.TestTypes on i.TestTypeId equals t.TestTypeId
                               where i.TestRequestId == id
                               select new
                               {
                                   t.Name,
                                   t.TestCategoryId,
                                   t.NormalMin,
                                   t.NormalMax,
                                   t.MeasurementUnit,
                                   i.ResultValue,
                                   i.IsAbnormal
                               }).ToListAsync();

            var categories = await _context.TestCategories.ToDictionaryAsync(c => c.TestCategoryId, c => c.Description);
            var units = await _context.MeasurementUnits
                .ToDictionaryAsync(u => u.MeasurementUnitId, u => u.Unit);

            var tests = items.Select(x => new RequestTestRow
            {
                TestName = x.Name,
                Category = categories.ContainsKey(x.TestCategoryId) ? categories[x.TestCategoryId] : "",
                Value = released ? x.ResultValue : null,
                Unit = units.ContainsKey(x.MeasurementUnit) ? units[x.MeasurementUnit] : "",
                Range = (x.NormalMin == 0 && x.NormalMax == 0)
                    ? "-"
                    : x.NormalMin.ToString("0.##") + " - " + x.NormalMax.ToString("0.##"),
                Abnormal = released && x.IsAbnormal
            }).OrderBy(t => t.Category).ThenBy(t => t.TestName).ToList();

            var step = request.Status switch
            {
                "Submitted" => 1,
                "Samples Received" => 2,
                "InProgress" => 3,
                "Completed" => 4,
                "ReleasedByDoctor" => 5,
                _ => 0
            };

            var model = new RequestDetailsViewModel
            {
                Id = request.TestRequestId,
                RequestNumber = request.RequestNumber,
                RequestDate = request.RequestDate,
                Doctor = RequestQueries.DoctorName(doctor?.FirstName + " " + doctor?.LastName),
                Urgency = request.Urgency.ToString(),
                Status = request.Status,
                ClinicalNotes = request.ClinicalNotes ?? "",
                Released = released,
                Cancelled = request.Status == "Cancelled",
                Step = step,
                AbnormalCount = tests.Count(t => t.Abnormal),
                Tests = tests
            };

            return View(model);
        }
    }
}