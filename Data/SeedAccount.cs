
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;

namespace NMB_HLabSys.Data
{
    public static class SeedAccount
    {
        public const string DoctorRole = "Doctor";
        public const string LabTechnicianRole = "LabTechnician";
        public const string ManagerRole = "Manager";
        public const string PatientRole = "Patient";
        public const string AdministratorRole = "Administrator";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var context =
                services.GetRequiredService<ApplicationDbContext>();

            var environment =
                services.GetRequiredService<IHostEnvironment>();

            var configuration =
                services.GetRequiredService<IConfiguration>();

            await CreateRolesAsync(roleManager);

            // TEMPORARY DEVELOPMENT-ONLY PASSWORD RESET
            // Enable explicitly using configuration.
            // Remove after Allan can sign in successfully.
            if (environment.IsDevelopment() &&
                configuration.GetValue<bool>(
                    "DevelopmentReset:ResetAllanPassword"))
            {
                await ResetAllanPasswordForTestingAsync(
                    userManager,
                    configuration);
            }

            await CreateUserAsync(
                userManager,
                "administrator@nmbhlab.co.za",
                "Admin@123",
                "John",
                "Administrator",
                AdministratorRole);

            await CreateDoctorAsync(
                userManager,
                context,
                "rkhauli646@gmail.com",
                "Doctor@123",
                "John",
                "Johnson",
                "MP0000000");

            await CreateTechnicianAsync(
                userManager,
                context,
                email: "technician@nmbhlab.co.za",
                password: "Tech@1234",
                firstName: "Thabo",
                lastName: "Technician",
                idNumber: "8001015009087",
                employeeNumber: "TECH001");

            await CreateTechnicianAsync(
                userManager,
                context,
                email: "technician2@nmbhlab.co.za",
                password: "Tech@1234",
                firstName: "Rudzani",
                lastName: "Khauli",
                idNumber: "0201015009087",
                employeeNumber: "TECH004");

            // Allan already exists with LabTechnician role
            // and technician profile TECH22.
            // Do not create a duplicate Allan account.

            await CreateUserAsync(
                userManager,
                "manager@nmbhlab.co.za",
                "Manager@123",
                "Sarah",
                "Manager",
                ManagerRole);

            await CreateUserAsync(
                userManager,
                "patient@nmbhlab.co.za",
                "Patient@123",
                "Lerato",
                "Patient",
                PatientRole);
        }

        private static async Task ResetAllanPasswordForTestingAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            const string email = "allankhauli@gmail.com";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Allan Khauli's account was not found.");
            }

            if (!await userManager.IsInRoleAsync(
                    user, LabTechnicianRole))
            {
                throw new InvalidOperationException(
                    "Allan's account does not have the LabTechnician role.");
            }

            var temporaryPassword =
                configuration["DevelopmentReset:AllanTemporaryPassword"];

            if (string.IsNullOrWhiteSpace(temporaryPassword))
            {
                throw new InvalidOperationException(
                    "Configure DevelopmentReset:AllanTemporaryPassword " +
                    "using development user secrets.");
            }

            // If this password is already set, do not reset again.
            if (await userManager.CheckPasswordAsync(
                    user, temporaryPassword))
            {
                Console.WriteLine(
                    "Allan's development password is already configured.");

                return;
            }

            var resetToken =
                await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(
                user,
                resetToken,
                temporaryPassword);

            EnsureSucceeded(
                result,
                "reset Allan Khauli's development password");

            Console.WriteLine(
                "Allan Khauli's development password reset succeeded.");
        }

        private static async Task CreateRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                DoctorRole,
                LabTechnicianRole,
                ManagerRole,
                PatientRole,
                AdministratorRole
            };

            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                EnsureSucceeded(
                    result,
                    $"create role '{roleName}'");
            }
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    IsActive = true,
                    MustChangePassword = true,
                    IsProfileCompleted = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result =
                    await userManager.CreateAsync(user, password);

                EnsureSucceeded(
                    result,
                    $"create user '{email}'");
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result =
                    await userManager.AddToRoleAsync(user, role);

                EnsureSucceeded(
                    result,
                    $"assign role '{role}' to '{email}'");
            }

            return user;
        }

        private static async Task CreateDoctorAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string firstName,
            string lastName,
            string hpcsaNumber)
        {
            var user = await CreateUserAsync(
                userManager,
                email,
                password,
                firstName,
                lastName,
                DoctorRole);

            var exists = await context.Set<DoctorProfile>()
                .AnyAsync(p => p.UserId == user.Id);

            if (exists)
                return;

            context.Set<DoctorProfile>().Add(
                new DoctorProfile
                {
                    UserId = user.Id,
                    HPCSANumber = hpcsaNumber
                });

            await context.SaveChangesAsync();
        }

        private static async Task CreateTechnicianAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string firstName,
            string lastName,
            string idNumber,
            string employeeNumber,
            string? phoneNumber = null)
        {
            var user = await CreateUserAsync(
                userManager,
                email,
                password,
                firstName,
                lastName,
                LabTechnicianRole);

            var profiles = context.Set<TechnicianProfile>();

            var existingProfile = await profiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (existingProfile == null)
            {
                var conflict = await profiles.AnyAsync(p =>
                    p.IdNumber == idNumber ||
                    p.EmployeeNumber == employeeNumber);

                if (conflict)
                {
                    throw new InvalidOperationException(
                        $"The ID number or employee number for " +
                        $"'{email}' is already assigned to another technician.");
                }

                profiles.Add(new TechnicianProfile
                {
                    UserId = user.Id,
                    IdNumber = idNumber,
                    EmployeeNumber = employeeNumber
                });

                await context.SaveChangesAsync();
            }
            else if (
                existingProfile.IdNumber != idNumber ||
                existingProfile.EmployeeNumber != employeeNumber)
            {
                // Never overwrite identification details automatically.
                throw new InvalidOperationException(
                    $"Existing profile for '{email}' has different " +
                    "identification details. Review it manually.");
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var cleanedPhone = phoneNumber.Trim();

                if (user.PhoneNumber != cleanedPhone)
                {
                    var result =
                        await userManager.SetPhoneNumberAsync(
                            user,
                            cleanedPhone);

                    EnsureSucceeded(
                        result,
                        $"set phone number for '{email}'");
                }
            }

            Console.WriteLine(
                $"Technician seed checked: " +
                $"{firstName} {lastName} ({employeeNumber})");
        }

        private static void EnsureSucceeded(
            IdentityResult result,
            string operation)
        {
            if (result.Succeeded)
                return;

            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Could not {operation}. Errors: {errors}");
        }
    }
}
