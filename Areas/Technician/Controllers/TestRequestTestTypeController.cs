using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TestRequestTestTypeController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        private readonly ILogger<TestRequestTestTypeController> _logger;

        public TestRequestTestTypeController(
            ApplicationDbContext context,
            UserManager<RegisterUser> userManager,
            ILogger<TestRequestTestTypeController> logger)
        {
            _dbcontext = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Status Constants - Moved outside controller or use enum
        public static class TestStatusConstants
        {
            public const string Pending = "Pending";
            public const string Submitted = "Submitted";
            public const string InProgress = "InProgress";
            public const string Completed = "Completed";
            public const string SamplesReceived = "Samples Received";
            public const string Verified = "Verified";
            public const string ToBeReviewed = "ToBeReviewed";
        }

        [HttpGet]
        public async Task<IActionResult> MyTests()
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return RedirectToAction("Login", "Account");
            }

            var tests = await _dbcontext.TestRequestTests
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Where(t => t.TechnicianId == technicianId)
                .OrderByDescending(t => t.StartDateTime)
                .ToListAsync();

            // Add counts for dashboard stats
            ViewBag.InProgressCount = tests.Count(t => t.Status == TestStatusConstants.InProgress);
            ViewBag.CompletedCount = tests.Count(t => t.Status == TestStatusConstants.Completed);
            ViewBag.VerifiedCount = tests.Count(t => t.Status == TestStatusConstants.Verified);

            return View(tests);
        }

        [HttpGet]
        public async Task<IActionResult> AvailableTests()
        {
            // Get signed in user
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify if signed user matches one in the TechnicianTestType model
            var allowedTestTypes = await _dbcontext.TechnicianTestTypes
                .Where(t => t.TechnicianId == technicianId && t.Status == "Approved")
                .Select(t => t.TestTypeId)
                .ToListAsync();

            if (!allowedTestTypes.Any())
            {
                TempData["Warning"] = "You don't have any test types assigned yet. Please contact your manager.";
                return View(new List<TestRequestTestTypeModel>());
            }

            // Get test request and test type from TestRequestTests table
            var tests = await _dbcontext.TestRequestTests
                .Include(t => t.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Include(t => t.TestType)
                .Where(t =>
                    t.Status == TestStatusConstants.Submitted &&
                    t.TestRequest.Status == TestStatusConstants.SamplesReceived &&
                    allowedTestTypes.Contains(t.TestTypeId) &&
                    t.TechnicianId == null)
                .OrderBy(t => t.TestRequest.Urgency == Urgency.Stat ? 0 :
                              t.TestRequest.Urgency == Urgency.Urgent ? 1 : 2)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            ViewBag.AllowedTestTypeCount = allowedTestTypes.Count;

            return View(tests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTest(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return Unauthorized();
            }

            var test = await _dbcontext.TestRequestTests
                .Include(t => t.TestRequest)
                .Include(t => t.TestType)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("AvailableTests");
            }

            // Check current status
            if (test.Status == TestStatusConstants.InProgress)
            {
                TempData["Error"] = "Test is already in progress.";
                return RedirectToAction("AvailableTests");
            }

            if (test.Status == TestStatusConstants.Completed)
            {
                TempData["Error"] = "This test has already been completed.";
                return RedirectToAction("AvailableTests");
            }

            if (test.Status == TestStatusConstants.Verified)
            {
                TempData["Error"] = "This test has already been verified.";
                return RedirectToAction("AvailableTests");
            }

            // Check if technician is authorized to perform this test
            var allowedTestTypes = await _dbcontext.TechnicianTestTypes
                .Where(t => t.TechnicianId == technicianId && t.Status == "Approved")
                .Select(t => t.TestTypeId)
                .ToListAsync();

            if (!allowedTestTypes.Contains(test.TestTypeId))
            {
                TempData["Error"] = "You are not authorized to perform this test.";
                return RedirectToAction("AvailableTests");
            }

            // Check consumables availability
            var consumables = await _dbcontext.TestTypeConsumables
                .Include(tc => tc.Consumable)
                .Where(c => c.TestTypeId == test.TestTypeId)
                .ToListAsync();

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                // Deduct consumables from stock
                foreach (var item in consumables)
                {
                    var stock = await _dbcontext.Consumables
                        .FirstOrDefaultAsync(s => s.ConsumableId == item.ConsumableId);

                    if (stock == null)
                    {
                        TempData["Error"] = $"Consumable not found: {item.ConsumableId}";
                        await transaction.RollbackAsync();
                        return RedirectToAction("AvailableTests");
                    }

                    if (stock.QuantityOnHand < item.QuantityRequired)
                    {
                        TempData["Error"] = $"Insufficient stock for {stock.Name}. Available: {stock.QuantityOnHand}, Required: {item.QuantityRequired}";
                        await transaction.RollbackAsync();
                        return RedirectToAction("AvailableTests");
                    }

                    stock.QuantityOnHand -= item.QuantityRequired;

                    // Log stock transaction
                    _dbcontext.StockTransactions.Add(new StockTransactionModel
                    {
                        ConsumableId = stock.ConsumableId,
                        Type = TransactionType.Decrease,
                        Quantity = item.QuantityRequired,
                        PreviousQuantity = stock.QuantityOnHand + item.QuantityRequired,
                        NewQuantity = stock.QuantityOnHand,
                        TransactionDate = DateTime.Now,
                        Reference = $"Test {test.TestType.Name} (ID: {test.TestRequestTestTypeId})",
                        PerformedBy = technicianId
                    });
                }

                // Update test status
                test.TechnicianId = technicianId;
                test.Status = TestStatusConstants.InProgress;
                test.StartDateTime = DateTime.UtcNow;
                test.DueDate = DateTime.UtcNow.AddHours(test.TestType.TurnAroundTimeHours);

                // Update test request status if it's the first test being processed
                if (test.TestRequest.Status != TestStatusConstants.InProgress)
                {
                    test.TestRequest.Status = TestStatusConstants.InProgress;
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Test '{test.TestType.Name}' started successfully. Due by: {test.DueDate:dd MMM yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error starting test {TestId}", id);
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("MyTests");
        }

        [HttpGet]
        public async Task<IActionResult> CaptureResult(int id)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return RedirectToAction("Login", "Account");
            }

            var test = await _dbcontext.TestRequestTests
                .Include(t => t.TestType)
                .Include(t => t.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("MyTests");
            }

            if (test.TechnicianId != technicianId)
            {
                TempData["Error"] = "You are not authorized to capture results for this test.";
                return RedirectToAction("MyTests");
            }

            if (test.Status != TestStatusConstants.InProgress)
            {
                TempData["Error"] = "This test is not in progress. Current status: " + test.Status;
                return RedirectToAction("MyTests");
            }

            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureTestResult(int id, string resultValue, string notes)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return Unauthorized();
            }

            var test = await _dbcontext.TestRequestTests
                .Include(t => t.TestRequest)
                .Include(t => t.TestType)
                .FirstOrDefaultAsync(t => t.TestRequestTestTypeId == id);

            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("MyTests");
            }

            if (test.TechnicianId != technicianId)
            {
                TempData["Error"] = "You are not authorized to capture results for this test.";
                return RedirectToAction("MyTests");
            }

            if (test.Status != TestStatusConstants.InProgress)
            {
                TempData["Error"] = $"Test must be in progress before capturing results. Current status: {test.Status}";
                return RedirectToAction("MyTests");
            }

            if (string.IsNullOrWhiteSpace(resultValue))
            {
                TempData["Error"] = "Result value is required.";
                return RedirectToAction("CaptureResult", new { id });
            }

            try
            {
                // Save result
                test.ResultValue = resultValue;
                test.Notes = notes;

                // Check for abnormal results
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
                    test.Notes = (string.IsNullOrEmpty(notes) ? "" : notes + " ") + "⚠️ ABNORMAL RESULT - Outside normal range";
                    test.IsAbnormal = true;

                    // Create notification for abnormal result
                    //await CreateAbnormalResultNotification(test);
                }

                // Mark test as completed
                test.Status = TestStatusConstants.Completed;
                test.CompletionDateTime = DateTime.UtcNow;

                // Update test request status
                var allTestsInRequest = await _dbcontext.TestRequestTests
                    .Where(t => t.TestRequestId == test.TestRequestId)
                    .ToListAsync();

                var allCompleted = allTestsInRequest.All(t => t.Status == TestStatusConstants.Completed);
                var anyInProgress = allTestsInRequest.Any(t => t.Status == TestStatusConstants.InProgress);

                if (allCompleted)
                {
                    test.TestRequest.Status = TestStatusConstants.Completed;
                    //test.TestRequest.CompletedDate = DateTime.UtcNow;

                    // Notify doctor that all tests are completed
                    await NotifyDoctorTestsCompleted(test.TestRequest);
                }
                else if (anyInProgress)
                {
                    test.TestRequest.Status = TestStatusConstants.InProgress;
                }

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] = $"Test result for '{test.TestType.Name}' captured successfully. {(isAbnormal ? "Abnormal result flagged." : "")}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing test result for test {TestId}", id);
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("CaptureResult", new { id });
            }

            return RedirectToAction("MyTests");
        }

        //private async Task CreateAbnormalResultNotification(TestRequestTestTypeModel test)
        //{
        //    var notification = new NotificationModel
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        Title = "Abnormal Test Result",
        //        Message = $"Test '{test.TestType.Name}' for patient {test.TestRequest.Patient?.FullName} shows abnormal result: {test.ResultValue} (Normal range: {test.TestType.NormalMin}-{test.TestType.NormalMax})",
        //        Type = "AbnormalResult",
        //        RecipientId = test.TestRequest.DoctorId,
        //        RelatedEntityId = test.TestRequestId,
        //        CreatedAt = DateTime.UtcNow,
        //        IsRead = false
        //    };

        //    _dbcontext.Notifications.Add(notification);
        //}

        private async Task NotifyDoctorTestsCompleted(TestRequestsModel testRequest)
        {
            // This would send email notification to doctor
            var doctor = await _userManager.FindByIdAsync(testRequest.DoctorId);
            if (doctor != null && !string.IsNullOrEmpty(doctor.Email))
            {
                // Implement email sending logic
                _logger.LogInformation($"Notification sent to doctor {doctor.Email} for completed tests");
            }
        }
    }
}
