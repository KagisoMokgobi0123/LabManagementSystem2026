using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "Technician")]
    public class SampleController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        public SampleController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Samples(DateTime? dateFrom, DateTime? dateTo, int? sampleTypeId, string status, string searchTerm)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(technicianId))
            {
                return RedirectToAction("LoginUser", "Account");
            }

            // Build query
            var query = _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Where(s => s.TechnicianId == technicianId);

            // Apply filters
            if (dateFrom.HasValue)
                query = query.Where(s => s.DateReceived >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(s => s.DateReceived <= dateTo.Value.Date.AddDays(1));

            if (sampleTypeId.HasValue && sampleTypeId.Value > 0)
                query = query.Where(s => s.SampleTypeId == sampleTypeId.Value);

            if (!string.IsNullOrEmpty(status) && status != "All Status")
                query = query.Where(s => s.Status == status);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Barcode.Contains(searchTerm) ||
                                         s.TestRequest.RequestNumber.Contains(searchTerm) ||
                                         (s.TestRequest.Patient != null && s.TestRequest.Patient.FullName.Contains(searchTerm)) ||
                                         (s.SampleType != null && s.SampleType.Name.Contains(searchTerm)));
            }

            var samples = await query
                .OrderByDescending(s => s.DateReceived)
                .ToListAsync();

            // Load filters for dropdown
            ViewBag.SampleTypes = await _dbcontext.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            ViewBag.StatusList = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "All Status" },
        new SelectListItem { Value = "Received", Text = "Received" },
        new SelectListItem { Value = "Pending", Text = "Pending" },
        new SelectListItem { Value = "Processing", Text = "Processing" },
        new SelectListItem { Value = "Completed", Text = "Completed" },
        new SelectListItem { Value = "Rejected", Text = "Rejected" }
    };

            // Store filter values in ViewBag to persist in form
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.SelectedSampleTypeId = sampleTypeId;
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = searchTerm;

            return View(samples);
        }
        [HttpGet]
        public async Task<IActionResult> VerifySample(int? testRequestId = null)
        {
            // Get current technician info
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
            {
                return RedirectToAction("LoginUser", "Account");
            }

            var technician = await _userManager.FindByNameAsync(technicianId);
            if (technician == null)
            {
                return RedirectToAction("LoginUser", "Account");
            }

            // Load sample types for dropdown
            var sampleTypes = await _dbcontext.SampleTypes
                .Where(st => st.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            // Debug: Check if data exists
            if (!sampleTypes.Any())
            {
                // Add fallback data if table is empty
                ViewBag.SampleTypes = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "🩸 Blood" },
            new SelectListItem { Value = "2", Text = "💧 Urine" },
            new SelectListItem { Value = "3", Text = "💦 Saliva" },
            new SelectListItem { Value = "4", Text = "🧪 Plasma" },
            new SelectListItem { Value = "5", Text = "🧫 Serum" },
            new SelectListItem { Value = "6", Text = "🔬 Tissue" },
            new SelectListItem { Value = "7", Text = "🫘 Stool" },
            new SelectListItem { Value = "8", Text = "🧠 CSF" },
            new SelectListItem { Value = "9", Text = "🫁 Sputum" },
            new SelectListItem { Value = "10", Text = "🦠 Swab" }
        };

                TempData["Warning"] = "Sample types loaded from fallback data. Please seed the database.";
            }
            else
            {
                ViewBag.SampleTypes = sampleTypes;
            }

            // Load recent pending requests for quick selection
            var recentPendingRequests = await _dbcontext.TestRequests
                .Include(tr => tr.Patient)
                .Where(tr => tr.Status == "Pending" || tr.Status == "Submitted")
                .OrderByDescending(tr => tr.RequestDate)
                .Take(5)
                .Select(tr => new
                {
                    tr.TestRequestId,
                    tr.RequestNumber,
                    PatientName = tr.Patient != null ? tr.Patient.FullName : "N/A",
                    tr.RequestDate,
                    Urgency = tr.Urgency.ToString()
                })
                .ToListAsync();

            ViewBag.RecentPendingRequests = recentPendingRequests;

            // Load technician's recent verifications
            var recentVerifications = await _dbcontext.Samples
                .Include(s => s.TestRequest)
                .Where(s => s.TechnicianId == technicianId && s.IsReceived)
                .OrderByDescending(s => s.DateReceived)
                .Take(5)
                .Select(s => new
                {
                    s.SampleId,
                    s.Barcode,
                    RequestNumber = s.TestRequest != null ? s.TestRequest.RequestNumber : "N/A",
                    s.DateReceived
                })
                .ToListAsync();

            ViewBag.RecentVerifications = recentVerifications;

            // Load statistics for the technician
            var today = DateTime.Now.Date;

            var samplesReceivedToday = await _dbcontext.Samples
                .CountAsync(s => s.TechnicianId == technicianId && s.DateReceived.Date == today);

            var pendingRequests = await _dbcontext.TestRequests
                .CountAsync(tr => tr.Status == "Pending" || tr.Status == "Submitted");

            var urgentPending = await _dbcontext.TestRequests
                .CountAsync(tr => (tr.Status == "Pending" || tr.Status == "Submitted") && tr.Urgency == Urgency.Stat);

            var stats = new
            {
                SamplesReceivedToday = samplesReceivedToday,
                PendingRequests = pendingRequests,
                UrgentPending = urgentPending
            };

            ViewBag.Stats = stats;

            // Create view model
            var model = new SampleModel();
            if (testRequestId.HasValue)
            {
                model.TestRequestId = testRequestId.Value;
                var testRequest = await _dbcontext.TestRequests
                    .Include(tr => tr.Patient)
                    .FirstOrDefaultAsync(tr => tr.TestRequestId == testRequestId.Value);

                if (testRequest != null)
                {
                    ViewBag.PreSelectedRequestNumber = testRequest.RequestNumber;
                    ViewBag.PreSelectedPatientName = testRequest.Patient?.FullName ?? "N/A";
                    ViewBag.PreSelectedUrgency = testRequest.Urgency.ToString();
                    ViewBag.PreSelectedClinicalNotes = testRequest.ClinicalNotes;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifySample(SampleModel model)
        {
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
                return Unauthorized();

            // Load dropdown for sample types (in case of validation error)
            ViewBag.SampleTypes = await _dbcontext.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields correctly.";
                return View(model);
            }

            // Find the test request
            var testRequest = await _dbcontext.TestRequests
                .Include(tr => tr.Patient)
                .FirstOrDefaultAsync(r => r.TestRequestId == model.TestRequestId);

            if (testRequest == null)
            {
                TempData["Error"] = "Invalid request number. Please search and select a valid request.";
                return View(model);
            }

            // Check if sample with this barcode already exists for this request
            var existingSample = await _dbcontext.Samples
                .FirstOrDefaultAsync(s => s.Barcode == model.Barcode && s.TestRequestId == model.TestRequestId);

            if (existingSample != null && existingSample.IsReceived)
            {
                TempData["Error"] = $"⚠️ Sample '{model.Barcode}' has already been received on {existingSample.DateReceived:dd MMM yyyy HH:mm}";
                return View(model);
            }

            try
            {
                if (existingSample != null)
                {
                    // Update existing sample
                    existingSample.SampleTypeId = model.SampleTypeId;
                    existingSample.IsReceived = true;
                    existingSample.DateReceived = DateTime.Now;
                    existingSample.TechnicianId = technicianId;
                    existingSample.Status = "Received";
                }
                else
                {
                    // Create new sample
                    var newSample = new SampleModel
                    {
                        Barcode = model.Barcode,
                        SampleTypeId = model.SampleTypeId,
                        TestRequestId = model.TestRequestId,
                        TechnicianId = technicianId,
                        DateReceived = DateTime.Now,
                        IsReceived = true,
                        Status = "Received"
                    };
                    _dbcontext.Samples.Add(newSample);
                }

                // Update test request status
                if (testRequest.Status == "Pending")
                {
                    testRequest.Status = "Samples Received";
                }

                await _dbcontext.SaveChangesAsync();

                // Get the sample ID for redirect
                var savedSample = await _dbcontext.Samples
                    .FirstOrDefaultAsync(s => s.Barcode == model.Barcode && s.TestRequestId == model.TestRequestId);

                // Set success message in TempData
                TempData["Success"] = $"✅ Sample '{model.Barcode}' has been successfully verified and received for {testRequest.RequestNumber}!";
                TempData["RequestNumber"] = testRequest.RequestNumber;
                TempData["PatientName"] = testRequest.Patient?.FullName;
                TempData["Barcode"] = model.Barcode;
                TempData["SampleTypeId"] = model.SampleTypeId;

                return RedirectToAction("VerifiedSample", new { id = savedSample?.SampleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ An error occurred: {ex.Message}";
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchRequest(string term)
        {
            var results = await _dbcontext.TestRequests
                .Include(r => r.Patient)
                .Where(r => r.RequestNumber.Contains(term))
                .Select(r => new
                {
                    requestNumber = r.RequestNumber,
                    patientName = r.Patient.FullName, // adjust if needed
                    urgency = r.Urgency
                })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }
    
        [HttpGet]
        public async Task<IActionResult> EditSample(int id)
        {
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            // Find the sample with related data
            var sample = await _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .FirstOrDefaultAsync(s => s.SampleId == id);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found.";
                return RedirectToAction("Samples");
            }

            // Check if technician has permission to edit this sample
            if (sample.TechnicianId != technicianId)
            {
                TempData["Error"] = "You don't have permission to edit this sample.";
                return RedirectToAction("Samples");
            }

            // Load sample types for dropdown
            ViewBag.SampleTypes = await _dbcontext.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name,
                    Selected = s.SampleTypeId == sample.SampleTypeId
                })
                .ToListAsync();

            return View(sample);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSample(int id, SampleModel model)
        {
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            if (id != model.SampleId)
            {
                TempData["Error"] = "Sample ID mismatch.";
                return RedirectToAction("Samples");
            }

            // Load sample types for dropdown (in case of validation error)
            ViewBag.SampleTypes = await _dbcontext.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name,
                    Selected = s.SampleTypeId == model.SampleTypeId
                })
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                // Find the existing sample
                var sample = await _dbcontext.Samples
                    .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

                if (sample == null)
                {
                    TempData["Error"] = "Sample not found or you don't have permission to edit it.";
                    return RedirectToAction("Samples");
                }

                // Check if sample is deleted
                if (sample.IsDeleted)
                {
                    TempData["Error"] = "Cannot edit a deleted sample. Please restore it first.";
                    return RedirectToAction("Samples");
                }

                // Update sample properties
                sample.Barcode = model.Barcode?.ToUpper().Trim();
                sample.SampleTypeId = model.SampleTypeId;
                sample.Status = model.Status;
                sample.IsReceived = model.IsReceived;

                // If status changed to Received and wasn't received before
                if (model.IsReceived && !sample.IsReceived)
                {
                    sample.DateReceived = DateTime.Now;
                }

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] = $"✅ Sample '{sample.Barcode}' has been updated successfully!";
                return RedirectToAction("SampleDetails", new { id = sample.SampleId });
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ An error occurred: {ex.Message}";
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteSample(int id)
        {
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            // Find the sample with related data
            var sample = await _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .FirstOrDefaultAsync(s => s.SampleId == id);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found.";
                return RedirectToAction("Samples");
            }

            // Check if technician has permission to delete this sample
            if (sample.TechnicianId != technicianId)
            {
                TempData["Error"] = "You don't have permission to delete this sample.";
                return RedirectToAction("Samples");
            }

            // Check if sample is already deleted
            if (sample.IsDeleted)
            {
                TempData["Error"] = "This sample has already been deleted.";
                return RedirectToAction("Samples");
            }

            return View(sample);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSample(int id, SampleModel model)
        {
            var technicianId = User.Identity?.Name;
            if (string.IsNullOrEmpty(technicianId))
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Find the sample
                var sample = await _dbcontext.Samples
                    .Include(s => s.TestRequest)
                    .FirstOrDefaultAsync(s => s.SampleId == id);

                if (sample == null)
                {
                    TempData["Error"] = "Sample not found.";
                    return RedirectToAction("Samples");
                }

                // Check if technician has permission to delete this sample
                if (sample.TechnicianId != technicianId)
                {
                    TempData["Error"] = "You don't have permission to delete this sample.";
                    return RedirectToAction("Samples");
                }

                // Check if sample is already deleted
                if (sample.IsDeleted)
                {
                    TempData["Error"] = "This sample has already been deleted.";
                    return RedirectToAction("Samples");
                }

                // Soft delete - mark as deleted instead of removing from database
                sample.IsDeleted = true;
                sample.DeletedAt = DateTime.Now;
                sample.DeletedBy = technicianId;
                sample.Status = "Deleted";

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] = $"✅ Sample '{sample.Barcode}' has been deleted successfully.";

                return RedirectToAction("Samples");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ An error occurred: {ex.Message}";
                return RedirectToAction("Samples");
            }
        }
       [HttpGet]
