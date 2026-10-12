using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Data
{
    public static class SeedAccount
    {
        // ============================================================
        // ROLE NAMES
        // ============================================================

        public const string DoctorRole = "Doctor";
        public const string LabTechnicianRole = "LabTechnician";
        public const string ManagerRole = "Manager";
        public const string PatientRole = "Patient";
        public const string AdministratorRole = "Administrator";


        // ============================================================
        // SEED
        // ============================================================

        public static async Task SeedAsync(
            IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<
                    RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<
                    UserManager<ApplicationUser>>();

            // Database Context - FIX
            var context = services.GetRequiredService<
                ApplicationDbContext>();

            // ========================================================
            // CREATE ROLES
            // ========================================================

            await CreateRolesAsync(roleManager);


            // ========================================================
            // ADMINISTRATOR
            // ========================================================

            await CreateUserAsync(
                userManager,
                email: "administrator@nmbhlab.co.za",
                password: "Admin@123",
                firstName: "John",
                lastName: "Administrator",
                role: AdministratorRole);


            // ========================================================
            // DOCTOR
            // ========================================================

            await CreateDoctorAsync(
                userManager,
                email: "doctor@nmbhlab.co.za",
                password: "Doctor@123",
                firstName: "John",
                lastName: "Doctor",
                hpcsaNumber: "MP0000000");


            // ========================================================
            // LAB TECHNICIAN
            // ========================================================
await CreateTechnicianAsync(
    userManager,
    context,
    email: "technician@nmbhlab.co.za",
    password: "Tech@1234",
    firstName: "Thabo",
    lastName: "Technician",
    idNumber: "8001015009087",
    employeeNumber: "TECH001");

            // ========================================================
            // LAB TECHNICIAN
            // ========================================================


            await CreateTechnicianAsync(
                userManager,
                context,
                email: "technician2@nmbhlab.co.za",
                password: "Tech@1234",
                firstName: "Rudzani",
                lastName: "Khauli",
                idNumber: "0201015009087",
                employeeNumber: "TECH002");

            // ========================================================
            // MANAGER
            // ========================================================

            await CreateUserAsync(
                userManager,
                email: "manager@nmbhlab.co.za",
                password: "Manager@123",
                firstName: "Sarah",
                lastName: "Manager",
                role: ManagerRole);


            await CreateUserAsync(
                userManager,
                email: "manager@nmbhlab.co.za",
                password: "Manager@123",
                firstName: "Frank",
                lastName: "Smith",
                role: ManagerRole);


            // ========================================================
            // PATIENT
            // ========================================================

            await CreateUserAsync(
                userManager,
                email: "patient@nmbhlab.co.za",
                password: "Patient@123",
                firstName: "Lerato",
                lastName: "Patient",
                role: PatientRole);
        }


        // ============================================================
        // CREATE ROLES
        // ============================================================

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
                {
                    continue;
                }


                var result =
                    await roleManager.CreateAsync(
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


        // ============================================================
        // CREATE GENERAL USER
        // ============================================================

        private static async Task<ApplicationUser>
            CreateUserAsync(
                UserManager<ApplicationUser> userManager,
                string email,
                string password,
                string firstName,
                string lastName,
                string role)
        {
            var user =
                await userManager.FindByEmailAsync(email);


            // ========================================================
            // CREATE USER IF NOT FOUND
            // ========================================================

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

                    MustChangePassword = false,

                    IsProfileCompleted = true,

                    CreatedDate = DateTime.UtcNow
                };


                var createResult =
                    await userManager.CreateAsync(
                        user,
                        password);


                if (!createResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            createResult.Errors.Select(
                                e => e.Description));


                    throw new InvalidOperationException(
                        $"Could not create user '{email}'. " +
                        $"Errors: {errors}");
                }
            }


            // ========================================================
            // ASSIGN ROLE
            // ========================================================

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        user,
                        role);


                if (!roleResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            roleResult.Errors.Select(
                                e => e.Description));


                    throw new InvalidOperationException(
                        $"Could not assign role '{role}' " +
                        $"to '{email}'. " +
                        $"Errors: {errors}");
                }
            }


            return user;
        }


        // ============================================================
        // CREATE DOCTOR
        // ============================================================

        private static async Task CreateDoctorAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string hpcsaNumber)
        {
            var user =
                await CreateUserAsync(
                    userManager,
                    email,
                    password,
                    firstName,
                    lastName,
                    DoctorRole);


            // ========================================================
            // EXISTING PROFILE
            // ========================================================

            if (user.DoctorProfile != null)
            {
                return;
            }


            // ========================================================
            // CREATE DOCTOR PROFILE
            // ========================================================

            user.DoctorProfile =
                new DoctorProfile
                {
                    UserId = user.Id,

                    HPCSANumber = hpcsaNumber
                };


            await userManager.UpdateAsync(user);
        }


        // ============================================================
        // CREATE LAB TECHNICIAN
        // ============================================================


        private static async Task CreateTechnicianAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string firstName,
            string lastName,
            string idNumber,
            string employeeNumber)
        {
            // Create or retrieve the Identity account
            var user = await CreateUserAsync(
                userManager,
                email,
                password,
                firstName,
                lastName,
                LabTechnicianRole);

            // Check whether technician profile exists
            var existingProfile = await context
                .Set<TechnicianProfile>()
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingProfile != null)
            {
                Console.WriteLine(
                    $"Technician profile already exists: {email}");
                return;
            }

            // Create technician profile
            var profile = new TechnicianProfile
            {
                UserId = user.Id,
                IdNumber = idNumber,
                EmployeeNumber = employeeNumber
            };

            context.Set<TechnicianProfile>().Add(profile);

            await context.SaveChangesAsync();

            Console.WriteLine(
                $"Successfully seeded technician: {firstName} {lastName}");
        }

    }
}