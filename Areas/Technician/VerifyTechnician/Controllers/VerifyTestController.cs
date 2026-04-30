using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.VerifyTechnician.Controllers
{
    public class VerifyTestController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;

        public VerifyTestController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> VerifyTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestType)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // 🔴 Must be completed first
            if (test.Status != "Completed")
            {
                TempData["Error"] = "Only completed tests can be verified.";
                return RedirectToAction("VerificationQueue");
            }

            // 🔴 Cannot verify own result
            if (test.TechnicianId == technicianId)
            {
                TempData["Error"] = "You cannot verify your own test.";
                return RedirectToAction("VerificationQueue");
            }

            // 🔴 Must be assigned to this test type
            var allowed = _dbcontext.TechnicianTestTypes
                .Any(t => t.TechnicianId == technicianId &&
                          t.TestTypeId == test.TestTypeId);

            if (!allowed)
            {
                TempData["Error"] = "You are not allowed to verify this test type.";
                return RedirectToAction("VerificationQueue");
            }

            // ✅ Approve
            test.Status = "Verified";
            test.VerifiedByTechnicianId = technicianId;
            test.VerificationDateTime = DateTime.Now;

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test verified successfully.";

            return RedirectToAction("VerificationQueue");
        }
    }
}
