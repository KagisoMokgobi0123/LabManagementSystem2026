using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TestRequestTestTypeController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;

        public TestRequestTestTypeController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> AvailableTests()
        {  

            // Get signed in user
            var technicianId = _userManager.GetUserId(User);

            // Verify if signed user matches one in the TestReuqestTestType model
            var allowedTestTypes = _dbcontext.TechnicianTestTypes
                .Where(t => t.TechnicianId == technicianId)
                .Select(t => t.TestTypeId);

            // Get test request and test type from TestRequestTestTypes table
            var tests = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestRequest)
                .Include(t => t.TestType)
                .Where(t =>
                    t.Status == "Pending" &&
                    t.TestRequest.Status == "SamplesReceived" &&
                    allowedTestTypes.Contains(t.TestTypeId))
                .ToListAsync();

            return View(tests);
        }
    }
}
