using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "LabManager")]
    public class CalendarController : Controller
    {
        private readonly ILabManagerService _service;

        public CalendarController(ILabManagerService service)
        {
            _service = service;
        }

        // GET: /Manager/Calendar  (calender.html)
        public async Task<IActionResult> Index()
        {
            var events = await _service.GetCalendarEventsAsync();
            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(CalendarEvent calendarEvent)
        {
            await _service.SaveCalendarEventAsync(calendarEvent);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteCalendarEventAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
