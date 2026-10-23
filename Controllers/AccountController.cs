using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Areas.Patient.Models;

namespace NMB_HLabSys.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        // ============================================================
        // REGISTER - GET
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // ============================================================
        // REGISTER - POST
        // ============================================================

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Role = "Patient";

            var email = model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email is required.");
                return View(model);
            }

            // Check for an existing email.
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            // Check for an existing South African ID number.
            if (!string.IsNullOrWhiteSpace(model.IdNumber))
            {
                var idExists = await _context.Patients
                    .AnyAsync(p => p.IDNumber == model.IdNumber);

                if (idExists)
                {
                    ModelState.AddModelError(
                        nameof(model.IdNumber),
                        "An account with this SA ID number already exists.");

                    return View(model);
                }
            }

            // Create the Identity user.
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = true,
                MustChangePassword = false,
                IsProfileCompleted = true,
                CreatedDate = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                    _logger.LogWarning(
                        "Registration failed for {Email}: {Error}",
                        email,
                        error.Description);
                }

                return View(model);
            }

            try
            {
                // Assign the Patient role.
                var roleResult = await _userManager.AddToRoleAsync(
                    user,
                    "Patient");

                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await _userManager.DeleteAsync(user);

                    return View(model);
                }

                // Create the associated patient profile.
                var patient = new PatientModel
                {
                    PatientId = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    Name = model.FirstName,
                    Surname = model.LastName,
                    IDNumber = model.IdNumber ?? "",
                    DateOfBirth = model.DateOfBirth ?? DateTime.Now,
                    ContactNumber = model.PhoneNumber ?? "",
                    Email = email,
                    Address = model.HomeAddress ?? "",
                    Suburb = "",
                    City = model.City ?? "",
                    Province = model.Province ?? "",
                    PostalCode = "",
                    CreatedDate = DateTime.Now
                };

                await using (var transaction =
                    await _context.Database.BeginTransactionAsync())
                {
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }

                // Sign in the new patient.
                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                _logger.LogInformation(
                    "New patient registered: {Email}",
                    email);

                return RedirectToAction(
                    "Index",
                    "Home",
                    new { area = "Patient" });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating patient profile for {Email}",
                    email);

                // Remove the partially created account if profile creation fails.
                await _userManager.DeleteAsync(user);

                ModelState.AddModelError(
                    string.Empty,
                    "Registration could not be completed. Please check your details and try again.");

                return View(model);
            }
        }

        // ============================================================
        // ALTERNATE REGISTRATION ROUTE
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return RedirectToAction(nameof(Register));
        }

        // ============================================================
        // LOGIN - GET
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginUser()
        {
            return RedirectToAction(nameof(PatientLogin));
        }

        // ============================================================
        // ROLE-SPECIFIC LOGIN PAGES
        // ============================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PatientLogin()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult DoctorLogin()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TechnicianLogin()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ManagerLogin()
        {
            return View(new LoginViewModel());
        }

        // ============================================================
        // ROLE LOGIN - POST
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

                return View("Login", model);
            }

            if (!ModelState.IsValid)
                return ReturnLoginView(role, model);

            var email = model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email address is required.");

                return ReturnLoginView(role, model);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }

            var belongsToRole = await _userManager.IsInRoleAsync(user, role);

            if (!belongsToRole)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You are not authorised to access this portal.");

                return ReturnLoginView(role, model);
            }

            if (!user.IsActive && !user.MustChangePassword)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is currently inactive.");

                return ReturnLoginView(role, model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has been temporarily locked. Please try again later.");

                return ReturnLoginView(role, model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return ReturnLoginView(role, model);
            }

            if (user.MustChangePassword)
            {
                return RedirectToAction(
                    nameof(ChangeTemporaryPassword));
            }

            if (!user.IsActive)
            {
                await _signInManager.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Your account has not been activated.");

                return ReturnLoginView(role, model);
            }

            user.LastLoginDate = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogWarning(
                    "Could not update last login for {Email}",
                    user.Email);
            }

            _logger.LogInformation(
                "User {Email} logged into the {Role} portal.",
                user.Email,
                role);

            return RedirectToDashboard(role);
        }

        private IActionResult ReturnLoginView(
            string role,
            LoginViewModel model)
        {
            return role switch
            {
                "Administrator" => View("AdminLogin", model),
                "Patient" => View("PatientLogin", model),
                "Doctor" => View("DoctorLogin", model),
                "LabTechnician" => View("TechnicianLogin", model),
                "Manager" => View("ManagerLogin", model),
                _ => View("Login", model)
            };
        }

        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Administrator" => RedirectToAction(
                    "Index", "Home", new { area = "Administrator" }),

                "Patient" => RedirectToAction(
                    "Index", "Home", new { area = "Patient" }),

                "Doctor" => RedirectToAction(
                    "Index", "Home", new { area = "Doctor" }),

                "LabTechnician" => RedirectToAction(
                    "Index", "Home", new { area = "Technician" }),

                "Manager" => RedirectToAction(
                    "Index", "Home", new { area = "Manager" }),

                _ => RedirectToAction(nameof(Login))
            };
        }

        // ============================================================
        // CHANGE TEMPORARY PASSWORD
        // ============================================================

        [Authorize]
        [HttpGet]
        public IActionResult ChangeTemporaryPassword()
        {
            return View(new ChangeTemporaryPasswordViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTemporaryPassword(
            ChangeTemporaryPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            user.MustChangePassword = false;

            // Do not automatically activate an account that an administrator
            // or manager has deliberately left inactive.
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] =
                "Your password has been changed successfully.";

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            // Prefer the role used to enter the portal, if available.
            var role = roles.FirstOrDefault(r =>
                r == "Administrator" ||
                r == "Patient" ||
                r == "Doctor" ||
                r == "LabTechnician" ||
                r == "Manager");

            if (role == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var email = user.Email;

            await _signInManager.SignOutAsync();

            _logger.LogInformation(
                "User {Email} with role {Role} logged out.",
                email,
                role);

            return role switch
            {
                "Administrator" => RedirectToAction(nameof(AdminLogin)),
                "Manager" => RedirectToAction(nameof(ManagerLogin)),
                "Doctor" => RedirectToAction(nameof(DoctorLogin)),
                "LabTechnician" => RedirectToAction(nameof(TechnicianLogin)),
                "Patient" => RedirectToAction(nameof(PatientLogin)),
                _ => RedirectToAction(nameof(Login))
            };
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