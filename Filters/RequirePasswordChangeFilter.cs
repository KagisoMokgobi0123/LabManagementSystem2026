
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Filters
{
    public class RequirePasswordChangeFilter
        : IAsyncAuthorizationFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequirePasswordChangeFilter(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            var principal = context.HttpContext.User;

            if (principal.Identity?.IsAuthenticated != true)
                return;

            if (!principal.IsInRole("LabTechnician"))
                return;

            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!user.MustChangePassword)
                return;

            var route = context.RouteData.Values;

            var controller = route["controller"]?.ToString();
            var action = route["action"]?.ToString();
            var area = route["area"]?.ToString();

            // Allow the mandatory password-change page.
            if (string.IsNullOrEmpty(area) &&
                string.Equals(controller, "Account",
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action, "ChangeTemporaryPassword",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Allow the technician to log out.
            if (string.IsNullOrEmpty(area) &&
                string.Equals(controller, "Account",
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action, "Logout",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            context.Result = new RedirectToActionResult(
                "ChangeTemporaryPassword",
                "Account",
                new { area = "" });
        }
    }
}
