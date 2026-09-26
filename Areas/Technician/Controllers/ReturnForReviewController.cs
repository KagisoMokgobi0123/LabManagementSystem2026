using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class ReturnForReviewController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReturnForReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
        public async Task<IActionResult> ReturnForReview(int id, string note)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTests
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Must be completed before review action
            if (test.Status != "Completed")
            {
                TempData["Error"] = "Only completed tests can be returned for review.";
                return RedirectToAction("VerificationQueue");
            }

            // Set proper status
            test.Status = "ToBeReviewed";

            test.VerifiedByTechnicianId = technicianId;
            test.VerificationNote = note;
            test.VerificationDateTime = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test returned to technician for review.";

            return RedirectToAction("VerificationQueue");
        }
    }
}
