using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Data
{

    public static class RoleInitializer
        {
            public static async Task InitializeAsync(UserManager<RegisterUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                string[] roles = { "Doctor", "Technician", "Manager", "Patient", "Administrator" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Default technician user
                var technicianEmail = "technician01@nmbhl.com";
                var technicianUser = await userManager.FindByEmailAsync(technicianEmail);

                if (technicianUser == null)
                {
                    technicianUser = new RegisterUser
                    {
                        UserName = technicianEmail,
                        Email = technicianEmail,
                        FullName = "Rudzani Khauli"
                    };
                    var result = await userManager.CreateAsync(technicianUser, "Technician@123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(technicianUser, "Technician");
                    }
                }
            }
        }


}
