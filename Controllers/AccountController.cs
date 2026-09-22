using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            IAccountService accountService,
            IOtpService otpService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _accountService = accountService;
            _otpService = otpService;
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
        }


        // ============================================================
        // REGISTER
        // ============================================================

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            /*
             * --------------------------------------------------------
             * Use RegisterWithResultAsync instead of RegisterAsync.
             *
             * RegisterWithResultAsync provides detailed registration
             * errors instead of the old generic:
             *
             * "Registration failed. Email may already exist."
             * --------------------------------------------------------
             */

            var result =
                await _accountService
                    .RegisterWithResultAsync(model);

            if (!result.Succeeded)
            {
                /*
                 * Add every service-level registration error to
                 * ModelState so the registration view can display
                 * the actual reason for failure.
                 */

                if (result.Errors != null &&
                    result.Errors.Length > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error);
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        string.IsNullOrWhiteSpace(
                            result.ErrorMessage)
                            ? "Registration failed."
                            : result.ErrorMessage);
                }

                return View(model);
            }

            /*
             * --------------------------------------------------------
             * Registration succeeded.
             *
             * Patient:
             * - Account is already active.
             * - Patient can proceed to login.
             *
             * Staff:
             * - Account is inactive.
             * - Temporary password was generated.
             * - Activation email was sent where possible.
             * - User must activate before login.
             *
             * Both flows eventually go to Login.
             * --------------------------------------------------------
             */

            return RedirectToAction(
                nameof(Login));
        }


        // ============================================================
        // ACTIVATE ACCOUNT
        // ============================================================

        // GET: /Account/Activate
        [HttpGet]
        public IActionResult Activate(
            string email)
        {
            var model =
                new ActivateAccountViewModel
                {
                    Email =
                        email?.Trim() ?? string.Empty
                };

            return View(model);
        }


        // POST: /Account/Activate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(
            ActivateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _accountService
                    .ActivateAccountAsync(model);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Account activation failed. " +
                    "Please verify your email address, " +
                    "temporary password, and new password.");

                return View(model);
            }

            /*
             * Activation has completed successfully.
             *
             * The account is now:
             *
             * IsActive = true
             *
             * MustChangePassword = false
             *
             * Staff users still need to complete their shared
             * UserProfile after logging in.
             */

            return RedirectToAction(
                nameof(Login));
        }


        // ============================================================
        // LOGIN
        // ============================================================

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            /*
             * --------------------------------------------------------
             * Authenticate through AccountService.
             *
             * AccountService verifies:
             *
             * - User exists
             * - Account is active
             * - Temporary password activation is complete
             * - Password is correct
             * --------------------------------------------------------
             */

            var loginSucceeded =
                await _accountService
                    .LoginAsync(model);

            if (!loginSucceeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid login details, or your account " +
                    "has not yet been activated.");

                return View(model);
            }

            /*
             * --------------------------------------------------------
             * Load the Identity user after successful login.
             * --------------------------------------------------------
             */

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                await _accountService.LogoutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "The login email address is invalid.");

                return View(model);
            }

            var user =
                await _userManager
                    .FindByEmailAsync(email);

            if (user == null)
            {
                /*
                 * This should not normally happen because LoginAsync
                 * already found and authenticated the user.
                 *
                 * Sign out defensively if the user cannot be loaded.
                 */

                await _accountService.LogoutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "The user account could not be loaded.");

                return View(model);
            }


            // ========================================================
            // GET USER ROLE
            // ========================================================

            var roles =
                await _userManager
                    .GetRolesAsync(user);

            var role =
                roles.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(role))
            {
                /*
                 * A valid authenticated user should always have a
                 * supported role.
                 *
                 * If the database contains an account without a role,
                 * do not allow access to an arbitrary dashboard.
                 */

                await _accountService.LogoutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Your account does not have a valid role.");

                return View(model);
            }


            // ========================================================
            // PATIENT
            // ========================================================

            /*
             * Patients have their own profile/registration workflow.
             *
             * Therefore they do NOT use the shared UserProfile.
             */

            if (role == "Patient")
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }


            // ========================================================
            // STAFF PROFILE COMPLETION
            // ========================================================

            /*
             * Administrator, Doctor, LabManager and LabTechnician
             * use the shared UserProfile.
             *
             * AccountService checks BOTH:
             *
             * 1. ApplicationUser.IsProfileCompleted
             * 2. Whether an actual UserProfile record exists.
             *
             * This is important because IsProfileCompleted by itself
             * is not sufficient to guarantee that UserProfile exists.
             */

            var requiresProfileCompletion =
                await _accountService
                    .RequiresProfileCompletionAsync(
                        user.Id);

            if (requiresProfileCompletion)
            {
                return RedirectToAction(
                    "Complete",
                    "Profile");
            }


            // ========================================================
            // STAFF DASHBOARDS
            // ========================================================

            switch (role)
            {
                case "Administrator":
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new
                        {
                            area = "Administrator"
                        });


                case "LabManager":
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new
                        {
                            area = "Manager"
                        });


                case "LabTechnician":
                    return RedirectToAction(
                        "Home",
                        "Home",
                        new
                        {
                            area = "Technician"
                        });


                case "Doctor":
                    return RedirectToAction(
        "Dashboard",
        "DoctorDashboard",
        new
        {
            area = "Doctor"
        });


                default:

                    /*
                     * Unknown/unsupported role.
                     *
                     * Do not leave the user authenticated if their
                     * role cannot be mapped to a dashboard.
                     */

                    await _accountService.LogoutAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        "Your account role is not supported.");

                    return View(model);
            }
        }


        // ============================================================
        // LOGOUT
        // ============================================================

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            return RedirectToAction(
                nameof(Login));
        }


        // ============================================================
        // FORGOT PASSWORD
        // ============================================================

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(
                    nameof(ForgotPassword));
            }

            /*
             * --------------------------------------------------------
             * Find Identity account.
             * --------------------------------------------------------
             */

            var user =
                await _userManager
                    .FindByEmailAsync(email);

            /*
             * --------------------------------------------------------
             * Do not expose whether an account exists.
             *
             * The response is intentionally the same whether the
             * email exists or not.
             * --------------------------------------------------------
             */

            if (user == null)
            {
                return RedirectToAction(
                    nameof(ForgotPassword));
            }


            // ========================================================
            // DETERMINE USER ROLE
            // ========================================================

            var roles =
                await _userManager
                    .GetRolesAsync(user);

            var role =
                roles.FirstOrDefault();


            // ========================================================
            // GET FIRST NAME
            // ========================================================

            string firstName =
                string.Empty;

            if (role == "Patient")
            {
                /*
                 * Patients store their personal information in the
                 * Patient entity rather than shared UserProfile.
                 */

                var patient =
                    await _context.Patients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.ApplicationUserId ==
                            user.Id);

                firstName =
                    patient?.Name?.Trim()
                    ?? string.Empty;
            }
            else
            {
                /*
                 * Staff users store their shared personal information
                 * in UserProfile.
                 *
                 * Note:
                 *
                 * A newly registered staff account may not yet have
                 * completed UserProfile.
                 *
                 * In that situation firstName simply remains empty.
                 */

                var profile =
                    await _context.UserProfiles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.ApplicationUserId ==
                            user.Id);

                firstName =
                    profile?.FirstName?.Trim()
                    ?? string.Empty;
            }


            // ========================================================
            // GENERATE OTP
            // ========================================================

            var otp =
                await _otpService
                    .GenerateOtpAsync(user.Id);


            // ========================================================
            // SEND OTP EMAIL
            // ========================================================

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                await _emailService
                    .SendPasswordResetOtpEmailAsync(
                        user.Email,
                        firstName,
                        otp);
            }


            // ========================================================
            // REDIRECT TO RESET PASSWORD
            // ========================================================

            return RedirectToAction(
                nameof(ResetPassword),
                new
                {
                    email = user.Email
                });
        }


        // ============================================================
        // RESET PASSWORD
        // ============================================================

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(
            string email)
        {
            var model =
                new ResetPasswordViewModel
                {
                    Email =
                        email?.Trim() ?? string.Empty
                };

            return View(model);
        }


        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email address is required.");

                return View(model);
            }


            // ========================================================
            // FIND USER
            // ========================================================

            var user =
                await _userManager
                    .FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid or expired password reset request.");

                return View(model);
            }


            // ========================================================
            // VALIDATE OTP
            // ========================================================

            var otpValid =
                await _otpService
                    .ValidateOtpAsync(
                        user.Id,
                        model.OTP);

            if (!otpValid)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid or expired OTP.");

                return View(model);
            }


            // ========================================================
            // GENERATE PASSWORD RESET TOKEN
            // ========================================================

            var token =
                await _userManager
                    .GeneratePasswordResetTokenAsync(
                        user);


            // ========================================================
            // RESET PASSWORD
            // ========================================================

            var resetResult =
                await _userManager
                    .ResetPasswordAsync(
                        user,
                        token,
                        model.NewPassword);

            if (!resetResult.Succeeded)
            {
                foreach (var error
                    in resetResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }


            // ========================================================
            // MARK OTP AS USED
            // ========================================================

            await _otpService
                .MarkOtpAsUsedAsync(
                    user.Id,
                    model.OTP);


            // ========================================================
            // PASSWORD RESET COMPLETE
            // ========================================================

            return RedirectToAction(
                nameof(Login));
        }
    }
}
