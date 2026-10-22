
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NMB_HLabSys.Areas.Doctor.ViewModels;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // =========================================================
        // MY PROFILE
        // GET: /Doctor/Profile
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            var model = new TechnicianProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };

            return View(model);
        }

        // =========================================================
        // EDIT PROFILE
        // GET: /Doctor/Profile/Edit
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            var model = new DoctorProfileEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        // =========================================================
        // EDIT PROFILE
        // POST: /Doctor/Profile/Edit
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            DoctorProfileEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var firstName = model.FirstName?.Trim();
            var lastName = model.LastName?.Trim();

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "First name and last name are required.");

                return View(model);
            }

            // Update only the permitted profile fields.
            // Email changes must use a separate verification workflow.
            user.FirstName = firstName;
            user.LastName = lastName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                _logger.LogWarning(
                    "Profile update failed for user {UserId}.",
                    user.Id);

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation(
                "Profile updated successfully for user {UserId}.",
                user.Id);

            TempData["SuccessMessage"] =
                "Your profile has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // CHANGE PASSWORD
        // GET: /Doctor/Profile/ChangePassword
        // =========================================================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // =========================================================
        // CHANGE PASSWORD
        // POST: /Doctor/Profile/ChangePassword
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation(
                "Password changed successfully for user {UserId}.",
                user.Id);

            TempData["SuccessMessage"] =
                "Your password has been changed successfully.";

            return RedirectToAction(nameof(ChangePassword));
        }

        // =========================================================
        // CHANGE PHONE NUMBER
        // GET: /Doctor/Profile/ChangePhoneNumber
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            var model = new UpdatePhoneNumberViewModel
            {
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };

            return View(model);
        }

        // =========================================================
        // CHANGE PHONE NUMBER
        // POST: /Doctor/Profile/ChangePhoneNumber
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(
            UpdatePhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("The current user account could not be found.");
            }

            var phoneNumber = model.PhoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Phone number is required.");

                return View(model);
            }

            if (user.PhoneNumber == phoneNumber)
            {
                TempData["InfoMessage"] =
                    "Your phone number has not changed.";

                return RedirectToAction(nameof(ChangePhoneNumber));
            }

            var result = await _userManager.SetPhoneNumberAsync(
                user,
                phoneNumber);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation(
                "Phone number updated successfully for user {UserId}.",
                user.Id);

            TempData["SuccessMessage"] =
                "Your phone number has been updated successfully.";

            return RedirectToAction(nameof(ChangePhoneNumber));
        }

        // =========================================================
        // LOGOUT
        // POST: /Doctor/Profile/Logout
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);

            var email = user?.Email
                ?? User.Identity?.Name
                ?? "Unknown";

            await _signInManager.SignOutAsync();

            _logger.LogInformation(
                "Doctor {Email} logged out successfully.",
                email);

            return RedirectToAction(
                "DoctorLogin",
                "Account",
                new { area = "" });
        }
    }
}
