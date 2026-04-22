using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.ViewModels;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    public class TestRequestController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        public TestRequestController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> ListRequests()
        {
        var pendingRequests = await _dbcontext.TestRequests
       .Where(r => r.Status == "Submitted")
       .Include(r => r.Patient)
       .Include(r => r.Doctor)
       .Include(r => r.Samples)
       .Include(r => r.TestTypes)
       .ToListAsync();

            return View(pendingRequests);
        }
        [HttpGet]
        public async Task<IActionResult> Receive(int id)
        {
            var request = await _dbcontext.TestRequests
                .FirstOrDefaultAsync(r => r.TestRequestId == id);

            if (request == null) return NotFound();

            var viewModel = new ReceiveSampleViewModel
            {
                TestRequestId = request.TestRequestId,
                RequestNumber = request.RequestNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(SampleModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            try
            {
                // 1. Find the parent Test Request
                var request = await _dbcontext.TestRequests
                    .FirstOrDefaultAsync(r => r.TestRequestId == model.TestRequestId);

                if (request == null) return NotFound();

                // 2. Create the Sample Record
                var sample = new SampleModel
                {
                    TestRequestId = request.TestRequestId,
                    Barcode = model.Barcode,
                    SampleType = model.SampleType,
                    DateReceived = DateTime.Now // Use local time for lab logs
                };

                // 3. Update Request Status
                request.Status = "Samples Received";

                _dbcontext.Samples.Add(sample);
                await _dbcontext.SaveChangesAsync();

                // Commit both changes at once
                await transaction.CommitAsync();

                TempData["Success"] = $"Request {request.RequestNumber} verified. Status: Samples Received.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while receiving the sample. Please try again.");
                return View(model);
            }
        }
    }
}
