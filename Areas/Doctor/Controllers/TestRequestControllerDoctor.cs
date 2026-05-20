using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;

using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class TestRequestControllerDoctor : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;
        private readonly ILogger<TestRequestControllerDoctor> _logger;

        public TestRequestControllerDoctor(ApplicationDbContext context, UserManager<RegisterUser> userManager, ILogger<TestRequestControllerDoctor> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<int> GetCurrentDoctorIdAsync()
        {
            var userId = _userManager.GetUserId(User);

            _logger.LogInformation($"GetCurrentDoctorIdAsync - UserId: {userId ?? "NULL"}");
            _logger.LogInformation($"User IsAuthenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogInformation($"User Name: {User.Identity?.Name ?? "NULL"}");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId is null or empty");
                return 0;
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserID == userId);

            if (doctor == null)
                _logger.LogWarning($"No doctor found for UserID: {userId}");
            else
                _logger.LogInformation($"Found DoctorID: {doctor.DoctorId} for UserID: {userId}");

            return doctor?.DoctorId ?? 0;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Dashboard()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
            {
                TempData["Error"] = "Doctor profile not found. Please contact administrator.";
                return RedirectToAction("LoginUser", "Account", new { area = "" });
            }

            var requests = await _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                .Where(r => r.DoctorId == doctorId.ToString())
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        public IActionResult Test()
        {
            return Content("TestRequest controller is working! Time: " + DateTime.Now.ToString("HH:mm:ss"));
        }

        // GET: Create
        public async Task<IActionResult> Create(int? patientId)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
            {
                TempData["Error"] = "Please login as a doctor to create test requests.";
                return RedirectToAction("LoginUser", "Account", new { area = "" });
            }

            var patients = await _context.Patients
                .OrderBy(p => p.Name)
                .ToListAsync();

            var testTypes = await _context.Set<TestTypeModel>()
                .Include(t => t.SampleType)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var sampleTypes = await _context.Set<SampleTypeModel>()
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Patients = patients;
            ViewBag.TestTypes = testTypes;
            ViewBag.SampleTypes = sampleTypes;

            if (patientId.HasValue)
                ViewBag.Patient = await _context.Patients.FindAsync(patientId.Value);

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestRequestsModel request, List<int> testTypeIds, List<string> barcodes, List<int> sampleTypeIds)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
            {
                TempData["Error"] = "Doctor not authenticated. Please login again.";
                return RedirectToAction("LoginUser", "Account", new { area = "" });
            }

            if (testTypeIds == null || testTypeIds.Count == 0)
                ModelState.AddModelError("", "At least one test type must be selected.");

            if (barcodes == null || barcodes.Count == 0)
                ModelState.AddModelError("", "At least one sample barcode must be provided.");

            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _context.Patients.OrderBy(p => p.Name).ToListAsync();
                ViewBag.TestTypes = await _context.Set<TestTypeModel>()
                    .Include(t => t.SampleType)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                ViewBag.SampleTypes = await _context.Set<SampleTypeModel>()
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                ViewBag.Patient = await _context.Patients.FindAsync(request.PatientId);
                return View(request);
            }

            request.RequestNumber = "REQ-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            request.DoctorId = doctorId.ToString();
            request.RequestDate = DateTime.Now;
            request.CreatedAt = DateTime.Now;
            request.Status = TestRequestStatus.Submitted.ToString();

            _context.TestRequests.Add(request);
            await _context.SaveChangesAsync();

            foreach (var testTypeId in testTypeIds)
            {
                _context.TestRequestItems.Add(new TestRequestItem
                {
                    TestRequestID = request.TestRequestId,
                    TestTypeID = testTypeId,
                    Status = TestItemStatus.Submitted
                });
            }

            for (int i = 0; i < barcodes.Count; i++)
            {
                _context.Samples.Add(new SampleModel
                {
                    TestRequestId = request.TestRequestId,
                    Barcode = barcodes[i],
                    SampleTypeId = (sampleTypeIds != null && sampleTypeIds.Count > i) ? sampleTypeIds[i] : 0,
                    CollectedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Test request submitted successfully.";
            return RedirectToAction("Details", new { id = request.TestRequestId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
                return RedirectToAction("LoginUser", "Account", new { area = "" });

            var request = await _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                    .ThenInclude(i => i.TestType)
                        .ThenInclude(t => t.SampleType)
                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)
                .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId.ToString());

            if (request == null)
                return NotFound();

            return View(request);
        }

        public async Task<IActionResult> TrackStatus(string statusFilter)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
                return RedirectToAction("LoginUser", "Account", new { area = "" });

            var requests = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                .Where(r => r.DoctorId == doctorId.ToString())
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
                requests = requests.Where(r => r.Status == statusFilter);

            ViewBag.StatusFilter = statusFilter;
            return View(await requests.OrderByDescending(r => r.RequestDate).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string cancelReason)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
                return RedirectToAction("LoginUser", "Account", new { area = "" });

            var request = await _context.TestRequests
                .FirstOrDefaultAsync(r => r.TestRequestId == id && r.DoctorId == doctorId.ToString());

            if (request == null)
                return NotFound();

            if (request.Status != TestRequestStatus.Submitted.ToString() &&
                request.Status != TestRequestStatus.SamplesReceived.ToString())
            {
                TempData["Error"] = "This request can no longer be cancelled.";
                return RedirectToAction("Details", new { id });
            }

            if (string.IsNullOrEmpty(cancelReason))
            {
                TempData["Error"] = "A reason must be provided to cancel.";
                return RedirectToAction("Details", new { id });
            }

            request.Status = TestRequestStatus.Cancelled.ToString();
            request.CancelReason = cancelReason;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Test request cancelled.";
            return RedirectToAction("Dashboard");
        }
    }
}