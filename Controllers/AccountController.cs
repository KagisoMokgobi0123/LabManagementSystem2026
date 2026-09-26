using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Controllers
{
  

    namespace NMB_HLabSys.Controllers
    {
        public class AccountController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly ILogger<AccountController> _logger;

            public AccountController(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<AccountController> logger)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _logger = logger;
            }

            // =========================
            // LOGIN
            // =========================

            [AllowAnonymous]
            [HttpGet]
            public IActionResult Login(string? returnUrl = null)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["ReturnUrl"] = returnUrl;

                return View(new LoginViewModel());
            }

            [AllowAnonymous]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(
                LoginViewModel model,
                string? returnUrl = null)
            {
                ViewData["ReturnUrl"] = returnUrl;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

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
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.UtcNow;

                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation(
                        "User {Email} logged in.",
                        user.Email);

                    if (!string.IsNullOrEmpty(returnUrl)
                        && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return await RedirectByRole(user);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Your account has been temporarily locked.");

                    return View(model);
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            // =========================
            // REGISTER
            // =========================

            [AllowAnonymous]
            [HttpGet]
            public IActionResult Register()
            {
                return View(new RegisterViewModel());
            }

            [AllowAnonymous]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var existingUser =
                    await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError(
                        nameof(model.Email),
                        "An account with this email address already exists.");

                    return View(model);
                }

                var user = new ApplicationUser
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),

                    UserName = model.Email.Trim(),
                    Email = model.Email.Trim(),

                    CreatedDate = DateTime.UtcNow
                };

                var result =
                    await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Public registration creates Patient accounts.
                    await _userManager.AddToRoleAsync(user, "Patient");

                    await _signInManager.SignInAsync(
                        user,
                        isPersistent: false);

                    _logger.LogInformation(
                        "New patient registered: {Email}",
                        user.Email);

                    TempData["Success"] =
                        "Your account has been created successfully.";

                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Patient" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // =========================
            // LOGOUT
            // =========================

            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(Login));
            }

            // =========================
            // ROLE REDIRECTION
            // =========================

            private async Task<IActionResult> RedirectByRole(
                ApplicationUser user)
            {
                if (await _userManager.IsInRoleAsync(user, "Administrator"))
                {
                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Administrator" });
                }

                if (await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Manager" });
                }

                if (await _userManager.IsInRoleAsync(user, "Technician"))
                {
                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Technician" });
                }

                if (await _userManager.IsInRoleAsync(user, "Doctor"))
                {
                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Doctor" });
                }

                if (await _userManager.IsInRoleAsync(user, "Patient"))
                {
                    return RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Patient" });
                }

                return RedirectToAction("Index", "Home");
            }
        }
    }
}