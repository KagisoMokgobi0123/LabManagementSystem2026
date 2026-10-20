using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        private readonly ILogger<ProfileController> _logger;


        public ProfileController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }


        // =========================================================
        // MY PROFILE
        // GET: /Technician/Profile
        // =========================================================

        [HttpGet]
        // =========================================================
        // MY PROFILE
        // GET: /Technician/Profile
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ProfileIndex()
        {
            // STEP 1: Get the logged-in technician
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            // STEP 2: Retrieve technician profile details
            var technicianProfile = await _context.TechnicianProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            // STEP 3: Populate profile view model
            var model = new TechnicianProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",

                IdNumber = technicianProfile?.IdNumber ?? "",
                EmployeeNumber = technicianProfile?.EmployeeNumber ?? ""
            };

            // STEP 4: Return profile view
            return View(model);
        }
        // =========================================================
        // CHANGE PASSWORD - GET
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            return View(new ChangePasswordViewModel());
        }

        // =========================================================
        // CHANGE PASSWORD - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    GetValidationErrors();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(
                    nameof(model.NewPassword),
                    "The new password must differ from the current password.");

                TempData["ErrorMessage"] =
                    "Choose a different new password.";

                return View(model);
            }

            // Identity verifies the current password and enforces
            // the password policy configured in Program.cs.
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);

                TempData["ErrorMessage"] =
                    string.Join(" ", errors);

                _logger.LogWarning(
                    "Identity password change failed for {UserId}: {Codes}",
                    user.Id,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return View(model);
            }

            // Password has already been changed by Identity.
            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;

                var updateResult =
                    await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    _logger.LogError(
                        "Password changed, but first-login flag update failed for {UserId}",
                        user.Id);
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] =
                "Your password has been changed successfully.";

            return RedirectToAction(nameof(ChangePassword));
        }

        // =========================================================
        // CHANGE PHONE NUMBER - GET
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            return View(new UpdatePhoneNumberViewModel
            {
                PhoneNumber = user.PhoneNumber ?? string.Empty
            });
        }

        // =========================================================
        // CHANGE PHONE NUMBER - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(
            UpdatePhoneNumberViewModel model)
        {
            // Normalize before validating the number's format.
            var phone = (model.PhoneNumber ?? string.Empty)
                .Trim()
                .Replace(" ", "")
                .Replace("-", "");

            // Remove earlier PhoneNumber validation results
            // so the normalized value can be checked.
            ModelState.Remove(nameof(model.PhoneNumber));

            model.PhoneNumber = phone;

            if (string.IsNullOrWhiteSpace(phone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(
                    phone,
                    @"^(0[6-8][0-9]{8}|\+27[6-8][0-9]{8})$"))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Enter a valid South African cellphone number.");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    GetValidationErrors();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            if (user.PhoneNumber == phone)
            {
                TempData["InfoMessage"] =
                    "This is already your current cellphone number.";

                return RedirectToAction(nameof(ChangePhoneNumber));
            }

            // ASP.NET Core Identity persists the cellphone number.
            var result =
                await _userManager.SetPhoneNumberAsync(user, phone);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);

                TempData["ErrorMessage"] =
                    string.Join(" ", errors);

                _logger.LogWarning(
                    "Identity phone update failed for {UserId}: {Codes}",
                    user.Id,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return View(model);
            }

            // Independently verify the persisted value using
            // a no-tracking database query.
            var savedPhone = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == user.Id)
                .Select(u => u.PhoneNumber)
                .SingleOrDefaultAsync();

            if (savedPhone != phone)
            {
                _logger.LogError(
                    "Phone persistence verification failed for {UserId}",
                    user.Id);

                TempData["ErrorMessage"] =
                    "Identity reported success, but the saved number could not be verified.";

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] =
                "Your cellphone number has been updated successfully.";

            return RedirectToAction(nameof(ChangePhoneNumber));
        }

        // =========================================================
        // SHARED VALIDATION HELPER
        // Place this inside ProfileController, outside all actions.
        // =========================================================
        private string GetValidationErrors()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToList();

            return errors.Count > 0
                ? string.Join(" ", errors)
                : "Please check the information entered.";
        }



        // =========================================================
        // LOGOUT
        // POST: /Technician/Profile/Logout
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Get user before signing out
            var user =
                await _userManager.GetUserAsync(User);


            var email =
                user?.Email ??
                User.Identity?.Name ??
                "Unknown";


            // =====================================================
            // SIGN OUT
            // =====================================================

            await _signInManager.SignOutAsync();


            _logger.LogInformation(
                "Lab Technician {Email} logged out successfully.",
                email
            );


            // =====================================================
            // REDIRECT TO TECHNICIAN LOGIN
            //
            // area = "" is VERY IMPORTANT.
            // TechnicianLogin is in the main AccountController,
            // not inside the Technician area.
            // =====================================================

            return RedirectToAction(
                "TechnicianLogin",
                "Account",
                new
                {
                    area = ""
                }
            );
        }

    }
}