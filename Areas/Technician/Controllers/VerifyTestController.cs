using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class VerifyTestController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        private readonly IHubContext<LabHub> _hubContext;

        public VerifyTestController(
       ApplicationDbContext dbcontext,
       UserManager<RegisterUser> userManager,
       IHubContext<LabHub> hubContext)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public static class Status
        {
            public const string Pending = "Pending";
            public const string InProgress = "In Progress";
            public const string Completed = "Completed";
            public const string SamplesReceived = "Samples Received";
            public const string Verified = "Verified";
            public const string Reviewed = "Reviewed";

        }
        public async Task<IActionResult> VerificationQueue()
        {
            var technicianId = _userManager.GetUserId(User);

            var tests = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .Where(t =>
                    t.Status == Status.Completed &&          // must be completed
                    t.Status != Status.Verified &&          // not yet verified
                    t.TechnicianId != technicianId)         // cannot verify own
                .ToListAsync();

            return View(tests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Must be completed first
            if (test.Status != Status.Completed)
            {
                TempData["Error"] = "Only completed tests can be verified.";
                return RedirectToAction("VerificationQueue");
            }

            // Cannot verify own result
            if (test.TechnicianId == technicianId)
            {
                TempData["Error"] = "You cannot verify your own test.";
                return RedirectToAction("VerificationQueue");
            }

            // Must be assigned to this test type
            var allowed = _dbcontext.TechnicianTestTypes
                .Any(t => t.TechnicianId == technicianId &&
                          t.TestTypeId == test.TestTypeId);

            if (!allowed)
            {
                TempData["Error"] = "You are not allowed to verify this test type.";
                return RedirectToAction("VerificationQueue");
            }

            // Approve
            test.Status = Status.Verified;
            test.VerifiedByTechnicianId = technicianId;
            test.VerificationDateTime = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();
            await NotifyDashboardUpdate();
            TempData["Success"] = "Test verified successfully.";

            return RedirectToAction("VerificationQueue");
        }
        private async Task NotifyDashboardUpdate()
        {
            await _hubContext.Clients.Group("Technicians")
           .SendAsync("DashboardUpdated");
        }
    }
}
