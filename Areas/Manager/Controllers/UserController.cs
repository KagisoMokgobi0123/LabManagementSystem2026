using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
using NMB_HLabSys.Models.Users.ViewModel;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await GetActiveUsers();

            return View(users);
        }


        // ============================================================
        // DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var doctor = await _context.DoctorProfiles
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            var technician = await _context.TechnicianProfiles
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            var testTypes = new List<object>();

            if (technician != null)
            {
                testTypes = await _context.TechnicianTestTypes
                    .Where(x =>
                        x.TechnicianId == user.Id &&
                        x.IsActive)
                    .Include(x => x.TestType)
                    .Select(x => new
                    {
                        x.TestTypeId,
                        TestType = x.TestType //!= null? x.TestType.Description: string.Empty
                    })
                    .Cast<object>()
                    .ToListAsync();
            }

            return Json(new
            {
                success = true,

                user = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.IsActive,
                    user.MustChangePassword,
                    user.CreatedDate,
                    user.LastLoginDate,

                    Role = roles.FirstOrDefault(),

                    HPCSANumber = doctor != null
                        ? doctor.HPCSANumber
                        : null,

                    IdNumber = technician != null
                        ? technician.IdNumber
                        : null,

                    EmployeeNumber = technician != null
                        ? technician.EmployeeNumber
                        : null,

                    TestTypes = testTypes
                }
            });
        }


        // ============================================================
        // CREATE DOCTOR
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(
            CreateDoctorViewModel model)
        {
            // --------------------------------------------------------
            // NORMALIZE INPUT
            // --------------------------------------------------------

            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;

            model.HPCSANumber =
                model.HPCSANumber?.Trim() ?? string.Empty;


            await ValidateDoctor(model);


            // --------------------------------------------------------
            // DUPLICATE EMAIL CHECK
            // --------------------------------------------------------

            var existingEmail =
                await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "A user with this e-mail address already exists.");
            }


            // --------------------------------------------------------
            // DUPLICATE HPCSA CHECK
            // --------------------------------------------------------

            bool duplicateHPCSA =
                await _context.DoctorProfiles
                    .AnyAsync(d =>
                        d.HPCSANumber.ToLower() ==
                        model.HPCSANumber.ToLower());

            if (duplicateHPCSA)
            {
                ModelState.AddModelError(
                    nameof(model.HPCSANumber),
                    "A doctor with this HPCSA number already exists.");
            }


            if (!ModelState.IsValid)
            {
                var users = await GetActiveUsers();

                return View("Index", users);
            }


            // --------------------------------------------------------
            // CREATE APPLICATION USER
            // --------------------------------------------------------

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,

                FirstName = model.FirstName,
                LastName = model.LastName,

                PhoneNumber = model.PhoneNumber,

                IsActive = true,
                MustChangePassword = true,
                IsProfileCompleted = true,

                CreatedDate = DateTime.UtcNow
            };


            // --------------------------------------------------------
            // GENERATE TEMPORARY PASSWORD
            // --------------------------------------------------------

            string temporaryPassword =
                GenerateTemporaryPassword();


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);


            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                var users = await GetActiveUsers();

                return View("Index", users);
            }


            // --------------------------------------------------------
            // ASSIGN DOCTOR ROLE
            // --------------------------------------------------------

            if (!await _roleManager.RoleExistsAsync("Doctor"))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole("Doctor"));
            }

            await _userManager.AddToRoleAsync(
                user,
                "Doctor");


            // --------------------------------------------------------
            // CREATE DOCTOR PROFILE
            // --------------------------------------------------------

            var doctorProfile = new DoctorProfile
            {
                UserId = user.Id,
                HPCSANumber = model.HPCSANumber
            };

            _context.DoctorProfiles.Add(
                doctorProfile);


            await _context.SaveChangesAsync();


            // --------------------------------------------------------
            // EMAIL PASSWORD
            // --------------------------------------------------------

            // TODO:
            // Send temporaryPassword to the doctor's
            // e-mail address using your email service.


            TempData["SuccessMessage"] =
                $"Doctor '{user.FullName}' was created successfully. " +
                "The temporary password has been generated.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // CREATE TECHNICIAN
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTechnician(
            CreateTechnicianViewModel model)
        {
            // --------------------------------------------------------
            // NORMALIZE INPUT
            // --------------------------------------------------------

            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;

            model.IdNumber =
                model.IdNumber?.Trim() ?? string.Empty;

            model.EmployeeNumber =
                model.EmployeeNumber?.Trim() ?? string.Empty;


            await ValidateTechnician(model);


            // --------------------------------------------------------
            // DUPLICATE EMAIL CHECK
            // --------------------------------------------------------

            var existingEmail =
                await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "A user with this e-mail address already exists.");
            }


            // --------------------------------------------------------
            // DUPLICATE SA ID CHECK
            // --------------------------------------------------------

            bool duplicateIdNumber =
                await _context.TechnicianProfiles
                    .AnyAsync(t =>
                        t.IdNumber == model.IdNumber);

            if (duplicateIdNumber)
            {
                ModelState.AddModelError(
                    nameof(model.IdNumber),
                    "A technician with this South African ID number already exists.");
            }


            // --------------------------------------------------------
            // DUPLICATE EMPLOYEE NUMBER CHECK
            // --------------------------------------------------------

            bool duplicateEmployeeNumber =
                await _context.TechnicianProfiles
                    .AnyAsync(t =>
                        t.EmployeeNumber.ToLower() ==
                        model.EmployeeNumber.ToLower());

            if (duplicateEmployeeNumber)
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "A technician with this employee number already exists.");
            }


            // --------------------------------------------------------
            // TEST TYPE REQUIREMENT
            // --------------------------------------------------------

            if (model.TestTypeIds == null ||
                !model.TestTypeIds.Any())
            {
                ModelState.AddModelError(
                    nameof(model.TestTypeIds),
                    "At least one test type must be assigned.");
            }


            // --------------------------------------------------------
            // VERIFY TEST TYPES
            // --------------------------------------------------------

            if (model.TestTypeIds != null &&
                model.TestTypeIds.Any())
            {
                var validTestTypeIds =
                    await _context.TestTypes
                        .Where(t =>
                            model.TestTypeIds.Contains(
                                t.TestTypeId))
                        .Select(t => t.TestTypeId)
                        .ToListAsync();


                var invalidTestTypeIds =
                    model.TestTypeIds
                        .Except(validTestTypeIds)
                        .ToList();


                if (invalidTestTypeIds.Any())
                {
                    ModelState.AddModelError(
                        nameof(model.TestTypeIds),
                        "One or more selected test types are invalid.");
                }
            }


            if (!ModelState.IsValid)
            {
                var users = await GetActiveUsers();

                return View("Index", users);
            }


            // --------------------------------------------------------
            // CREATE APPLICATION USER
            // --------------------------------------------------------

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,

                FirstName = model.FirstName,
                LastName = model.LastName,

                PhoneNumber = model.PhoneNumber,

                IsActive = true,
                MustChangePassword = true,
                IsProfileCompleted = true,

                CreatedDate = DateTime.UtcNow
            };


            // --------------------------------------------------------
            // GENERATE TEMPORARY PASSWORD
            // --------------------------------------------------------

            string temporaryPassword =
                GenerateTemporaryPassword();


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);


            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                var users = await GetActiveUsers();

                return View("Index", users);
            }


            // --------------------------------------------------------
            // ASSIGN TECHNICIAN ROLE
            // --------------------------------------------------------

            if (!await _roleManager.RoleExistsAsync(
                "Lab Technician"))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole("Lab Technician"));
            }

            await _userManager.AddToRoleAsync(
                user,
                "Lab Technician");


            // --------------------------------------------------------
            // CREATE TECHNICIAN PROFILE
            // --------------------------------------------------------

            var technicianProfile =
                new TechnicianProfile
                {
                    UserId = user.Id,

                    IdNumber =
                        model.IdNumber,

                    EmployeeNumber =
                        model.EmployeeNumber
                };


            _context.TechnicianProfiles.Add(
                technicianProfile);


            // --------------------------------------------------------
            // CREATE TEST TYPE ASSIGNMENTS
            // --------------------------------------------------------

            var managerId =
                _userManager.GetUserId(User);


            foreach (var testTypeId in
                     model.TestTypeIds.Distinct())
            {
                var assignment =
                    new TechnicianTestTypeModel
                    {
                        TechnicianId = user.Id,

                        TestTypeId = testTypeId,

                        ManagerId =
                            managerId ?? string.Empty,

                        AssignedDate =
                            DateTime.Now,

                        IsActive = true
                    };


                _context.TechnicianTestTypes.Add(
                    assignment);
            }


            await _context.SaveChangesAsync();


            // --------------------------------------------------------
            // EMAIL PASSWORD
            // --------------------------------------------------------

            // TODO:
            // Send temporaryPassword to the technician's
            // e-mail address using your email service.


            TempData["SuccessMessage"] =
                $"Lab technician '{user.FullName}' was created successfully. " +
                "The temporary password has been generated.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // DEACTIVATE USER
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            var user =
                await _userManager.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == id);


            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // PREVENT MANAGER DELETION
            // --------------------------------------------------------

            if (await _userManager.IsInRoleAsync(
                user,
                "Manager"))
            {
                TempData["ErrorMessage"] =
                    "Manager accounts cannot be deleted from this page.";

                return RedirectToAction(
                    nameof(Index));
            }


            // --------------------------------------------------------
            // SOFT DELETE
            // --------------------------------------------------------

            user.IsActive = false;


            // --------------------------------------------------------
            // DEACTIVATE TECHNICIAN ASSIGNMENTS
            // --------------------------------------------------------

            var technicianAssignments =
                await _context.TechnicianTestTypes
                    .Where(t =>
                        t.TechnicianId == user.Id &&
                        t.IsActive)
                    .ToListAsync();


            foreach (var assignment in
                     technicianAssignments)
            {
                assignment.IsActive = false;
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"User '{user.FullName}' was deactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // REACTIVATE USER
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index));


            var user =
                await _userManager.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == id);


            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            user.IsActive = true;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"User '{user.FullName}' was activated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        // ============================================================
        // VALIDATE DOCTOR
        // ============================================================

        private async Task ValidateDoctor(
            CreateDoctorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(
                model.FirstName))
            {
                ModelState.AddModelError(
                    nameof(model.FirstName),
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.LastName))
            {
                ModelState.AddModelError(
                    nameof(model.LastName),
                    "Last name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "E-mail address is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Contact number is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.HPCSANumber))
            {
                ModelState.AddModelError(
                    nameof(model.HPCSANumber),
                    "HPCSA number is required.");
            }


            if (model.HPCSANumber.Length > 50)
            {
                ModelState.AddModelError(
                    nameof(model.HPCSANumber),
                    "HPCSA number cannot exceed 50 characters.");
            }
        }


        // ============================================================
        // VALIDATE TECHNICIAN
        // ============================================================

        private async Task ValidateTechnician(
            CreateTechnicianViewModel model)
        {
            if (string.IsNullOrWhiteSpace(
                model.FirstName))
            {
                ModelState.AddModelError(
                    nameof(model.FirstName),
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.LastName))
            {
                ModelState.AddModelError(
                    nameof(model.LastName),
                    "Last name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "E-mail address is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Contact number is required.");
            }


            if (string.IsNullOrWhiteSpace(
                model.IdNumber))
            {
                ModelState.AddModelError(
                    nameof(model.IdNumber),
                    "South African ID number is required.");
            }


            if (model.IdNumber.Length != 13 ||
                !model.IdNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(
                    nameof(model.IdNumber),
                    "South African ID number must contain exactly 13 digits.");
            }


            if (string.IsNullOrWhiteSpace(
                model.EmployeeNumber))
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "Employee number is required.");
            }


            if (model.EmployeeNumber.Length > 50)
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "Employee number cannot exceed 50 characters.");
            }
        }


        // ============================================================
        // LOAD ACTIVE USERS
        // ============================================================

        private async Task<List<ApplicationUser>>
            GetActiveUsers()
        {
            return await _userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }


        // ============================================================
        // GENERATE TEMPORARY PASSWORD
        // ============================================================

        private string GenerateTemporaryPassword()
        {
            return $"NMB@{Guid.NewGuid():N}".Substring(0, 14);
        }
    }
}