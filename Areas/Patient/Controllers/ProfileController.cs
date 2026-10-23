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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ============================================================
        // 1. My Profile
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("Index", "Home");

            ViewBag.PatientName = patient.Name + " " + patient.Surname;

            return View(patient);
        }

        // ============================================================
        // 2. Change Password - GET
        // ============================================================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // ============================================================
        // 2. Change Password - POST (ASP.NET Core Identity)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("PatientLogin", "Account",
                    new { area = "" });
            }

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                TempData["Error"] = "Enter your current password.";
                return RedirectToAction(nameof(ChangePassword));
            }

            if (string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length < 8 ||
                !newPassword.Any(char.IsUpper) ||
                !newPassword.Any(char.IsDigit) ||
                !newPassword.Any(c => !char.IsLetterOrDigit(c)))
            {
                TempData["Error"] =
                    "New password must be at least 8 characters and contain an uppercase letter, a number, and a special character.";

                return RedirectToAction(nameof(ChangePassword));
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction(nameof(ChangePassword));
            }

            if (newPassword == currentPassword)
            {
                TempData["Error"] =
                    "Your new password must be different from your current password.";

                return RedirectToAction(nameof(ChangePassword));
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ",
                    result.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(ChangePassword));
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 3. Change Phone Number - GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction(nameof(Index));

            return View(patient);
        }

        // ============================================================
        // 3. Change Phone Number - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(string contactNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction(nameof(Index));

            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                TempData["Error"] = "Phone number cannot be empty.";
                return RedirectToAction(nameof(ChangePhoneNumber));
            }

            patient.ContactNumber = contactNumber.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Phone number updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 4. Inline Update - Phone
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhone(string contactNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                TempData["Error"] = "Patient profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                TempData["Error"] = "Phone number cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            patient.ContactNumber = contactNumber.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Phone number updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 5. Inline Update - Address
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(string address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                TempData["Error"] = "Patient profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                TempData["Error"] = "Address cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            patient.Address = address.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Address updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 6. Inline Update - City
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCity(string city)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                TempData["Error"] = "Patient profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                TempData["Error"] = "City cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            patient.City = city.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "City updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 7. Inline Update - Province
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProvince(string province)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                TempData["Error"] = "Patient profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(province))
            {
                TempData["Error"] = "Province cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            patient.Province = province.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Province updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}