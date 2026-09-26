using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class PatientConsentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientConsentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Doctor/PatientConsent/Index
        public IActionResult Index()
        {
            var doctorId = GetCurrentDoctorId();

            if (doctorId == 0)
            {
                TempData["Error"] = "Please login as a doctor to view consents.";
                return RedirectToAction("Dashboard", "DoctorDashboard");
            }

            var consents = _context.PatientConsents
                .Include(c => c.Patient)
                .Include(c => c.ConsentRequestItems)
                    .ThenInclude(ci => ci.TestRequest)
                .Where(c => c.DoctorId == doctorId && c.IsActive)
                .ToList();

            return View(consents);
        }

        // GET: /Doctor/PatientConsent/ViewSharedResults/5
        public IActionResult ViewSharedResults(int consentId)
        {
            var doctorId = GetCurrentDoctorId();

            if (doctorId == 0)
            {
                TempData["Error"] = "Please login as a doctor to view results.";
                return RedirectToAction("Dashboard", "DoctorDashboard");
            }

            var consent = _context.PatientConsents
                .Include(c => c.Patient)
                .Include(c => c.ConsentRequestItems)
                    .ThenInclude(ci => ci.TestRequest)
                        .ThenInclude(r => r.TestRequestItems)
                            .ThenInclude(i => i.TestType)
                .FirstOrDefault(c => c.PatientConsentId == consentId
                                  && c.DoctorId == doctorId
                                  && c.IsActive);

            if (consent == null)
                return NotFound();

            return View(consent);
        }

        private int GetCurrentDoctorId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return 0;

            var doctor = _context.Doctors.FirstOrDefault(d => d.ApplicationUserId == userId);
            return doctor?.DoctorId ?? 0;
        }
    }
}