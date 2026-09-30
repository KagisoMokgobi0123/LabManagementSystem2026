using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
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
        // GET: /Technician/Profile
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new TechnicianProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };

            return View(model);
        }


        // =========================================================
        // CHANGE PASSWORD
        // GET: /Technician/Profile/ChangePassword
        // =========================================================

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }


        // =========================================================
        // CHANGE PASSWORD
        // POST: /Technician/Profile/ChangePassword
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
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                // Refresh authentication cookie
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] =
                    "Your password has been changed successfully.";

                return RedirectToAction(nameof(ChangePassword));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        // =========================================================
        // CHANGE PHONE NUMBER
        // GET: /Technician/Profile/ChangePhoneNumber
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model =
                new UpdatePhoneNumberViewModel
                {
                    PhoneNumber =
                        user.PhoneNumber ?? ""
                };

            return View(model);
        }


        // =========================================================
        // CHANGE PHONE NUMBER
        // POST: /Technician/Profile/ChangePhoneNumber
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


            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }


            var phoneNumber =
                model.PhoneNumber.Trim();


            // No change
            if (user.PhoneNumber == phoneNumber)
            {
                TempData["InfoMessage"] =
                    "Your phone number has not changed.";

                return RedirectToAction(
                    nameof(ChangePhoneNumber)
                );
            }


            // Update through Identity
            var result =
                await _userManager.SetPhoneNumberAsync(
                    user,
                    phoneNumber
                );


            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description
                    );
                }

                return View(model);
            }


            // Refresh authentication cookie
            await _signInManager.RefreshSignInAsync(user);


            TempData["SuccessMessage"] =
                "Your phone number has been updated successfully.";


            return RedirectToAction(
                nameof(ChangePhoneNumber)
            );
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
                "ManagerLogin",
                "Account",
                new
                {
                    area = ""
                }
            );
        }

    }
}