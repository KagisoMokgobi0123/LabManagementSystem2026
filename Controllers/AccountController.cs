using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Account.ViewModels;
using System.Threading.Tasks;

namespace NMB_HLabSys.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<RegisterUser> _userManager;
        private readonly SignInManager<RegisterUser> _signInManager;

        public AccountController(
            UserManager<RegisterUser> userManager,
            SignInManager<RegisterUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // ============================================================
        // REGISTER
        // ============================================================

        // GET: Account/RegisterUser
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }


        // ============================================================
        // LOGIN
        // ============================================================

        // GET: Account/LoginUser
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }


        // POST: Account/LoginUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Email address is used as the username
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }


            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);


            if (result.Succeeded)
            {
                // ====================================================
                // ROLE-BASED REDIRECTION
                // ====================================================

                if (await _userManager.IsInRoleAsync(user, "Doctor"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "DoctorDashboard",
                        new { area = "Doctor" });
                }

                if (await _userManager.IsInRoleAsync(user, "Technician"))
                {
                    return RedirectToAction(
                        "Home",
                        "Home",
                        new { area = "Technician" });
                }

                if (await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Home");
                }

                if (await _userManager.IsInRoleAsync(
                    user,
                    "Admnistrator"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Home");
                }

                if (await _userManager.IsInRoleAsync(user, "Patient"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Home");
                }


                // Default dashboard
                return RedirectToAction(
                    "Index",
                    "Home");
            }


            ModelState.AddModelError(
                string.Empty,
                "Invalid email address or password.");

            return View(model);
        }


        // ============================================================
        // CHANGE PASSWORD
        // ============================================================

        // GET: Account/ChangePassword
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }


        // POST: Account/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // Get currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    nameof(LoginUser));
            }


            // ========================================================
            // PREVENT USING CURRENT PASSWORD AS NEW PASSWORD
            // ========================================================

            var samePassword =
                await _userManager.CheckPasswordAsync(
                    user,
                    model.NewPassword);

            if (samePassword)
            {
                ModelState.AddModelError(
                    nameof(model.NewPassword),
                    "The new password must be different from your current password.");

                return View(model);
            }


            // ========================================================
            // CHANGE PASSWORD USING ASP.NET CORE IDENTITY
            // ========================================================

            var result =
                await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);


            if (result.Succeeded)
            {
                // Keep user logged in after password change
                await _signInManager.RefreshSignInAsync(user);


                TempData["SuccessMessage"] =
                    "Your password has been changed successfully.";


                return RedirectToAction(
                    nameof(ChangePassword));
            }


            // ========================================================
            // DISPLAY IDENTITY ERRORS
            // ========================================================

            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordMismatch")
                {
                    ModelState.AddModelError(
                        nameof(model.CurrentPassword),
                        "The current password you entered is incorrect.");
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(model.NewPassword),
                        error.Description);
                }
            }


            return View(model);
        }


        // ============================================================
        // LOGOUT
        // ============================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                nameof(LoginUser));
        }
    }
}