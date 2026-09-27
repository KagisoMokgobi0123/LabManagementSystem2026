using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;

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


        // ============================================================
        // MAIN LOGIN / PORTAL SELECTION
        // URL: /Account/Login
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // ============================================================
        // ADMIN LOGIN
        // URL: /Account/AdminLogin
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View(new LoginViewModel());
        }


        // ============================================================
        // PATIENT LOGIN
        // URL: /Account/PatientLogin
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PatientLogin()
        {
            return View(new LoginViewModel());
        }


        // ============================================================
        // DOCTOR LOGIN
        // URL: /Account/DoctorLogin
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult DoctorLogin()
        {
            return View(new LoginViewModel());
        }


        // ============================================================
        // LAB TECHNICIAN LOGIN
        // URL: /Account/TechnicianLogin
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TechnicianLogin()
        {
            return View(new LoginViewModel());
        }


        // ============================================================
        // LAB MANAGER LOGIN
        // URL: /Account/ManagerLogin
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ManagerLogin()
        {
            return View(new LoginViewModel());
        }


        // ============================================================
        // SHARED ROLE LOGIN
        // ============================================================

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleLogin(
            LoginViewModel model,
            string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid login portal.");

                return View("Login");
            }


            if (!ModelState.IsValid)
            {
                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Find user
            // --------------------------------------------------------

            var user = await _userManager.FindByEmailAsync(
                model.Email.Trim());


            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Verify role
            // --------------------------------------------------------

            var belongsToRole =
                await _userManager.IsInRoleAsync(
                    user,
                    role);


            if (!belongsToRole)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You are not authorised to access this portal.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Check account state
            //
            // Staff with a temporary password can still login so that
            // they can change the temporary password.
            // --------------------------------------------------------

            if (!user.IsActive &&
                !user.MustChangePassword)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is currently inactive.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Authenticate
            // --------------------------------------------------------

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);


            // --------------------------------------------------------
            // Locked account
            // --------------------------------------------------------

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has been temporarily locked. " +
                    "Please try again later.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Invalid password
            // --------------------------------------------------------

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Temporary password
            // --------------------------------------------------------

            if (user.MustChangePassword)
            {
                return RedirectToAction(
                    nameof(ChangeTemporaryPassword));
            }


            // --------------------------------------------------------
            // Account must be active
            // --------------------------------------------------------

            if (!user.IsActive)
            {
                await _signInManager.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Your account has not been activated.");

                return ReturnLoginView(role, model);
            }


            // --------------------------------------------------------
            // Update last login
            // --------------------------------------------------------

            user.LastLoginDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);


            _logger.LogInformation(
                "User {Email} logged into the {Role} portal.",
                user.Email,
                role);


            // --------------------------------------------------------
            // Dashboard
            // --------------------------------------------------------

            return RedirectToDashboard(role);
        }


        // ============================================================
        // RETURN CORRECT LOGIN PAGE
        // ============================================================

        private IActionResult ReturnLoginView(
            string role,
            LoginViewModel model)
        {
            return role switch
            {
                "Administrator" =>
                    View("AdminLogin", model),

                "Patient" =>
                    View("PatientLogin", model),

                "Doctor" =>
                    View("DoctorLogin", model),

                "LabTechnician" =>
                    View("TechnicianLogin", model),

                "Manager" =>
                    View("ManagerLogin", model),

                _ =>
                    View("PatientLogin", model)
            };
        }


        // ============================================================
        // DASHBOARD REDIRECTION
        // ============================================================

        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Administrator" =>
                    RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Administrator" }),

                "Patient" =>
                    RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Patient" }),

                "Doctor" =>
                    RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Doctor" }),

                "LabTechnician" =>
                    RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Technician" }),

                "Manager" =>
                    RedirectToAction(
                        "Index",
                        "Home",
                        new { area = "Manager" }),

                _ =>
                    RedirectToAction(
                        nameof(Login))
            };
        }


        // ============================================================
        // CHANGE TEMPORARY PASSWORD
        // ============================================================

        [Authorize]
        [HttpGet]
        public IActionResult ChangeTemporaryPassword()
        {
            return View(
                new ChangeTemporaryPasswordViewModel());
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTemporaryPassword(
            ChangeTemporaryPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user =
                await _userManager.GetUserAsync(User);


            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    nameof(Login));
            }


            var result =
                await _userManager.ChangePasswordAsync(
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


            // --------------------------------------------------------
            // Activate account
            // --------------------------------------------------------

            user.MustChangePassword = false;

            user.IsActive = true;


            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }


            // Refresh authentication cookie
            await _signInManager.RefreshSignInAsync(user);


            TempData["Success"] =
                "Your password has been changed successfully.";


            // --------------------------------------------------------
            // Determine user's role
            // --------------------------------------------------------

            var roles =
                await _userManager.GetRolesAsync(user);


            var role =
                roles.FirstOrDefault();


            if (string.IsNullOrWhiteSpace(role))
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    nameof(Login));
            }


            return RedirectToDashboard(role);
        }


        // ============================================================
        // LOGOUT
        // ============================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var email =
                User.Identity?.Name;


            await _signInManager.SignOutAsync();


            _logger.LogInformation(
                "User {Email} logged out.",
                email);


            return RedirectToAction(
                nameof(Login));
        }


        // ============================================================
        // ACCESS DENIED
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}