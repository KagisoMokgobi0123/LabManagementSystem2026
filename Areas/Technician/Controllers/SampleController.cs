using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class SampleController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        public SampleController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> VerifiedSample()
        {
            var technicianId = _userManager.GetUserId(User);

            var samples = await _dbcontext.Samples
                .Where(s => s.IsReceived == true &&
                            s.TechnicianId == technicianId)
                .OrderByDescending(s => s.DateReceived)
                .ToListAsync();

            return View(samples);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifySample(SampleModel model)
        {
            // Get logged-in technician
            var technicianId = User.Identity?.Name;

            if (string.IsNullOrEmpty(technicianId))
                return Unauthorized();

            // Basic validation
            if (!ModelState.IsValid)
                return View("VerifySample", model);

            // Check request exists
            var request = await _dbcontext.TestRequests
                .FirstOrDefaultAsync(r => r.RequestNumber == model.RequestNumber);

            if (request == null)
            {
                ModelState.AddModelError("", "Invalid request number.");
                return View("VerifySample", model);
            }

            // Find sample
            var sample = await _dbcontext.Samples
                .FirstOrDefaultAsync(s =>
                    s.Barcode == model.Barcode &&
                    s.RequestNumber == model.RequestNumber);

            if (sample == null)
            {
                ModelState.AddModelError("", "Sample barcode does not match request.");
                return View("VerifySample", model);
            }

            // Prevent duplicate receiving
            if (sample.IsReceived)
            {
                ModelState.AddModelError("", "This sample has already been received.");
                return View("VerifySample", model);
            }

            // Validate sample type
            if (sample.SampleType != model.SampleType)
            {
                ModelState.AddModelError("", "Incorrect sample type.");
                return View("VerifySample", model);
            }

            // VERIFY / RECEIVE SAMPLE
            sample.IsReceived = true;
            sample.DateReceived = DateTime.Now;
            sample.TechnicianId = technicianId;

            // (Optional) update request status
            request.Status = "Samples Received";

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Sample verified and received successfully.";

            return RedirectToAction("VerifiedSample");
        }



      
       

        

    }
}
