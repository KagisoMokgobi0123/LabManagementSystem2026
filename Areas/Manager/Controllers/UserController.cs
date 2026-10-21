using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
using NMB_HLabSys.Models.Users.ViewModel;
using System.Security.Cryptography;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SeedAccount.ManagerRole)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser>
            _userManager;

        private readonly RoleManager<IdentityRole>
            _roleManager;

        private readonly IEmailSender
            _emailSender;


        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadUsers();

            return View();
        }


 

        [HttpGet]
        public IActionResult EditDoctor()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditTechnician()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DoctorDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var doctor =
                await _context.DoctorProfiles
                    .AsNoTracking()
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d =>
                        d.UserId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Json(new
            {
                success = true,

                doctor = new
                {
                    doctor.UserId,

                    FirstName =
                        doctor.User?.FirstName,

                    LastName =
                        doctor.User?.LastName,

                    HPCSANumber =
                        doctor.HPCSANumber,

                    Email =
                        doctor.User?.Email,

                    PhoneNumber =
                        doctor.User?.PhoneNumber,

                    IsActive =
                        doctor.User?.IsActive ?? false,

                    MustChangePassword =
                        doctor.User?.MustChangePassword ?? false,

                    IsProfileCompleted =
                        doctor.User?.IsProfileCompleted ?? false,

                    CreatedDate =
                        doctor.User?.CreatedDate,

                    LastLoginDate =
                        doctor.User?.LastLoginDate
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> TechnicianDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var technician =
                await _context.TechnicianProfiles
                    .AsNoTracking()
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t =>
                        t.UserId == id);

            if (technician == null)
            {
                return NotFound();
            }

            return Json(new
            {
                success = true,

                technician = new
                {
                    technician.UserId,

                    FirstName =
                        technician.User?.FirstName,

                    LastName =
                        technician.User?.LastName,

                    technician.IdNumber,

                    technician.EmployeeNumber,

                    Email =
                        technician.User?.Email,

                    PhoneNumber =
                        technician.User?.PhoneNumber,

                    IsActive =
                        technician.User?.IsActive ?? false,

                    MustChangePassword =
                        technician.User?.MustChangePassword ?? false,

                    IsProfileCompleted =
                        technician.User?.IsProfileCompleted ?? false,

                    CreatedDate =
                        technician.User?.CreatedDate,

                    LastLoginDate =
                        technician.User?.LastLoginDate
                }
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(
            CreateDoctorViewModel model)
        {
            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.HPCSANumber =
                model.HPCSANumber?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;


            await ValidateDoctor(model);


            if (!ModelState.IsValid)
            {
                await LoadUsers();

                return View("Index");
            }


            string temporaryPassword =
                GenerateTemporaryPassword();


            var user =
                new ApplicationUser
                {
                    UserName = model.Email,

                    Email = model.Email,

                    FirstName = model.FirstName,

                    LastName = model.LastName,

                    PhoneNumber = model.PhoneNumber,

                    IsActive = true,

                    MustChangePassword = true,

                    IsProfileCompleted = true,

                    EmailConfirmed = true,

                    CreatedDate = DateTime.UtcNow
                };


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);


            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);

                await LoadUsers();

                return View("Index");
            }


            await EnsureRoleExists(
                SeedAccount.DoctorRole);


            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    SeedAccount.DoctorRole);


            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                AddIdentityErrors(roleResult);

                await LoadUsers();

                return View("Index");
            }


            var doctorProfile =
                new DoctorProfile
                {
                    UserId = user.Id,

                    HPCSANumber =
                        model.HPCSANumber
                };


            _context.DoctorProfiles.Add(
                doctorProfile);


            await _context.SaveChangesAsync();


            await SendTemporaryPasswordEmail(
                user,
                temporaryPassword,
                "Doctor");


            TempData["SuccessMessage"] =
                $"Doctor '{user.FullName}' was created successfully. " +
                "The temporary password has been e-mailed to the doctor.";


            return RedirectToAction(
                nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTechnician(
            CreateTechnicianViewModel model)
        {
            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.IdNumber =
                model.IdNumber?.Trim() ?? string.Empty;

            model.EmployeeNumber =
                model.EmployeeNumber?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;


            await ValidateTechnician(model);


            if (!ModelState.IsValid)
            {
                await LoadUsers();

                return View("Index");
            }


            string temporaryPassword =
                GenerateTemporaryPassword();


            var user =
                new ApplicationUser
                {
                    UserName = model.Email,

                    Email = model.Email,

                    FirstName = model.FirstName,

                    LastName = model.LastName,

                    PhoneNumber = model.PhoneNumber,

                    IsActive = true,

                    MustChangePassword = true,

                    IsProfileCompleted = true,

                    EmailConfirmed = true,

                    CreatedDate = DateTime.UtcNow
                };


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);


            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);

                await LoadUsers();

                return View("Index");
            }


            await EnsureRoleExists(
                SeedAccount.LabTechnicianRole);


            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    SeedAccount.LabTechnicianRole);


            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                AddIdentityErrors(roleResult);

                await LoadUsers();

                return View("Index");
            }


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


            await _context.SaveChangesAsync();


            await SendTemporaryPasswordEmail(
                user,
                temporaryPassword,
                "Lab Technician");


            TempData["SuccessMessage"] =
                $"Lab technician '{user.FullName}' was created successfully. " +
                "The temporary password has been e-mailed to the technician.";


            return RedirectToAction(
                nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(
            string UserId,
            CreateDoctorViewModel model)
        {

            UserId =
                UserId?.Trim() ?? string.Empty;


            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.HPCSANumber =
                model.HPCSANumber?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;


            if (string.IsNullOrWhiteSpace(UserId))
            {
                TempData["ErrorMessage"] =
                    "The doctor account could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }




            var doctor =
                await _context.DoctorProfiles
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d =>
                        d.UserId == UserId);


            if (doctor == null ||
                doctor.User == null)
            {
                TempData["ErrorMessage"] =
                    "The doctor account could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }



            await ValidateDoctor(
                model,
                UserId);


            if (!ModelState.IsValid)
            {
                await LoadUsers();

                return View("Index");
            }


            var user =
                doctor.User;



            user.FirstName =
                model.FirstName;

            user.LastName =
                model.LastName;

            user.Email =
                model.Email;

            user.UserName =
                model.Email;

            user.PhoneNumber =
                model.PhoneNumber;




            doctor.HPCSANumber =
                model.HPCSANumber;



            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);

                await LoadUsers();

                return View("Index");
            }



            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Doctor '{user.FullName}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTechnician(
            string UserId,
            CreateTechnicianViewModel model)
        {


            UserId =
                UserId?.Trim() ?? string.Empty;


            model.FirstName =
                model.FirstName?.Trim() ?? string.Empty;

            model.LastName =
                model.LastName?.Trim() ?? string.Empty;

            model.IdNumber =
                model.IdNumber?.Trim() ?? string.Empty;

            model.EmployeeNumber =
                model.EmployeeNumber?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            model.PhoneNumber =
                model.PhoneNumber?.Trim() ?? string.Empty;



            if (string.IsNullOrWhiteSpace(UserId))
            {
                TempData["ErrorMessage"] =
                    "The technician account could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }



            var technician =
                await _context.TechnicianProfiles
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t =>
                        t.UserId == UserId);


            if (technician == null ||
                technician.User == null)
            {
                TempData["ErrorMessage"] =
                    "The technician account could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }



            await ValidateTechnician(
                model,
                UserId);


            if (!ModelState.IsValid)
            {
                await LoadUsers();

                return View("Index");
            }


            var user =
                technician.User;




            user.FirstName =
                model.FirstName;

            user.LastName =
                model.LastName;

            user.Email =
                model.Email;

            user.UserName =
                model.Email;

            user.PhoneNumber =
                model.PhoneNumber;



            technician.IdNumber =
                model.IdNumber;

            technician.EmployeeNumber =
                model.EmployeeNumber;



            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);

                await LoadUsers();

                return View("Index");
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Lab technician '{user.FullName}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }

//soft delte

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] =
                    "User could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }


            var user =
                await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            user.IsActive =
                false;


            var result =
                await _userManager.UpdateAsync(user);


            if (!result.Succeeded)
            {
                AddIdentityErrors(result);

                await LoadUsers();

                return View("Index");
            }


            TempData["SuccessMessage"] =
                $"User '{user.FullName}' deactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] =
                    "User could not be identified.";

                return RedirectToAction(
                    nameof(Index));
            }


            var user =
                await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            user.IsActive =
                true;


            var result =
                await _userManager.UpdateAsync(user);


            if (!result.Succeeded)
            {
                AddIdentityErrors(result);

                await LoadUsers();

                return View("Index");
            }


            TempData["SuccessMessage"] =
                $"User '{user.FullName}' reactivated successfully.";


            return RedirectToAction(
                nameof(Index));
        }




        private async Task ValidateDoctor(
            CreateDoctorViewModel model,
            string? existingUserId = null)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                ModelState.AddModelError(
                    nameof(model.FirstName),
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                ModelState.AddModelError(
                    nameof(model.LastName),
                    "Last name is required.");
            }


            if (string.IsNullOrWhiteSpace(model.HPCSANumber))
            {
                ModelState.AddModelError(
                    nameof(model.HPCSANumber),
                    "HPCSA number is required.");
            }
            else
            {
                bool duplicateHPCSA =
                    await _context.DoctorProfiles
                        .AnyAsync(d =>
                            d.HPCSANumber.ToLower() ==
                            model.HPCSANumber.ToLower()
                            &&
                            (
                                existingUserId == null ||
                                d.UserId != existingUserId
                            ));

                if (duplicateHPCSA)
                {
                    ModelState.AddModelError(
                        nameof(model.HPCSANumber),
                        "A doctor with this HPCSA number already exists.");
                }
            }


            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "E-mail address is required.");
            }
            else
            {
                var existingUser =
                    await _userManager.FindByEmailAsync(
                        model.Email);

                if (existingUser != null &&
                    existingUser.Id != existingUserId)
                {
                    ModelState.AddModelError(
                        nameof(model.Email),
                        "A user with this e-mail address already exists.");
                }
            }


            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Contact number is required.");
            }
        }


        private async Task ValidateTechnician(
            CreateTechnicianViewModel model,
            string? existingUserId = null)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                ModelState.AddModelError(
                    nameof(model.FirstName),
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                ModelState.AddModelError(
                    nameof(model.LastName),
                    "Last name is required.");
            }


            if (string.IsNullOrWhiteSpace(model.IdNumber))
            {
                ModelState.AddModelError(
                    nameof(model.IdNumber),
                    "South African ID number is required.");
            }
            else
            {
                bool duplicateIdNumber =
                    await _context.TechnicianProfiles
                        .AnyAsync(t =>
                            t.IdNumber ==
                            model.IdNumber
                            &&
                            (
                                existingUserId == null ||
                                t.UserId != existingUserId
                            ));

                if (duplicateIdNumber)
                {
                    ModelState.AddModelError(
                        nameof(model.IdNumber),
                        "A technician with this South African ID number already exists.");
                }
            }


            if (string.IsNullOrWhiteSpace(model.EmployeeNumber))
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "Employee number is required.");
            }
            else
            {
                bool duplicateEmployeeNumber =
                    await _context.TechnicianProfiles
                        .AnyAsync(t =>
                            t.EmployeeNumber.ToLower() ==
                            model.EmployeeNumber.ToLower()
                            &&
                            (
                                existingUserId == null ||
                                t.UserId != existingUserId
                            ));

                if (duplicateEmployeeNumber)
                {
                    ModelState.AddModelError(
                        nameof(model.EmployeeNumber),
                        "A technician with this employee number already exists.");
                }
            }


            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "E-mail address is required.");
            }
            else
            {
                var existingUser =
                    await _userManager.FindByEmailAsync(
                        model.Email);

                if (existingUser != null &&
                    existingUser.Id != existingUserId)
                {
                    ModelState.AddModelError(
                        nameof(model.Email),
                        "A user with this e-mail address already exists.");
                }
            }


            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Contact number is required.");
            }
        }


        private async Task LoadUsers()
        {
            ViewBag.Doctors =
                await GetActiveDoctors();

            ViewBag.Technicians =
                await GetActiveTechnicians();
        }




        private async Task<List<DoctorProfile>>
            GetActiveDoctors()
        {
            return await _context.DoctorProfiles
                .AsNoTracking()
                .Include(d => d.User)
                .Where(d =>
                    d.User != null &&
                    d.User.IsActive)
                .OrderBy(d =>
                    d.User!.LastName)
                .ThenBy(d =>
                    d.User!.FirstName)
                .ToListAsync();
        }




        private async Task<List<TechnicianProfile>>
            GetActiveTechnicians()
        {
            return await _context.TechnicianProfiles
                .AsNoTracking()
                .Include(t => t.User)
                .Where(t =>
                    t.User != null &&
                    t.User.IsActive)
                .OrderBy(t =>
                    t.User!.LastName)
                .ThenBy(t =>
                    t.User!.FirstName)
                .ToListAsync();
        }



        private async Task EnsureRoleExists(
            string roleName)
        {
            bool roleExists =
                await _roleManager.RoleExistsAsync(
                    roleName);

            if (!roleExists)
            {
                var result =
                    await _roleManager.CreateAsync(
                        new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                e => e.Description));

                    throw new InvalidOperationException(
                        $"Could not create role '{roleName}'. " +
                        $"Errors: {errors}");
                }
            }
        }



        private string GenerateTemporaryPassword()
        {
            const string upper =
                "ABCDEFGHJKLMNPQRSTUVWXYZ";

            const string lower =
                "abcdefghijkmnopqrstuvwxyz";

            const string numbers =
                "23456789";

            const string special =
                "!@#$%&*";


            var password =
                new List<char>
                {
                    GetRandomCharacter(upper),
                    GetRandomCharacter(lower),
                    GetRandomCharacter(numbers),
                    GetRandomCharacter(special)
                };


            const string all =
                upper +
                lower +
                numbers +
                special;


            while (password.Count < 12)
            {
                password.Add(
                    GetRandomCharacter(all));
            }


            return new string(
                password
                    .OrderBy(_ =>
                        RandomNumberGenerator.GetInt32(
                            int.MaxValue))
                    .ToArray());
        }


        private char GetRandomCharacter(
            string characters)
        {
            int index =
                RandomNumberGenerator.GetInt32(
                    characters.Length);

            return characters[index];
        }



        private async Task SendTemporaryPasswordEmail(
            ApplicationUser user,
            string temporaryPassword,
            string userType)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(
                    $"Cannot send temporary password email because " +
                    $"the {userType} account does not have an e-mail address.");
            }


            string subject =
                "NMB Health Laboratory System - Account Created";


            string body =
                $@"
                <p>Dear {user.FirstName},</p>

                <p>
                    Your {userType} account for the
                    <strong>NMB Health Laboratory System</strong>
                    has been created.
                </p>

                <p>
                    <strong>Username:</strong>
                    {user.Email}
                    <br />

                    <strong>Temporary Password:</strong>
                    {temporaryPassword}
                </p>

                <p>
                    Please use these credentials to log in.
                    You will be required to change your password
                    when you log in for the first time.
                </p>

                <p>
                    For security reasons, please do not share
                    your password with anyone.
                </p>

                <p>
                    Regards,<br />
                    NMB Health Laboratory System
                </p>
                ";


            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                body);
        }


        private void AddIdentityErrors(
            IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }
    }
}