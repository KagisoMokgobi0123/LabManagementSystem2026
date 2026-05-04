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
        public static class Status
        {
            public const string Pending = "Pending";
            public const string InProgress = "In Progress";
            public const string Completed = "Completed";
            public const string SamplesReceived = "Samples Received";
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
                    t.Status == Status.Pending &&
                    t.TestRequest.Status == Status.SamplesReceived &&
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
                .Include(t => t.TestType)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status == Status.InProgress)
            {
                TempData["Error"] = "Test is already in progress.";
                return RedirectToAction("AvailableTests");
            }

            if (test.Status == Status.Completed)
            {
                TempData["Error"] = "This test has already been completed.";
                return RedirectToAction("AvailableTests");
            }

            var allowedTestTypes = _dbcontext.TechnicianTestTypes
                .Where(t => t.TechnicianId == technicianId)
                .Select(t => t.TestTypeId);

            if (!allowedTestTypes.Contains(test.TestTypeId))
                return Forbid();

            var consumables = await _dbcontext.TestTypeConsumables
                .Where(c => c.TestTypeId == test.TestTypeId)
                .ToListAsync();

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
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

                test.TechnicianId = technicianId;
                test.Status = Status.InProgress;
                test.StartDateTime = DateTime.UtcNow;
                test.DueDate = DateTime.UtcNow.AddHours(test.TestType.TurnAroundTimeHours);

                if (test.TestRequest.Status != Status.InProgress)
                {
                    test.TestRequest.Status = Status.InProgress;
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            TempData["Success"] = "Test started successfully.";
            return RedirectToAction("MyTests");
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureTestResult(int id, string resultValue, string notes)
        {
            var test = await _dbcontext.TestRequestTestTypes
                .Include(t => t.TestRequest)
                .Include(t => t.TestType)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
                return NotFound();

            if (test.Status != Status.InProgress)
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
            test.Status = Status.Completed;
            test.CompletionDateTime = DateTime.UtcNow;

            // Update request status
            var allCompleted = _dbcontext.TestRequestTestTypes
                .Where(t => t.TestRequestId == test.TestRequestId)
                .All(t => t.Status == Status.Completed);

            if (allCompleted)
            {
                test.TestRequest.Status = Status.Completed;
            }
            else
            {
                test.TestRequest.Status = Status.InProgress;
            }

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Test result captured successfully.";

            return RedirectToAction("MyTests");
        }
    }
}
