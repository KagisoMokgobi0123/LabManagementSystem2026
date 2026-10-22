
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class NotificationsController : Controller
    {
        private readonly TechnicianNotificationService
            _notificationService;

        private readonly UserManager<ApplicationUser>
            _userManager;

        public NotificationsController(
            TechnicianNotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

#if DEBUG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestNotification()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            await _notificationService.CreateNotificationAsync(
                technicianId: user.Id,
                title: "Laboratory Notification Test",
                message: "Your notification system is working! " +
                         "This message was saved successfully in SQL Server.",
                actionUrl: "/Technician/Notifications"
            );

            TempData["SuccessMessage"] =
                "Test notification created successfully.";

            return RedirectToAction(nameof(Index));
        }
#endif

        // GET: /Technician/Notifications
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var notifications =
                await _notificationService
                    .GetNotificationsAsync(user.Id);

            return View(notifications);
        }

        // GET: /Technician/Notifications/UnreadCount
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var count = await _notificationService
                .GetUnreadCountAsync(user.Id);

            return Json(new { count });
        }

        // POST: /Technician/Notifications/MarkAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var updated = await _notificationService
                .MarkAsReadAsync(id, user.Id);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
