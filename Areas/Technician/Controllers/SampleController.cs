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


        // GET: TestRequests/Receive/5
        public async Task<IActionResult> Sample(int id)
        {
            var request = await _dbcontext.TestRequests.FindAsync(id);
            if (request == null) return NotFound();

            var viewModel = new SampleModel
            {
                TestRequestId = request.TestRequestId,
                RequestNumber = request.RequestNumber,
                TechnicianId = _userManager.GetUserId(User) // Get current technician ID
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sample(SampleModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await Sample(model.TestRequestId, model.Barcode, model.SampleType, model.TechnicianId);
                if (success)
                {
                    TempData["Success"] = "Sample verified and received successfully.";
                    return RedirectToAction("Index"); // Redirect to your dashboard
                }
            }
            return View(model);
        }

        public async Task<bool> Sample(int requestId, string barcode, string sampleType, string technicianId)
        {
            //GET TestRequest
            var request = await _dbcontext.TestRequests
                .FirstOrDefaultAsync(r => r.TestRequestId == requestId);

            if (request == null) return false;

            // 1. Update the Status
            request.Status = "Samples Received";

            // 2. Log the Verification details 


            await _dbcontext.SaveChangesAsync();
            return true;
        }

    }
}
