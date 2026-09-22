using System.Collections.Generic;
using System.Threading.Tasks;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<IReadOnlyList<ApplicationUser>> GetUsersByRoleAsync(string roleName);

        Task<IReadOnlyList<ApplicationUser>> GetRecentUsersAsync(
            int count = 5);

        Task<ApplicationUser?> GetByIdAsync(
            string id);

        Task<(bool Succeeded, string[] Errors, ApplicationUser? User)>
            CreateUserAsync(
                ApplicationUser user,
                string role,
                string temporaryPassword);


        Task<(bool Succeeded, string[] Errors)>
            UpdateUserAsync(
                ApplicationUser user);


        Task<(bool Succeeded, string[] Errors)>
            DeactivateUserAsync(
                string id);


        Task<(bool Succeeded, string[] Errors)>
            ResetPasswordAsync(
                string id);


        Task AssignTestTypesAsync(
            string technicianId,
            IEnumerable<int> testTypeIds);
    }
}