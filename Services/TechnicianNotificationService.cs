
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Services
{
    public class TechnicianNotificationService
    {
        private readonly ApplicationDbContext _context;

        public TechnicianNotificationService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE A NEW NOTIFICATION
        public async Task CreateNotificationAsync(
            string technicianId,
            string title,
            string message,
            string? actionUrl = null)
        {
            var notification = new TechnicianNotificationModel
            {
                TechnicianId = technicianId,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.TechnicianNotifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        // GET NOTIFICATIONS FOR ONE TECHNICIAN
        public async Task<List<TechnicianNotificationModel>>
            GetNotificationsAsync(string technicianId)
        {
            return await _context.TechnicianNotifications
                .AsNoTracking()
                .Where(n => n.TechnicianId == technicianId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // COUNT UNREAD NOTIFICATIONS
        public async Task<int> GetUnreadCountAsync(
            string technicianId)
        {
            return await _context.TechnicianNotifications
                .CountAsync(n =>
                    n.TechnicianId == technicianId &&
                    !n.IsRead);
        }

        // MARK NOTIFICATION AS READ
        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            string technicianId)
        {
            var notification =
                await _context.TechnicianNotifications
                    .FirstOrDefaultAsync(n =>
                        n.NotificationId == notificationId &&
                        n.TechnicianId == technicianId);

            if (notification == null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
