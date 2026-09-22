using System.Threading.Tasks;
using NMB_HLabSys.Models.Account.Enums;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
     [Authorize(Roles = "LabManager")]
    public class UsersController : Controller
    {
        private readonly IUserManagementService _userService;
        private readonly ILabManagerService _labManagerService;


        public UsersController(
            IUserManagementService userService,
            ILabManagerService labManagerService)
        {
            _userService = userService;
            _labManagerService = labManagerService;
        }



        // GET: /Manager/Users
        public async Task<IActionResult> Index()
        {
            ViewBag.Doctors =
                await _userService.GetUsersByRoleAsync(
                    UserRole.Doctor.ToString());


            ViewBag.Technicians =
                await _userService.GetUsersByRoleAsync(
                    UserRole.LabTechnician.ToString());


            ViewBag.TestTypes =
                await _labManagerService.GetTestTypesAsync();


            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(
            string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction(nameof(Index));
            }


            var result =
                await _userService.DeactivateUserAsync(id);


            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(
                        ", ",
                        result.Errors);
            }
            else
            {
                TempData["Success"] =
                    "User deactivated successfully.";
            }


            return RedirectToAction(nameof(Index));
        }
    }
}