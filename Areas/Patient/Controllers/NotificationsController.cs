using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    public class NotificationRow
    {
        public int Id { get; set; }
        public string Doctor { get; set; } = "";
        public DateTime RequestedOn { get; set; }
    }

    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<PatientModel?> GetPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        private async Task<List<PatientDoctorConsentModel>> PendingAsync(PatientModel patient)
        {
            var mine = await _context.PatientDoctorConsents
                .Where(c => c.PatientId == patient.PatientId)
                .ToListAsync();
            return mine.Where(c => !c.IsActive && c.RevokedDate == null)
                       .OrderByDescending(c => c.GrantedDate)
                       .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var pending = await PendingAsync(patient);

            var users = await _context.Users.ToListAsync();
            var names = users.ToDictionary(
                u => u.Id,
                u => ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim()
            );

            var rows = pending.Select(c => new NotificationRow
            {
                Id = c.PatientDoctorConsentId,
                Doctor = names.ContainsKey(c.DoctorUserId ?? "") ? names[c.DoctorUserId!] : "A doctor",
                RequestedOn = c.GrantedDate
            }).ToList();

            return View(rows);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var consent = (await PendingAsync(patient)).FirstOrDefault(c => c.PatientDoctorConsentId == id);
            if (consent != null)
            {
                consent.IsActive = true;
                consent.GrantedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Access approved. You can choose which records the doctor sees on the Doctor Access page.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var patient = await GetPatient();
            if (patient == null) return RedirectToAction("Index", "Home");

            var consent = (await PendingAsync(patient)).FirstOrDefault(c => c.PatientDoctorConsentId == id);
            if (consent != null)
            {
                consent.RevokedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Request declined.";
            }
            return RedirectToAction("Index");
        }
    }
}