using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.ViewModels;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Authorize(Roles = "Technician")]
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
        //List Doctor Requests
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
        //Verify Request
        [HttpGet]
      
        public async Task<IActionResult> ProcessSample(int id)
        {
            var request = await _dbcontext.TestRequests
                .FirstOrDefaultAsync(r => r.TestRequestId == id);

            if (request == null)
                return NotFound();

            var model = new ReceiveSampleViewModel
            {
                RequestId = request.TestRequestId,
                RequestNumber = request.RequestNumber,
                ReceivedDateTime = DateTime.Now,
                TechnicianId = _userManager.GetUserId(User)!
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessSample(ReceiveSampleViewModel model)
        {
            var request = await _dbcontext.TestRequests
                .Include(r => r.Samples)
                .FirstOrDefaultAsync(r => r.TestRequestId == model.RequestId);

            if (request == null)
                return BadRequest("Invalid request");

            var sample = request.Samples
                .FirstOrDefault(s => s.Barcode == model.Barcode);

            if (sample == null)
                return BadRequest("Invalid sample");

            if (sample.Status == "Received")
                return BadRequest("Sample already received");

            sample.Status = "SampleReceived";
            sample.TechnicianId = _userManager.GetUserId(User)!;
            sample.DateReceived = DateTime.Now;

            await _dbcontext.SaveChangesAsync();

            TempData["Success"] = "Sample received successfully!";
            return RedirectToAction("ProcessSample", new { id = model.RequestId });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyQRCode(string qrData)
        {
            if (string.IsNullOrEmpty(qrData) || !qrData.Contains("|"))
                return BadRequest("Invalid QR format");

            var parts = qrData.Split('|');

            if (parts.Length < 2)
                return BadRequest("Invalid QR format");

            var requestPart = parts[0].Replace("REQ:", "");
            var barcodePart = parts[1].Replace("BAR:", "");

            if (!int.TryParse(requestPart, out int requestId))
                return BadRequest("Invalid request ID");

            // ✅ FIX: pass ViewModel instead
            var model = new ReceiveSampleViewModel
            {
                RequestId = requestId,
                Barcode = barcodePart
            };

            return await ProcessSample(model);
        }
    }
}

