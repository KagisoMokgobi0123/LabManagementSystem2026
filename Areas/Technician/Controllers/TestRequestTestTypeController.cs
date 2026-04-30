using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
        [HttpPost]
        public async Task<IActionResult> StartTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Already in progress check
            if (test.Status == "In Progress")
            {
                TempData["Error"] = "Test is already in progress.";
                return RedirectToAction("AvailableTests");
            }

            // Already completed check
            if (test.Status == "Completed")
            {
                TempData["Error"] = "This test has already been completed.";
                return RedirectToAction("AvailableTests");
            }

            // CHECK + DEDUCT CONSUMABLES
            var consumables = await _dbcontext.TestTypeConsumables
                .Where(c => c.TestTypeId == test.TestTypeId)
                .ToListAsync();

            foreach (var item in consumables)
            {
                var stock = await _dbcontext.Consumables
                    .FirstOrDefaultAsync(s => s.ConsumableId == item.ConsumableId);

                if (stock == null || stock.QuantityOnHand < item.QuantityRequired)
                {
                    TempData["Error"] = "Insufficient stock for this test.";
                    return RedirectToAction("AvailableTests");
                }

                stock.QuantityOnHand -= item.QuantityRequired;
            }

            // Assign technician
            test.TechnicianId = technicianId;
            test.Status = "In Progress";

            // Start time
            test.StartDateTime = DateTime.Now;

            // Expected completion time (TURNAROUND TIME)
            test.CompletionDateTime = DateTime.Now.AddHours(test.TestType.TurnAroundTimeHours);

            // Update request status
            if (test.TestRequest.Status != "In Progress")
            {
                test.TestRequest.Status = "In Progress";
            }

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test started successfully.";

            return RedirectToAction("MyTests");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTest(int id)
        {
            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            // Check if already completed
            if (test.Status == "Completed")
            {
                TempData["Error"] = "This test has already been completed.";
                return RedirectToAction("MyTests");
            }

            // ensure it was started first
            if (test.Status != "In Progress")
            {
                TempData["Error"] = "Test must be started before it can be completed.";
                return RedirectToAction("MyTests");
            }

            // Complete test
            test.Status = "Completed";
            test.CompletionDateTime = DateTime.Now;

            // Check if ALL tests are completed
            var allCompleted = _dbcontext.TestRequestTestTypes
                .Where(t => t.TestRequestId == test.TestRequestId)
                .All(t => t.Status == "Completed");

            if (allCompleted)
            {
                test.TestRequest.Status = "Completed";
            }

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test completed successfully.";

            return RedirectToAction("MyTests");
        }


        public async Task<IActionResult> MyTests()
        {
            var technicianId = _userManager.GetUserId(User);

            var tests = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                .Where(t => t.TechnicianId == technicianId)
                .ToListAsync();

            return View(tests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureTestResult(int id, string resultValue, string notes)
        {
            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestRequest)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != "In Progress")
            {
                TempData["Error"] = "Test must be in progress before capturing results.";
                return RedirectToAction("MyTests");
            }

            // 🧪 Save basic data first
            test.ResultValue = resultValue;
            test.Notes = notes;

            // ABNORMAL CHECK 
            decimal value = 0;
            bool parsed = decimal.TryParse(resultValue, out value);

            bool isAbnormal = false;

            if (parsed)
            {
                if (value < test.TestType.NormalMin || value > test.TestType.NormalMax)
                {
                    isAbnormal = true;
                }
            }

            if (isAbnormal)
            {
                test.Notes += " (ABNORMAL RESULT)";
            }

            // Mark test completed
            test.Status = "Completed";
            test.CompletionDateTime = DateTime.Now;

            // Update request status
            var allCompleted = _dbcontext.TestRequestTestTypes
                .Where(t => t.TestRequestId == test.TestRequestId)
                .All(t => t.Status == "Completed");

            if (allCompleted)
            {
                test.TestRequest.Status = "Completed";
            }
            else
            {
                test.TestRequest.Status = "In Progress";
            }

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test result captured successfully.";

            return RedirectToAction("MyTests");
        }
    }
}
