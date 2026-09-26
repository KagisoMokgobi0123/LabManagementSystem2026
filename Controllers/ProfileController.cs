using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Account.Enums;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users.Enums;
using NMB_HLabSys.Services.Interfaces;
using System.Security.Claims;

namespace NMB_HLabSys.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;
        private readonly ILocationService _locationService;

        public ProfileController(
            IProfileService profileService,
            ILocationService locationService,
            UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _locationService = locationService;
            _userManager = userManager;
        }


        // ============================================================
        // GET: /Profile/GetCities
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetCities(
            int provinceId)
        {
            if (provinceId <= 0)
            {
                return Json(
                    new List<SelectListItem>());
            }

            var cities =
                await _locationService
                    .GetCitiesAsync(provinceId);

            return Json(cities);
        }


        // ============================================================
        // GET: /Profile/Complete
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Complete()
        {
            var user =
                await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // --------------------------------------------------------
            // Patients do not use the shared UserProfile workflow.
            // --------------------------------------------------------

            if (await IsPatientAsync(user))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }

            // --------------------------------------------------------
            // If profile is already completed, go to profile index.
            // --------------------------------------------------------

            var completed =
                await _profileService
                    .IsProfileCompletedAsync(user.Id);

            if (completed)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            // --------------------------------------------------------
            // Get existing incomplete profile.
            //
            // CompleteProfileViewModel is intentionally used here.
            //
            // FirstName and LastName are NOT collected here because
            // they were already supplied during registration.
            // --------------------------------------------------------

            var model =
                await _profileService
                    .GetProfileAsync(user.Id);

            if (model == null)
            {
                model =
                    new CompleteProfileViewModel();
            }

            // --------------------------------------------------------
            // Populate location dropdowns.
            // --------------------------------------------------------

            await PopulateCompleteLocationListsAsync(
                model);

            return View(model);
        }


        // ============================================================
        // POST: /Profile/Complete
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(
            CompleteProfileViewModel model)
        {
            var user =
                await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // --------------------------------------------------------
            // Patients do not use the shared UserProfile workflow.
            // --------------------------------------------------------

            if (await IsPatientAsync(user))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }

            // --------------------------------------------------------
            // Prevent editing an already-completed profile through
            // the completion workflow.
            // --------------------------------------------------------

            var alreadyCompleted =
                await _profileService
                    .IsProfileCompletedAsync(user.Id);

            if (alreadyCompleted)
            {
                return await RedirectToRoleDashboardAsync(user);
            }

            // --------------------------------------------------------
            // Validate completion model.
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                await PopulateCompleteLocationListsAsync(
                    model);

                return View(model);
            }

            // --------------------------------------------------------
            // Complete existing UserProfile.
            // --------------------------------------------------------

            try
            {
                var result =
                    await _profileService
                        .CompleteProfileAsync(
                            user.Id,
                            model);

                if (!result)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to complete profile.");

                    await PopulateCompleteLocationListsAsync(
                        model);

                    return View(model);
                }

                return await RedirectToRoleDashboardAsync(
                    user);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred while completing your profile.");

                await PopulateCompleteLocationListsAsync(
                    model);

                return View(model);
            }
        }


        // ============================================================
        // GET: /Profile/Index
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user =
                await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // --------------------------------------------------------
            // Patients use their own profile/dashboard workflow.
            // --------------------------------------------------------

            if (await IsPatientAsync(user))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }

            // --------------------------------------------------------
            // Load shared UserProfile.
            // --------------------------------------------------------

            var profile =
                await _profileService
                    .GetProfileAsync(user.Id);

            if (profile == null)
            {
                return RedirectToAction(
                    nameof(Complete));
            }

            return View(profile);
        }


        // ============================================================
        // GET: /Profile/Edit
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user =
                await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // --------------------------------------------------------
            // Patients do not use the shared UserProfile workflow.
            // --------------------------------------------------------

            if (await IsPatientAsync(user))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }

            // --------------------------------------------------------
            // Get the dedicated EditProfileViewModel.
            //
            // This is intentionally separate from
            // CompleteProfileViewModel.
            // --------------------------------------------------------

            var model =
                await _profileService
                    .GetEditProfileAsync(user.Id);

            if (model == null)
            {
                return RedirectToAction(
                    nameof(Complete));
            }

            // --------------------------------------------------------
            // Populate edit-specific location dropdowns.
            // --------------------------------------------------------

            await PopulateEditLocationListsAsync(
                model);

            return View(model);
        }


        // ============================================================
        // POST: /Profile/Edit
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EditProfileViewModel model)
        {
            var user =
                await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // --------------------------------------------------------
            // Patients do not use the shared UserProfile workflow.
            // --------------------------------------------------------

            if (await IsPatientAsync(user))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Patient"
                    });
            }

            // --------------------------------------------------------
            // Validate edit model.
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                await PopulateEditLocationListsAsync(
                    model);

                return View(model);
            }

            // --------------------------------------------------------
            // Update existing account/profile.
            //
            // This is deliberately separate from
            // CompleteProfileAsync().
            // --------------------------------------------------------

            try
            {
                var result =
                    await _profileService
                        .UpdateAccountProfileAsync(
                            user.Id,
                            model);

                if (!result)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Profile update failed.");

                    await PopulateEditLocationListsAsync(
                        model);

                    return View(model);
                }

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred while updating your profile.");

                await PopulateEditLocationListsAsync(
                    model);

                return View(model);
            }
        }


        // ============================================================
        // CURRENT USER
        // ============================================================

        private async Task<ApplicationUser?>
            GetCurrentUserAsync()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _userManager
                .FindByIdAsync(userId);
        }


        // ============================================================
        // PATIENT CHECK
        // ============================================================

        private async Task<bool>
            IsPatientAsync(
                ApplicationUser user)
        {
            return await _userManager
                .IsInRoleAsync(
                    user,
                    UserRole.Patient.ToString());
        }


        // ============================================================
        // COMPLETE PROFILE LOCATION LISTS
        // ============================================================

        private async Task
            PopulateCompleteLocationListsAsync(
                CompleteProfileViewModel model)
        {
            // --------------------------------------------------------
            // Provinces
            // --------------------------------------------------------

            model.Provinces =
                await _locationService
                    .GetProvincesAsync();

            // --------------------------------------------------------
            // Cities
            // --------------------------------------------------------

            if (model.ProvinceId > 0)
            {
                model.Cities =
                    await _locationService
                        .GetCitiesAsync(
                            model.ProvinceId);
            }
            else
            {
                model.Cities =
                    new List<SelectListItem>();
            }
        }


        // ============================================================
        // EDIT PROFILE LOCATION LISTS
        // ============================================================

        private async Task
            PopulateEditLocationListsAsync(
                EditProfileViewModel model)
        {
            // --------------------------------------------------------
            // Provinces
            // --------------------------------------------------------

            model.Provinces =
                await _locationService
                    .GetProvincesAsync();

            // --------------------------------------------------------
            // Cities
            // --------------------------------------------------------

            if (model.ProvinceId > 0)
            {
                model.Cities =
                    await _locationService
                        .GetCitiesAsync(
                            model.ProvinceId);
            }
            else
            {
                model.Cities =
                    new List<SelectListItem>();
            }
        }


        // ============================================================
        // ROLE DASHBOARD REDIRECTION
        // ============================================================

        private async Task<IActionResult>
            RedirectToRoleDashboardAsync(
                ApplicationUser user)
        {
            var roles =
                await _userManager
                    .GetRolesAsync(user);

            var role =
                roles.FirstOrDefault();

            return role switch
            {
                "Administrator" =>
                    RedirectToAction(
                        "Index",
                        "Dashboard",
                        new
                        {
                            area = "Administrator"
                        }),

                "LabManager" =>
                    RedirectToAction(
                        "Index",
                        "Dashboard",
                        new
                        {
                            area = "Manager"
                        }),

                "LabTechnician" =>
                    RedirectToAction(
                        "Home",
                        "Home",
                        new
                        {
                            area = "Technician"
                        }),

                "Doctor" =>
                    RedirectToAction(
                        "Dashboard",
                        "DoctorDashboard",
                        new
                        {
                            area = "Doctor"
                        }),

                "Patient" =>
                    RedirectToAction(
                        "Index",
                        "Dashboard",
                        new
                        {
                            area = "Patient"
                        }),

                _ =>
                    RedirectToAction(
                        "Login",
                        "Account")
            };
        }
    }
}
