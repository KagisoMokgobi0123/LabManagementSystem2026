using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // SHA256 helper — matches AccountController.HashPassword pattern
        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // ============================================================
        // 1. My Profile
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(patient);
        }

        // ============================================================
        // 2. Change Password — GET (page)
        // ============================================================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // ============================================================
        // 2. Change Password — POST
        // Uses SHA256 (session auth), NOT UserManager.ChangePasswordAsync
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Index", "Home");

            // Verify current password using SHA256
            if (user.PasswordHash != HashPassword(currentPassword))
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("ChangePassword");
            }

            // Validate new password rules
            var ok = !string.IsNullOrEmpty(newPassword)
                     && newPassword.Length >= 8
                     && newPassword.Any(char.IsUpper)
                     && newPassword.Any(char.IsDigit)
                     && newPassword.Any(c => !char.IsLetterOrDigit(c));

            if (!ok)
            {
                TempData["Error"] = "New password must be at least 8 characters with one uppercase, one number, and one special character.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword == currentPassword)
            {
                TempData["Error"] = "New password must be different from the current password.";
                return RedirectToAction("ChangePassword");
            }

            user.PasswordHash = HashPassword(newPassword);
            user.MustChangePassword = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }

        // ============================================================
        // 3. Change Phone Number
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(string contactNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                TempData["Error"] = "Phone number cannot be empty.";
                return RedirectToAction("ChangePhoneNumber");
            }

            patient.ContactNumber = contactNumber.Trim();
            patient.LastUpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Phone number updated successfully.";
            return RedirectToAction("Index");
        }

        // ============================================================
        // 3. Inline Update — Phone (used by Profile page inline form)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhone(string contactNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                if (string.IsNullOrWhiteSpace(contactNumber))
                {
                    TempData["Error"] = "Phone number cannot be empty.";
                }
                else
                {
                    patient.ContactNumber = contactNumber.Trim();
                    patient.LastUpdatedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Phone number updated.";
                }
            }
            return RedirectToAction("Index");
        }

        // ============================================================
        // 3. Inline Update — Address
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(string address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    TempData["Error"] = "Address cannot be empty.";
                }
                else
                {
                    patient.Address = address.Trim();
                    patient.LastUpdatedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Address updated.";
                }
            }
            return RedirectToAction("Index");
        }

        // ============================================================
        // 3. Inline Update — City
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCity(string city)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                patient.City = city?.Trim();
                patient.LastUpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "City updated.";
            }
            return RedirectToAction("Index");
        }

        // ============================================================
        // 3. Inline Update — Province
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProvince(string province)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                patient.Province = province?.Trim();
                patient.LastUpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Province updated.";
            }
            return RedirectToAction("Index");
        }
    }
}