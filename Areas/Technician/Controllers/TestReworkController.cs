using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TestReworkController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;

        public TestReworkController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        public static class Status
        {
            public const string Pending = "Pending";
            public const string InProgress = "In Progress";
            public const string Completed = "Completed";
            public const string SamplesReceived = "Samples Received";
            public const string Verified = "Verified";
            public const string ReturnedForReview = "To be Reviewed";

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReworkTest(int id, string newResult, string notes)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTestTypes
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != Status.ReturnedForReview)
            {
                TempData["Error"] = "Only tests sent for review can be edited.";
                return RedirectToAction("MyTests");
            }

            // Save history BEFORE overwriting
            var history = new TestRequestTestTypeHistoryModel
            {
                TestRequestTestTypeId = id,
                ResultValue = newResult,
                Notes = notes,
                ActionType = "Reworked",
                ActionByTechnicianId = technicianId,
                ActionDateTime = DateTime.UtcNow
            };

            _dbcontext.TestRequestTestTypeHistories.Add(history);

            // update current test
            test.ResultValue = newResult;
            test.Notes = notes;

            test.Status = Status.Completed; // ready for re-verification

            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("MyTests");
        }
    }
}
