using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class MedicalHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MedicalHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
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
    }
}