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
    public class ResultsController : Controller
    {
        private const string SharedView = "~/Areas/Patient/Views/TestRequests/Index.cshtml";
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<PatientModel?> GetPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // My Results: only what the doctor has released
        public async Task<IActionResult> Index()
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            ViewBag.Heading = "My Results";
            ViewBag.Subheading = "Results your doctors have released to you. Select View details to see each result and download it as a PDF.";
            ViewBag.Empty = "No results have been released yet. You will find them here once your doctor releases them.";

            var rows = await RequestQueries.LoadRowsAsync(_context, patient.UserId, new[] { "ReleasedByDoctor" });
            return View(SharedView, rows);
        }

        // Result History: every request that has finished
        public async Task<IActionResult> History()
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            ViewBag.Heading = "Result History";
            ViewBag.Subheading = "Requests that have finished: completed, released or cancelled, newest first.";
            ViewBag.Empty = "Finished requests will appear here.";

            var statuses = new[] { "Completed", "ReleasedByDoctor", "Cancelled" };
            var rows = await RequestQueries.LoadRowsAsync(_context, patient.UserId, statuses);
            return View(SharedView, rows);
        }
    }
}