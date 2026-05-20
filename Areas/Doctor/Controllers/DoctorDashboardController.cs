using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Areas.Doctor.Models;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class DoctorDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;

        public DoctorDashboardController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ── Helper ────────────────────────────────────────────
        private int GetDoctorId()
        {
            // Get UserId as string (matches what you stored in AccountController)
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return 0;
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserID.ToString() == userIdString);
            return doctor?.DoctorId ?? 0;
        }

        // ── 1. Dashboard ──────────────────────────────────────
        public IActionResult Dashboard()
        {
            var doctorId = GetDoctorId();

            // Set ViewBag properties for the dashboard
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.PendingTests = _context.TestRequests
                .Count(r => r.DoctorId == doctorId.ToString() && r.Status == TestRequestStatus.Submitted.ToString());
            ViewBag.CompletedTests = _context.TestRequests
                .Count(r => r.DoctorId == doctorId.ToString() && r.Status == TestRequestStatus.Completed.ToString());
            ViewBag.ActiveAlerts = _context.DoctorAlerts
                .Count(a => a.DoctorId == doctorId && a.CreatedAt >= DateTime.Now.AddDays(-5));
            ViewBag.RecentRequests = _context.TestRequests
                .Include(r => r.Patient)
                .Where(r => r.DoctorId == doctorId.ToString())
                .OrderByDescending(r => r.RequestDate)
                .Take(10)
                .ToList();

            return View();
        }

        // ── 2. Manage Patients ────────────────────────────────
        public IActionResult Patients()
        {
            var patients = _context.Patients.ToList();
            return View(patients);
        }

        public IActionResult PatientDetails(int id)
        {
            var patient = _context.Patients
                .Include(p => p.PatientConditions).ThenInclude(pc => pc.MedicalCondition)
                .Include(p => p.PatientAllergies).ThenInclude(pa => pa.Allergy)
                .Include(p => p.PatientMedications).ThenInclude(pm => pm.Medication)
                .FirstOrDefault(p => p.PatientID == id);

            if (patient == null) return NotFound();
            return View(patient);
        }

        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(PatientModel patient)
        {
            if (!ModelState.IsValid) return View(patient);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Patients));
        }

        public IActionResult EditPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(PatientModel patient)
        {
            if (!ModelState.IsValid) return View(patient);

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Patients));
        }

        // ── 3. Test Requests ──────────────────────────────────
        public IActionResult TestRequests()
        {
            var doctorId = GetDoctorId();
            var requests = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                .Where(r => r.DoctorId == doctorId.ToString())
                .OrderByDescending(r => r.RequestDate)
                .ToList();
            return View(requests);
        }

        public IActionResult CreateTestRequest()
        {
            ViewBag.Patients = _context.Patients.ToList();
            ViewBag.TestTypes = _context.TestTypes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestRequest(TestRequestsModel request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = _context.Patients.ToList();
                return View(request);
            }

            request.DoctorId = GetDoctorId().ToString();
            request.RequestDate = DateTime.Now;
            request.CreatedAt = DateTime.Now;
            request.Status = TestRequestStatus.Submitted.ToString();

            _context.TestRequests.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestRequests));
        }

        public IActionResult TestRequestDetails(int id)
        {
            var request = _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestRequestItems)
                .Include(r => r.Samples)
                .FirstOrDefault(r => r.TestRequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTestRequest(int id, string cancelReason)
        {
            var request = _context.TestRequests.Find(id);
            if (request == null) return NotFound();

            // Doctor can only cancel if Submitted or SamplesReceived
            if (request.Status != TestRequestStatus.Submitted.ToString() &&
                request.Status != TestRequestStatus.SamplesReceived.ToString())
            {
                TempData["Error"] = "Test request can only be cancelled if status is Submitted or Samples Received.";
                return RedirectToAction(nameof(TestRequestDetails), new { id });
            }

            request.Status = TestRequestStatus.Cancelled.ToString();
            request.CancelReason = cancelReason;

            _context.TestRequests.Update(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestRequests));
        }

        // ── 4. View Results ───────────────────────────────────
        public IActionResult Results(int requestId)
        {
            var results = _context.TestResults
                .Include(r => r.TestRequestItem)
                .Where(r => r.TestRequestItem.TestRequestID == requestId)
                .ToList();
            return View(results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReleaseResults(int requestId, string releaseNote)
        {
            var request = _context.TestRequests
                .Include(r => r.ResultRelease)
                .FirstOrDefault(r => r.TestRequestId == requestId);

            if (request == null) return NotFound();

            request.Status = TestRequestStatus.ReleasedByDoctor.ToString();

            if (request.ResultRelease == null)
            {
                request.ResultRelease = new ResultRelease
                {
                    TestRequestId = requestId,
                    Note = releaseNote,
                    ReleasedAt = DateTime.Now
                };
            }

            _context.TestRequests.Update(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestRequests));
        }

        // ── 5. Alerts ─────────────────────────────────────────
        public IActionResult Alerts(DateTime? fromDate = null)
        {
            try
            {
                var doctorId = GetDoctorId();
                if (doctorId == 0)
                {
                    ViewBag.Error = "No doctor profile found. Please contact administrator.";
                    return View(new List<TestRequestItem>());
                }

                // Get abnormal results for tests ordered by this doctor in last 5 days
                var alerts = _context.TestRequestItems
                    .Include(t => t.TestRequest)
                    //.Include(t => t.TestResults)
                    .Where(t => t.TestRequest.DoctorId == doctorId.ToString()
                     //   && t.TestResults.Any(r => r.IsAbnormal == true)
                        && t.TestRequest.RequestDate >= (fromDate ?? DateTime.Now.AddDays(-5)))
                    .ToList();

                return View(alerts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Alerts: {ex.Message}");
                ViewBag.Error = "Unable to load alerts. Please try again later.";
                return View(new List<TestRequestItem>());
            }
        }
    }
}