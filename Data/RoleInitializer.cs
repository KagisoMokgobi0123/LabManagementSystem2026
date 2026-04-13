using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Data
{

    public static class RoleInitializer
        {
            public static async Task InitializeAsync(UserManager<RegisterUser> userManager, RoleManager<IdentityRole> roleManager)
            {   

                //Roles
                string[] roles = { "Doctor", "Technician", "Manager", "Patient", "Administrator" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

              // Helper method-like logic for seeding users
    async Task SeedUser(string email, string name, string role, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new RegisterUser
            {
                UserName = email,
                Email = email,
                FullName = name,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    // 2. Seed Technicians (Keeping your original one)
    await SeedUser("technician01@nmbhl.com", "Rudzani Khauli", "Technician", "Technician@123!");

    // 3. Seed Doctors
    await SeedUser("doctor01@nmbhl.com", "Dr. Sarah Pillay", "Doctor", "Doctor@123!");
    await SeedUser("doctor02@nmbhl.com", "Dr. Mark van Wyk", "Doctor", "Doctor@123!");

    // 4. Seed Patients
    await SeedUser("patient01@gmail.com", "Sipho Dlamini", "Patient", "Patient@123!");
    await SeedUser("patient02@gmail.com", "Lerato Molefe", "Patient", "Patient@123!");

    // 5. Seed Managers
    await SeedUser("manager01@nmbhl.com", "Adriaan Jooste", "Manager", "Manager@123!");
    await SeedUser("manager02@nmbhl.com", "Nomsa Cele", "Manager", "Manager@123!");
            



        }


    }


}
