using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Data
{
    public static class RoleInitializer
    {

        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {

            string[] roles =
            {
                "Administrator",
                "Doctor",
                "LabManager",
                "LabTechnician",
                "Patient"
            };


            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

        }
    }


}
