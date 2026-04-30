using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public async Task<IActionResult> ReworkTest(int id, string newResult, string notes)
        {
            var test = await _dbcontext.TestRequestTestTypes
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != "To Be Reviewed")
            {
                TempData["Error"] = "Only tests sent for review can be edited.";
                return RedirectToAction("MyTests");
            }

            // update result
            test.ResultValue = newResult;
            test.Notes = notes;

            // back to completed for re-verification
            test.Status = "Completed";

            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("MyTests");
        }
    }
}
