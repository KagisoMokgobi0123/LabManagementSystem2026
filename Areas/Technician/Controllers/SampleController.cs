using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Inventory.Enums;
using NMB_HLabSys.Models.Laboratory.Enums;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class SampleController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SampleController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }

        // ============================================================
        // GET: /Technician/Sample/Samples
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Samples(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? sampleTypeId,
            string? status,
            string? searchTerm)
        {
            var technicianId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Doctor)
                .Where(s => s.TechnicianId == technicianId);

            if (dateFrom.HasValue)
            {
                query = query.Where(
                    s => s.DateReceived >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var endDate = dateTo.Value.Date.AddDays(1);

                query = query.Where(
                    s => s.DateReceived < endDate);
            }

            if (sampleTypeId.HasValue &&
                sampleTypeId.Value > 0)
            {
                query = query.Where(
                    s => s.SampleTypeId == sampleTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All Status")
            {
                query = query.Where(
                    s => s.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(
                    s =>
                        s.Barcode.Contains(searchTerm) ||
                        s.TestRequest.RequestNumber.Contains(searchTerm) ||
                        (
                            s.TestRequest.Patient != null &&
                            (
                                s.TestRequest.Patient.Name.Contains(searchTerm) ||
                                s.TestRequest.Patient.Surname.Contains(searchTerm)
                            )
                        ) ||
                        (
                            s.SampleType != null &&
                            s.SampleType.Name.Contains(searchTerm)
                        ));
            }

            var samples = await query
                .OrderByDescending(s => s.DateReceived)
                .ToListAsync();

            ViewBag.SampleTypes =
                await _dbcontext.SampleTypes
                    .Where(s =>
                        s.Status == SampleTypeStatus.Active)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SampleTypeId.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync();

            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "All Status"
                },
                new SelectListItem
                {
                    Value = "Received",
                    Text = "Received"
                },
                new SelectListItem
                {
                    Value = "Pending",
                    Text = "Pending"
                },
                new SelectListItem
                {
                    Value = "Processing",
                    Text = "Processing"
                },
                new SelectListItem
                {
                    Value = "Completed",
                    Text = "Completed"
                },
                new SelectListItem
                {
                    Value = "Rejected",
                    Text = "Rejected"
                },
                new SelectListItem
                {
                    Value = "Deleted",
                    Text = "Deleted"
                }
            };

            ViewBag.DateFrom =
                dateFrom?.ToString("yyyy-MM-dd");

            ViewBag.DateTo =
                dateTo?.ToString("yyyy-MM-dd");

            ViewBag.SelectedSampleTypeId =
                sampleTypeId;

            ViewBag.SelectedStatus =
                status;

            ViewBag.SearchTerm =
                searchTerm;

            return View(samples);
        }

        // ============================================================
        // GET: /Technician/Sample/VerifySample
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> VerifySample(
            int? testRequestId = null)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var technician =
                await _userManager.FindByIdAsync(
                    technicianId);

            if (technician == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            ViewBag.SampleTypes =
                await _dbcontext.SampleTypes
                    .Where(st =>
                        st.Status == SampleTypeStatus.Active)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SampleTypeId.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync();

            var recentPendingRequests =
                await _dbcontext.TestRequests
                    .Include(tr => tr.Patient)
                    .Where(tr =>
                        tr.Status == "Pending" ||
                        tr.Status == "Submitted")
                    .OrderByDescending(
                        tr => tr.RequestDate)
                    .Take(5)
                    .Select(tr => new
                    {
                        tr.TestRequestId,
                        tr.RequestNumber,

                        PatientName =
                            tr.Patient != null
                                ? tr.Patient.Name + " " +
                                  tr.Patient.Surname
                                : "N/A",

                        tr.RequestDate,

                        Urgency =
                            tr.Urgency.ToString()
                    })
                    .ToListAsync();

            ViewBag.RecentPendingRequests =
                recentPendingRequests;

            var recentVerifications =
                await _dbcontext.Samples
                    .Include(s => s.TestRequest)
                    .Where(s =>
                        s.TechnicianId == technicianId &&
                        s.IsReceived)
                    .OrderByDescending(
                        s => s.DateReceived)
                    .Take(5)
                    .Select(s => new
                    {
                        s.SampleId,
                        s.Barcode,

                        RequestNumber =
                            s.TestRequest != null
                                ? s.TestRequest.RequestNumber
                                : "N/A",

                        s.DateReceived
                    })
                    .ToListAsync();

            ViewBag.RecentVerifications =
                recentVerifications;

            var today =
                DateTime.Now.Date;

            var samplesReceivedToday =
                await _dbcontext.Samples
                    .CountAsync(s =>
                        s.TechnicianId == technicianId &&
                        s.DateReceived.Date == today);

            var pendingRequests =
                await _dbcontext.TestRequests
                    .CountAsync(tr =>
                        tr.Status == "Pending" ||
                        tr.Status == "Submitted");

            var urgentPending =
                await _dbcontext.TestRequests
                    .CountAsync(tr =>
                        (
                            tr.Status == "Pending" ||
                            tr.Status == "Submitted"
                        ) &&
                        tr.Urgency == Urgency.Stat);

            ViewBag.Stats = new
            {
                SamplesReceivedToday =
                    samplesReceivedToday,

                PendingRequests =
                    pendingRequests,

                UrgentPending =
                    urgentPending
            };

            var model =
                new SampleModel();

            if (testRequestId.HasValue)
            {
                model.TestRequestId =
                    testRequestId.Value;

                var testRequest =
                    await _dbcontext.TestRequests
                        .Include(tr => tr.Patient)
                        .FirstOrDefaultAsync(
                            tr =>
                                tr.TestRequestId ==
                                testRequestId.Value);

                if (testRequest != null)
                {
                    ViewBag.PreSelectedRequestNumber =
                        testRequest.RequestNumber;

                    ViewBag.PreSelectedPatientName =
                        testRequest.Patient != null
                            ? testRequest.Patient.Name + " " +
                              testRequest.Patient.Surname
                            : "N/A";

                    ViewBag.PreSelectedUrgency =
                        testRequest.Urgency.ToString();

                    ViewBag.PreSelectedClinicalNotes =
                        testRequest.ClinicalNotes;
                }
            }

            return View(model);
        }

        // ============================================================
        // POST: /Technician/Sample/VerifySample
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifySample(
            SampleModel model)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return Unauthorized();
            }

            ViewBag.SampleTypes =
                await _dbcontext.SampleTypes
                    .Where(s =>
                        s.Status == SampleTypeStatus.Active)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SampleTypeId.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Please fill in all required fields correctly.";

                return View(model);
            }

            var testRequest =
                await _dbcontext.TestRequests
                    .Include(tr => tr.Patient)
                    .FirstOrDefaultAsync(
                        r =>
                            r.TestRequestId ==
                            model.TestRequestId);

            if (testRequest == null)
            {
                TempData["Error"] =
                    "Invalid request number. Please search and select a valid request.";

                return View(model);
            }

            var existingSample =
                await _dbcontext.Samples
                    .FirstOrDefaultAsync(
                        s =>
                            s.Barcode == model.Barcode &&
                            s.TestRequestId ==
                            model.TestRequestId);

            if (existingSample != null &&
                existingSample.IsReceived)
            {
                TempData["Error"] =
                    $"Sample '{model.Barcode}' has already been received on " +
                    $"{existingSample.DateReceived:dd MMM yyyy HH:mm}";

                return View(model);
            }

            try
            {
                if (existingSample != null)
                {
                    existingSample.SampleTypeId =
                        model.SampleTypeId;

                    existingSample.IsReceived =
                        true;

                    existingSample.DateReceived =
                        DateTime.Now;

                    existingSample.TechnicianId =
                        technicianId;

                    existingSample.Status =
                        "Received";
                }
                else
                {
                    var newSample =
                        new SampleModel
                        {
                            Barcode =
                                model.Barcode,

                            SampleTypeId =
                                model.SampleTypeId,

                            TestRequestId =
                                model.TestRequestId,

                            TechnicianId =
                                technicianId,

                            DateReceived =
                                DateTime.Now,

                            IsReceived =
                                true,

                            Status =
                                "Received"
                        };

                    _dbcontext.Samples.Add(
                        newSample);
                }

                if (testRequest.Status ==
                    "Pending")
                {
                    testRequest.Status =
                        "Samples Received";
                }

                await _dbcontext.SaveChangesAsync();

                var savedSample =
                    await _dbcontext.Samples
                        .FirstOrDefaultAsync(
                            s =>
                                s.Barcode ==
                                    model.Barcode &&
                                s.TestRequestId ==
                                    model.TestRequestId);

                TempData["Success"] =
                    $"Sample '{model.Barcode}' has been successfully verified and received for {testRequest.RequestNumber}!";

                TempData["RequestNumber"] =
                    testRequest.RequestNumber;

                TempData["PatientName"] =
                    testRequest.Patient != null
                        ? testRequest.Patient.Name + " " +
                          testRequest.Patient.Surname
                        : "N/A";

                TempData["Barcode"] =
                    model.Barcode;

                TempData["SampleTypeId"] =
                    model.SampleTypeId;

                return RedirectToAction(
                    "VerifiedSample",
                    new
                    {
                        id = savedSample?.SampleId
                    });
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"An error occurred: {ex.Message}";

                return View(model);
            }
        }

        // ============================================================
        // GET: /Technician/Sample/SearchRequest
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> SearchRequest(
            string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(
                    new List<object>());
            }

            term = term.Trim();

            var results =
                await _dbcontext.TestRequests
                    .Include(r => r.Patient)
                    .Where(r =>
                        r.RequestNumber.Contains(term))
                    .Select(r => new
                    {
                        requestNumber =
                            r.RequestNumber,

                        patientName =
                            r.Patient != null
                                ? r.Patient.Name + " " +
                                  r.Patient.Surname
                                : "N/A",

                        urgency =
                            r.Urgency
                    })
                    .Take(10)
                    .ToListAsync();

            return Json(results);
        }

        // ============================================================
        // GET: /Technician/Sample/EditSample
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> EditSample(
            int id)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                TempData["Error"] =
                    "Session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var sample =
                await _dbcontext.Samples
                    .Include(s => s.SampleType)
                    .Include(s => s.TestRequest)
                        .ThenInclude(tr => tr.Patient)
                    .FirstOrDefaultAsync(
                        s => s.SampleId == id);

            if (sample == null)
            {
                TempData["Error"] =
                    "Sample not found.";

                return RedirectToAction(
                    "Samples");
            }

            if (sample.TechnicianId !=
                technicianId)
            {
                TempData["Error"] =
                    "You don't have permission to edit this sample.";

                return RedirectToAction(
                    "Samples");
            }

            ViewBag.SampleTypes =
                await _dbcontext.SampleTypes
                    .Where(s =>
                        s.Status ==
                        SampleTypeStatus.Active)
                    .Select(s => new SelectListItem
                    {
                        Value =
                            s.SampleTypeId.ToString(),

                        Text =
                            s.Name,

                        Selected =
                            s.SampleTypeId ==
                            sample.SampleTypeId
                    })
                    .ToListAsync();

            return View(sample);
        }

        // ============================================================
        // POST: /Technician/Sample/EditSample
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSample(
            int id,
            SampleModel model)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                TempData["Error"] =
                    "Session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (id != model.SampleId)
            {
                TempData["Error"] =
                    "Sample ID mismatch.";

                return RedirectToAction(
                    "Samples");
            }

            ViewBag.SampleTypes =
                await _dbcontext.SampleTypes
                    .Where(s =>
                        s.Status ==
                        SampleTypeStatus.Active)
                    .Select(s => new SelectListItem
                    {
                        Value =
                            s.SampleTypeId.ToString(),

                        Text =
                            s.Name,

                        Selected =
                            s.SampleTypeId ==
                            model.SampleTypeId
                    })
                    .ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Please correct the errors below.";

                return View(model);
            }

            try
            {
                var sample =
                    await _dbcontext.Samples
                        .FirstOrDefaultAsync(
                            s =>
                                s.SampleId == id &&
                                s.TechnicianId ==
                                    technicianId);

                if (sample == null)
                {
                    TempData["Error"] =
                        "Sample not found or you don't have permission to edit it.";

                    return RedirectToAction(
                        "Samples");
                }

                if (sample.IsDeleted)
                {
                    TempData["Error"] =
                        "Cannot edit a deleted sample.";

                    return RedirectToAction(
                        "Samples");
                }

                var wasReceived =
                    sample.IsReceived;

                sample.Barcode =
                    model.Barcode?
                        .ToUpper()
                        .Trim();

                sample.SampleTypeId =
                    model.SampleTypeId;

                sample.Status =
                    model.Status;

                sample.IsReceived =
                    model.IsReceived;

                if (model.IsReceived &&
                    !wasReceived)
                {
                    sample.DateReceived =
                        DateTime.Now;
                }

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] =
                    $"Sample '{sample.Barcode}' has been updated successfully!";

                return RedirectToAction(
                    "DetailsSample",
                    new
                    {
                        id = sample.SampleId
                    });
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] =
                    $"Database error: {ex.InnerException?.Message ?? ex.Message}";

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"An error occurred: {ex.Message}";

                return View(model);
            }
        }

        // ============================================================
        // GET: /Technician/Sample/DeleteSample
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DeleteSample(
            int id)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                TempData["Error"] =
                    "Session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var sample =
                await _dbcontext.Samples
                    .Include(s => s.SampleType)
                    .Include(s => s.TestRequest)
                        .ThenInclude(tr => tr.Patient)
                    .FirstOrDefaultAsync(
                        s => s.SampleId == id);

            if (sample == null)
            {
                TempData["Error"] =
                    "Sample not found.";

                return RedirectToAction(
                    "Samples");
            }

            if (sample.TechnicianId !=
                technicianId)
            {
                TempData["Error"] =
                    "You don't have permission to delete this sample.";

                return RedirectToAction(
                    "Samples");
            }

            if (sample.IsDeleted)
            {
                TempData["Error"] =
                    "This sample has already been deleted.";

                return RedirectToAction(
                    "Samples");
            }

            return View(sample);
        }

        // ============================================================
        // POST: /Technician/Sample/DeleteSample
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSample(
            int id,
            SampleModel model)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                TempData["Error"] =
                    "Session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            try
            {
                var sample =
                    await _dbcontext.Samples
                        .Include(s => s.TestRequest)
                        .FirstOrDefaultAsync(
                            s => s.SampleId == id);

                if (sample == null)
                {
                    TempData["Error"] =
                        "Sample not found.";

                    return RedirectToAction(
                        "Samples");
                }

                if (sample.TechnicianId !=
                    technicianId)
                {
                    TempData["Error"] =
                        "You don't have permission to delete this sample.";

                    return RedirectToAction(
                        "Samples");
                }

                if (sample.IsDeleted)
                {
                    TempData["Error"] =
                        "This sample has already been deleted.";

                    return RedirectToAction(
                        "Samples");
                }

                sample.IsDeleted =
                    true;

                sample.DeletedAt =
                    DateTime.Now;

                sample.DeletedBy =
                    technicianId;

                sample.Status =
                    "Deleted";

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] =
                    $"Sample '{sample.Barcode}' has been deleted successfully.";

                return RedirectToAction(
                    "Samples");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"An error occurred: {ex.Message}";

                return RedirectToAction(
                    "Samples");
            }
        }

        // ============================================================
        // GET: /Technician/Sample/DetailsSample
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DetailsSample(
            int id)
        {
            var technicianId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(technicianId))
            {
                TempData["Error"] =
                    "Session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var sample =
                await _dbcontext.Samples
                    .Include(s => s.SampleType)
                    .Include(s => s.TestRequest)
                        .ThenInclude(tr => tr.Patient)
                    .Include(s => s.TestRequest)
                        .ThenInclude(tr => tr.TechnicianTests)
                            .ThenInclude(rt => rt.TestType)
                    .Include(s => s.Technician)
                    .FirstOrDefaultAsync(
                        s => s.SampleId == id);

            if (sample == null)
            {
                TempData["Error"] =
                    "Sample not found.";

                return RedirectToAction(
                    "Samples");
            }

            if (sample.TechnicianId !=
                technicianId)
            {
                TempData["Error"] =
                    "You don't have permission to view this sample.";

                return RedirectToAction(
                    "Samples");
            }

            var relatedSamples =
                await _dbcontext.Samples
                    .Include(s => s.SampleType)
                    .Where(s =>
                        s.TestRequestId ==
                            sample.TestRequestId &&
                        s.SampleId != id)
                    .OrderBy(s => s.DateReceived)
                    .ToListAsync();

            ViewBag.RelatedSamples =
                relatedSamples;

            var testResults =
                await _dbcontext.TestRequestTests
                    .Include(rt => rt.TestType)
                    .Where(rt =>
                        rt.TestRequestId ==
                        sample.TestRequestId)
                    .ToListAsync();

            ViewBag.TestResults =
                testResults;

            return View(sample);
        }
    }
}
