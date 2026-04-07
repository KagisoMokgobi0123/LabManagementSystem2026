using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class ReceiveSampleController : Controller
    {

        // 1. Declare the field
        private readonly ApplicationDbContext _dbcontext;
        public ReceiveSampleController(ApplicationDbContext context)
        {
            _dbcontext = context;
        }


        [HttpGet]
        public async Task<IActionResult> ReceiveSample(int requestId)
        {
            // Fetch the request so we can show Patient Name/Request Number on the screen
            var request = await _dbcontext.TestRequests
                .Include(r => r.Patient) // Assuming you have a Patient relationship
                .FirstOrDefaultAsync(r => r.TestRequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            // Pre-fill the model for the View
            var model = new SampleModel
            {
                TestRequestId = requestId
            };

            // Pass the request details to the View using ViewBag for display only
            ViewBag.RequestNumber = request.RequestNumber;
            ViewBag.PatientName = request.Patient?.FullName; 

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveSample(SampleModel model)
        {
            // 1. Check for duplicate barcode BEFORE generating a new one (if input from user)
            // Note: If you generate it automatically (Step 4), you might not need this check here.
            var exists = await _dbcontext.Samples
                .AnyAsync(s => s.BarcodeNumber == model.BarcodeNumber);

            if (exists)
            {
                ModelState.AddModelError("BarcodeNumber", "This barcode has already been scanned.");
            }

            if (ModelState.IsValid)
            {
                // 2. Get logged-in technician ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 3. Assign metadata
                model.TechnicianId = userId!;
                model.RecievedDateTime = DateTime.UtcNow;

                // 4. Generate unique barcode
                // Using a Prefix + Short Guid helps with readability in the lab
                model.BarcodeNumber = "SMP-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // 5. Update the parent TestRequest status
                var request = await _dbcontext.TestRequests
                    .FirstOrDefaultAsync(r => r.TestRequestId == model.TestRequestId);

                if (request != null)
                {
                    request.Status = "Samples Received";
                    // No need for _dbcontext.Update(request) if it's tracked by the context
                }

                // 6. Add the sample to the database
                _dbcontext.Samples.Add(model);

                await _dbcontext.SaveChangesAsync();

                // Redirect to the list or details page
                return RedirectToAction("Details", "TestRequests", new { id = model.TestRequestId });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _dbcontext.Samples.FindAsync(id);
            if (sample == null)
            {
                return NotFound();
            }

            // Optional: If you need a dropdown for TestRequests or Technicians
            ViewBag.TestRequestId = new SelectList(_dbcontext.TestRequests, "TestRequestId", "RequestNumber", sample.TestRequestId);

            return View(sample);
        }
        // POST: Samples/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SampleModel model)
        {
            if (id != model.SampleId) // Ensure the ID in the URL matches the Model ID
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update metadata if needed, or keep original received date
                    // model.LastUpdated = DateTime.Now;

                    _dbcontext.Update(model);
                    await _dbcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleExists(model.SampleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Something else went wrong
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Helper method to check if the record still exists
        private bool SampleExists(int id)
        {
            return _dbcontext.Samples.Any(e => e.SampleId == id);
        }
    }
}
