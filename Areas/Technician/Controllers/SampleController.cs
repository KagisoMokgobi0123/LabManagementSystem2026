using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
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
        public async Task<IActionResult> VerifySample()
        {
            // Load sample types for dropdown
            var sampleTypes = await _dbcontext.SampleTypes
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
            new SelectListItem { Value = "1", Text = "Blood" },
            new SelectListItem { Value = "2", Text = "Urine" },
            new SelectListItem { Value = "3", Text = "Saliva" },
            new SelectListItem { Value = "4", Text = "Plasma" },
            new SelectListItem { Value = "5", Text = "Serum" }
        };

                // Optional: Show warning in TempData
                TempData["Warning"] = "Sample types loaded from fallback data. Please seed the database.";
            }
            else
            {
                ViewBag.SampleTypes = sampleTypes;
            }

            return View(new SampleModel());
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
        [HttpGet] // List all samples (verified, unverified)
        public async Task<IActionResult> Samples(DateTime? dateFrom, DateTime? dateTo, int? sampleTypeId, string status, string searchTerm)
        {
            var technicianId = _userManager.GetUserId(User);

            // Load sample types for filter dropdown
            ViewBag.SampleTypes = await _dbcontext.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            // Build query
            var query = _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Include(s => s.Technician)
                .Where(s => s.TechnicianId == technicianId);

            // Apply filters
            if (dateFrom.HasValue)
                query = query.Where(s => s.DateReceived >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(s => s.DateReceived <= dateTo.Value.Date.AddDays(1));

            if (sampleTypeId.HasValue && sampleTypeId.Value > 0)
                query = query.Where(s => s.SampleTypeId == sampleTypeId.Value);

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                bool isReceived = status == "Received";
                query = query.Where(s => s.IsReceived == isReceived);
            }

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

            // Store filter values in ViewBag to persist in form
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.SelectedSampleTypeId = sampleTypeId;
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = searchTerm;

            return View(samples);
        }

        [HttpGet]
        public async Task<IActionResult> EditSample(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var sample = await _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found or you don't have permission to edit it.";
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
            if (id != model.SampleId)
            {
                TempData["Error"] = "Sample ID mismatch.";
                return RedirectToAction("Samples");
            }

            var technicianId = _userManager.GetUserId(User);

            var sample = await _dbcontext.Samples
                .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found or you don't have permission to edit it.";
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
                // Update sample properties
                sample.Barcode = model.Barcode;
                sample.SampleTypeId = model.SampleTypeId;
                sample.Status = model.Status;
                sample.IsReceived = model.IsReceived;

                if (model.IsReceived && !sample.IsReceived)
                {
                    sample.DateReceived = DateTime.Now;
                }

                await _dbcontext.SaveChangesAsync();

                TempData["Success"] = $"Sample '{sample.Barcode}' has been updated successfully!";
                return RedirectToAction("Samples");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSample(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var sample = await _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found or you don't have permission to delete it.";
                return RedirectToAction("Samples");
            }

            return View(sample);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSampleConfirmed(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var sample = await _dbcontext.Samples
                .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found or you don't have permission to delete it.";
                return RedirectToAction("Samples");
            }

            try
            {
                string barcode = sample.Barcode;
                _dbcontext.Samples.Remove(sample);
                await _dbcontext.SaveChangesAsync();

                TempData["Success"] = $"Sample '{barcode}' has been deleted successfully!";
                return RedirectToAction("Samples");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting: {ex.Message}";
                return RedirectToAction("Samples");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SampleDetails(int id)
        {
            var technicianId = _userManager.GetUserId(User);

            var sample = await _dbcontext.Samples
                .Include(s => s.SampleType)
                .Include(s => s.TestRequest)
                    .ThenInclude(tr => tr.Patient)
                .Include(s => s.Technician)
                .FirstOrDefaultAsync(s => s.SampleId == id && s.TechnicianId == technicianId);

            if (sample == null)
            {
                TempData["Error"] = "Sample not found.";
                return RedirectToAction("Samples");
            }

            return View(sample);
        }







    }
}
