using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Laboratory;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;


        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }



        public async Task<IReadOnlyList<ApplicationUser>> GetUsersByRoleAsync(
            string roleName)
        {
            var users =
                await _userManager.GetUsersInRoleAsync(roleName);

            return users
                .OrderBy(x => x.Surname)
                .ThenBy(x => x.FullName)
                .ToList();
        }




        public async Task<IReadOnlyList<ApplicationUser>> GetRecentUsersAsync(
            int count = 5)
        {
            return await _db.Users
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync();
        }





        public async Task<ApplicationUser?> GetByIdAsync(
            string id)
        {
            return await _userManager.FindByIdAsync(id);
        }





        public async Task<(bool Succeeded, string[] Errors, ApplicationUser? User)>
            CreateUserAsync(
                ApplicationUser user,
                string role,
                string temporaryPassword)
        {

            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            user.MustChangePassword = true;


            var result =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);


            if (!result.Succeeded)
            {
                return (
                    false,
                    result.Errors
                        .Select(x => x.Description)
                        .ToArray(),
                    null
                );
            }



            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    role);



            if (!roleResult.Succeeded)
            {
                return (
                    false,
                    roleResult.Errors
                        .Select(x => x.Description)
                        .ToArray(),
                    user
                );
            }



            return (
                true,
                Array.Empty<string>(),
                user
            );
        }







        public async Task<(bool Succeeded, string[] Errors)>
            UpdateUserAsync(
                ApplicationUser user)
        {

            var result =
                await _userManager.UpdateAsync(user);


            return (
                result.Succeeded,
                result.Errors
                    .Select(x => x.Description)
                    .ToArray()
            );
        }








        public async Task<(bool Succeeded, string[] Errors)>
            DeactivateUserAsync(
                string id)
        {

            var user =
                await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                return (
                    false,
                    new[] { "User not found." }
                );
            }



            user.IsActive = false;


            var result =
                await _userManager.UpdateAsync(user);



            return (
                result.Succeeded,
                result.Errors
                    .Select(x => x.Description)
                    .ToArray()
            );
        }








        public async Task<(bool Succeeded, string[] Errors)>
            ResetPasswordAsync(
                string id)
        {

            var user =
                await _userManager.FindByIdAsync(id);



            if (user == null)
            {
                return (
                    false,
                    new[] { "User not found." }
                );
            }



            var token =
                await _userManager
                    .GeneratePasswordResetTokenAsync(user);



            var tempPassword =
                $"Temp@{Guid.NewGuid()
                .ToString("N")
                .Substring(0, 8)}";



            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    tempPassword);



            if (result.Succeeded)
            {
                user.MustChangePassword = true;

                await _userManager.UpdateAsync(user);
            }



            return (
                result.Succeeded,
                result.Errors
                    .Select(x => x.Description)
                    .ToArray()
            );
        }






        public async Task AssignTestTypesAsync(
            string technicianId,
            IEnumerable<int> testTypeIds)
        {

            var existing =
                _db.TechnicianTestTypes
                .Where(x => x.TechnicianId.ToString() == technicianId);



            _db.TechnicianTestTypes.RemoveRange(existing);



            foreach (var id in testTypeIds)
            {
                _db.TechnicianTestTypes.Add(new TechnicianTestType{TechnicianId = technicianId,TestTypeId = id});
            }



            await _db.SaveChangesAsync();
        }
    }
}