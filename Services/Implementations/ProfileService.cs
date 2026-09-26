using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }


        // ============================================================
        // GET PROFILE
        // ============================================================

        public async Task<CompleteProfileViewModel?>
            GetProfileAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var profile =
                await _context.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationUserId == userId);

            if (profile == null)
            {
                return null;
            }

            /*
             * --------------------------------------------------------
             * This method belongs ONLY to the profile-completion
             * workflow.
             *
             * Do not add FirstName, LastName, or edit-only properties
             * here.
             * --------------------------------------------------------
             */

            return new CompleteProfileViewModel
            {
                Title =
                    profile.Title ?? default,

                Gender =
                    profile.Gender ?? default,

                IDNumber =
                    profile.IDNumber ?? string.Empty,

                DateOfBirth =
                    profile.DateOfBirth ?? DateTime.MinValue,

                Ethnicity =
                    profile.Ethnicity ?? default,

                Country =
                    profile.Country ?? string.Empty,

                ProvinceId =
                    profile.ProvinceId ?? 0,

                CityId =
                    profile.CityId ?? 0,

                Address1 =
                    profile.Address1 ?? string.Empty,

                Address2 =
                    profile.Address2,

                PostalCode =
                    profile.PostalCode ?? string.Empty
            };
        }


        // ============================================================
        // GET EDIT PROFILE
        // ============================================================

        public async Task<EditProfileViewModel?>
            GetEditProfileAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var profile =
                await _context.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationUserId == userId);

            if (profile == null)
            {
                return null;
            }

            /*
             * --------------------------------------------------------
             * EditProfileViewModel contains non-nullable form fields,
             * while UserProfile intentionally contains nullable
             * database fields.
             *
             * Therefore we explicitly provide safe defaults here.
             * --------------------------------------------------------
             */

            return new EditProfileViewModel
            {
                // ----------------------------------------------------
                // ApplicationUser
                // ----------------------------------------------------

                Email =
                    user.Email ?? string.Empty,


                // ----------------------------------------------------
                // UserProfile
                // ----------------------------------------------------

                FirstName =
                    profile.FirstName ?? string.Empty,

                LastName =
                    profile.LastName ?? string.Empty,

                Title =
                    profile.Title ?? default,

                Gender =
                    profile.Gender ?? default,

                IDNumber =
                    profile.IDNumber ?? string.Empty,

                DateOfBirth =
                    profile.DateOfBirth ?? DateTime.MinValue,

                Ethnicity =
                    profile.Ethnicity ?? default,

                Country =
                    profile.Country ?? string.Empty,

                ProvinceId =
                    profile.ProvinceId ?? 0,

                CityId =
                    profile.CityId ?? 0,

                Address1 =
                    profile.Address1 ?? string.Empty,

                Address2 =
                    profile.Address2,

                PostalCode =
                    profile.PostalCode ?? string.Empty,

                ExistingProfileImagePath =
                    profile.ProfileImagePath
            };
        }


        // ============================================================
        // PROFILE COMPLETION STATUS
        // ============================================================

        public async Task<bool>
            IsProfileCompletedAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (!user.IsProfileCompleted)
            {
                return false;
            }

            /*
             * Protect against an inconsistent database state where
             * ApplicationUser says the profile is complete but the
             * UserProfile row does not exist.
             */

            var profileExists =
                await _context.UserProfiles
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ApplicationUserId == userId);

            return profileExists;
        }


        // ============================================================
        // COMPLETE PROFILE
        // ============================================================

        public async Task<bool>
            CompleteProfileAsync(
                string userId,
                CompleteProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                model == null)
            {
                return false;
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var profile =
                await _context.UserProfiles
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationUserId == userId);

            if (profile == null)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * IMPORTANT:
             *
             * FirstName and LastName are NOT touched.
             *
             * They were captured during registration.
             *
             * CompleteProfileViewModel only supplies the remaining
             * profile information.
             * --------------------------------------------------------
             */

            profile.Title =
                model.Title;

            profile.Gender =
                model.Gender;

            profile.IDNumber =
                model.IDNumber?.Trim();

            profile.DateOfBirth =
                model.DateOfBirth;

            profile.Ethnicity =
                model.Ethnicity;

            profile.Country =
                model.Country?.Trim();

            profile.ProvinceId =
                model.ProvinceId;

            profile.CityId =
                model.CityId;

            profile.Address1 =
                model.Address1?.Trim();

            profile.Address2 =
                string.IsNullOrWhiteSpace(
                    model.Address2)
                    ? null
                    : model.Address2.Trim();

            profile.PostalCode =
                model.PostalCode?.Trim();

            /*
             * --------------------------------------------------------
             * Profile image
             *
             * Only use this if CompleteProfileViewModel actually has
             * ProfileImage.
             * --------------------------------------------------------
             */

            if (model.ProfileImage != null)
            {
                var imagePath =
                    await SaveProfileImageAsync(
                        userId,
                        model.ProfileImage);

                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    return false;
                }

                profile.ProfileImagePath =
                    imagePath;
            }

            profile.UpdatedDate =
                DateTime.UtcNow;

            /*
             * The profile becomes complete only after the information
             * has been successfully persisted.
             */

            user.IsProfileCompleted =
                true;

            try
            {
                await _context.SaveChangesAsync();

                var result =
                    await _userManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }


        // ============================================================
        // UPDATE ACCOUNT PROFILE
        // ============================================================

        public async Task<bool>
            UpdateAccountProfileAsync(
                string userId,
                EditProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                model == null)
            {
                return false;
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var profile =
                await _context.UserProfiles
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationUserId == userId);

            if (profile == null)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * First Name / Last Name
             * --------------------------------------------------------
             *
             * These are editable from the Edit Profile workflow.
             *
             * They were initially created during registration but can
             * subsequently be changed here.
             */

            profile.FirstName =
                model.FirstName.Trim();

            profile.LastName =
                model.LastName.Trim();


            /*
             * --------------------------------------------------------
             * PROFILE INFORMATION
             * --------------------------------------------------------
             */

            profile.Title =
                model.Title;

            profile.Gender =
                model.Gender;

            profile.IDNumber =
                model.IDNumber?.Trim();

            profile.DateOfBirth =
                model.DateOfBirth;

            profile.Ethnicity =
                model.Ethnicity;

            profile.Country =
                model.Country?.Trim();

            profile.ProvinceId =
                model.ProvinceId;

            profile.CityId =
                model.CityId;

            profile.Address1 =
                model.Address1?.Trim();

            profile.Address2 =
                string.IsNullOrWhiteSpace(
                    model.Address2)
                    ? null
                    : model.Address2.Trim();

            profile.PostalCode =
                model.PostalCode?.Trim();


            /*
             * --------------------------------------------------------
             * PROFILE IMAGE
             * --------------------------------------------------------
             */

            if (model.ProfileImage != null)
            {
                var imagePath =
                    await SaveProfileImageAsync(
                        userId,
                        model.ProfileImage);

                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    return false;
                }

                profile.ProfileImagePath =
                    imagePath;
            }


            /*
             * --------------------------------------------------------
             * PASSWORD
             * --------------------------------------------------------
             *
             * Password is optional.
             *
             * If NewPassword is empty, the current password remains
             * unchanged.
             */

            if (!string.IsNullOrWhiteSpace(
                    model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(
                        model.CurrentPassword))
                {
                    return false;
                }

                var currentPasswordValid =
                    await _userManager.CheckPasswordAsync(
                        user,
                        model.CurrentPassword);

                if (!currentPasswordValid)
                {
                    return false;
                }

                var passwordResult =
                    await _userManager.ChangePasswordAsync(
                        user,
                        model.CurrentPassword,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    return false;
                }
            }


            /*
             * --------------------------------------------------------
             * SAVE
             * --------------------------------------------------------
             */

            profile.UpdatedDate =
                DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        // ============================================================
        // SAVE PROFILE IMAGE
        // ============================================================

        private async Task<string?>
            SaveProfileImageAsync(
                string userId,
                IFormFile image)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                image == null ||
                image.Length == 0)
            {
                return null;
            }

            const long maximumFileSize =
                5 * 1024 * 1024;

            if (image.Length > maximumFileSize)
            {
                return null;
            }

            var extension =
                Path.GetExtension(
                    image.FileName)
                    .ToLowerInvariant();

            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".webp"
                };

            if (!allowedExtensions.Contains(
                    extension))
            {
                return null;
            }

            var uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "profile-images");

            Directory.CreateDirectory(
                uploadsFolder);

            var fileName =
                $"{userId}_{Guid.NewGuid():N}{extension}";

            var physicalPath =
                Path.Combine(
                    uploadsFolder,
                    fileName);

            await using var stream =
                new FileStream(
                    physicalPath,
                    FileMode.Create);

            await image.CopyToAsync(stream);

            return
                $"/uploads/profile-images/{fileName}";
        }
    }
}
