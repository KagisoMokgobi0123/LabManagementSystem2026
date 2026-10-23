using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class MedicalInformationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalInformationController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<NMB_HLabSys.Areas.Patient.Models.PatientModel?> GetCurrentPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // Medical Information overview — all 3 cards
        public async Task<IActionResult> Index()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .ToListAsync();

            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Conditions = conditions;
            ViewBag.Allergies = allergies;
            ViewBag.Medications = medications;

            return View();
        }

        // Conditions only
        public async Task<IActionResult> Conditions()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Conditions = conditions;
            return View();
        }

        // Allergies only
        public async Task<IActionResult> Allergies()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Allergies = allergies;
            return View();
        }

        // Medications only
        public async Task<IActionResult> Medications()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Medications = medications;
            return View();
        }
    }
}