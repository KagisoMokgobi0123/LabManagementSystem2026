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
        // Login

        [AllowAnonymous]//Add this(KG)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]//Add this(KG)
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


  
        // ADMIN LOGIN

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View(new LoginViewModel());
        }


        // Patient login

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PatientLogin()
        {
            return View(new LoginViewModel());
        }
   
        // Doc login

        [AllowAnonymous]
        [HttpGet]
        public IActionResult DoctorLogin()
        {
            return View(new LoginViewModel());
        }

        // Lab technician login

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TechnicianLogin()
        {
            return View(new LoginViewModel());
        }


        // Manager login

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ManagerLogin()
        {
            return View(new LoginViewModel());
        }

        // Shared  role login

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


            // Find user
            var user = await _userManager.FindByEmailAsync(
                model.Email.Trim());


            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }

            // Verify role
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



            // Check account state
            // Staff with a temporary password can still login so that they can change the temporary password.
    

            if (!user.IsActive &&
                !user.MustChangePassword)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is currently inactive.");

                return ReturnLoginView(role, model);
            }



            // Authenticate
            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);



            // Locked account
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has been temporarily locked. " +
                    "Please try again later.");

                return ReturnLoginView(role, model);
            }


            // Invalid password

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }



            // Temporary password
            if (user.MustChangePassword)
            {
                return RedirectToAction(
                    nameof(ChangeTemporaryPassword));
            }


            // Account must be active
            if (!user.IsActive)
            {
                await _signInManager.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Your account has not been activated.");

                return ReturnLoginView(role, model);
            }

            // Update last login
            user.LastLoginDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);


            _logger.LogInformation(
                "User {Email} logged into the {Role} portal.",
                user.Email,
                role);


            // Redirect to the Dashboard depending on the role
            return RedirectToDashboard(role);
        }

        // Return the login page depending on the user.
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

        // This will redirect the user after entered correct login detials to their dahsboard

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



        // User will have to change their  temporary passwords
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

            // Retrieve an active account
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

            // Determine user's role
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



        // Safe logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Get currently logged-in user
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(Login));
            }


            // Get user's role before signing out
            var roles =
                await _userManager.GetRolesAsync(user);

            var role =
                roles.FirstOrDefault();

            var email =
                user.Email;


            // Sign user out
            await _signInManager.SignOutAsync();


            _logger.LogInformation(
                "User {Email} with role {Role} logged out.",
                email,
                role
            );


            // Redirect each role to its own login page
            return role switch
            {
                "Admin" =>
                    RedirectToAction(
                        nameof(AdminLogin)
                    ),

                "Manager" =>
                    RedirectToAction(
                        nameof(ManagerLogin)
                    ),

                "Doctor" =>
                    RedirectToAction(
                        nameof(DoctorLogin)
                    ),

                "LabTechnician" =>
                    RedirectToAction(
                        nameof(TechnicianLogin)
                    ),

                "Patient" =>
                    RedirectToAction(
                        nameof(PatientLogin)
                    ),

                _ =>
                    RedirectToAction(
                        nameof(Login)
                    )
            };
        }

        // For purpose of wrong login details access will be denied and this view will handle it.

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}