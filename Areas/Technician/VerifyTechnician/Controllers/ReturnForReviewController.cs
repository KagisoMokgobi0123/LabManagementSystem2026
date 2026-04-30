using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.VerifyTechnician.Controllers
{
    public class ReturnForReviewController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;

        public ReturnForReviewController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> ReturnForReview(int id, string note)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTestTypes
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            test.Status = "To Be Reviewed";
            test.VerifiedByTechnicianId = technicianId;
            test.VerificationDateTime = DateTime.Now;
            test.VerificationNote = note;

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test returned for review.";

            return RedirectToAction("VerificationQueue");
        }
    }
}
