using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Identity;

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


        // ============================================================
        // SEED
        // ============================================================

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();


            // Create roles
            await CreateRolesAsync(roleManager);


            // ========================================================
            // DOCTOR
            // ========================================================

            await CreateUserAsync(
                userManager,
                email: "doctor@nmbhlab.co.za",
                password: "Doctor@123",
                firstName: "John",
                lastName: "Doctor",
                role: DoctorRole);


            // ========================================================
            // LAB TECHNICIAN
            // ========================================================

            await CreateUserAsync(
                userManager,
                email: "technician@nmbhlab.co.za",
                password: "Tech@1234",
                firstName: "Thabo",
                lastName: "Technician",
                role: LabTechnicianRole);


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
                PatientRole
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
                    var errors = string.Join(
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
        // CREATE USER
        // ============================================================

        private static async Task CreateUserAsync(
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
                    var errors = string.Join(
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
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            e => e.Description));


                    throw new InvalidOperationException(
                        $"Could not assign role '{role}' " +
                        $"to '{email}'. Errors: {errors}");
                }
            }
        }
    }
}