public async Task<IActionResult> DetailsSample(int id)
{
    var technicianId = User.Identity?.Name;
    if (string.IsNullOrEmpty(technicianId))
    {
        TempData["Error"] = "Session expired. Please login again.";
        return RedirectToAction("Login", "Account");
    }

    // Find the sample with all related data
    var sample = await _dbcontext.Samples
        .Include(s => s.SampleType)
        .Include(s => s.TestRequest)
            .ThenInclude(tr => tr.Patient)
        .Include(s => s.TestRequest)
            .ThenInclude(tr => tr.TechnicianTests)
            .ThenInclude(rt => rt.TestType)
        .Include(s => s.Technician)
        .FirstOrDefaultAsync(s => s.SampleId == id);

    if (sample == null)
    {
        TempData["Error"] = "Sample not found.";
        return RedirectToAction("Samples");
    }

    // Check if technician has permission to view this sample
    if (sample.TechnicianId != technicianId)
    {
        TempData["Error"] = "You don't have permission to view this sample.";
        return RedirectToAction("Samples");
    }

    // Get all samples for the same test request
    var relatedSamples = await _dbcontext.Samples
        .Include(s => s.SampleType)
        .Where(s => s.TestRequestId == sample.TestRequestId && s.SampleId != id)
        .OrderBy(s => s.DateReceived)
        .ToListAsync();

    ViewBag.RelatedSamples = relatedSamples;
    
    // Get test results for this request
    var testResults = await _dbcontext.TestRequestTests
        .Include(rt => rt.TestType)
        .Where(rt => rt.TestRequestId == sample.TestRequestId)
        .ToListAsync();
    
    ViewBag.TestResults = testResults;

    return View(sample);
}







    }
}
