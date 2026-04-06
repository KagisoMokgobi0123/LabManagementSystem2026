using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public async Task<IActionResult> ReceiveSample(SampleModel model)
        {

            //Check duplicate barcode
            var exists = await _dbcontext.Samples
                .AnyAsync(s => s.BarcodeNumber == model.BarcodeNumber);

            if (exists)
            {
                ModelState.AddModelError("BarcodeNumber", "Barcode already exists.");
            }
            //if the model is valid, save the sample to the database and redirect to the sample list page
            if (ModelState.IsValid)
            {
                // 1. Get logged-in technician
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //2. Assign technician
                model.TechnicianId = userId!;
                model.RecievedDateTime = DateTime.Now;

                // 3.Generate barcode HERE
                model.BarcodeNumber = "SMP-" + Guid.NewGuid().ToString().Substring(0, 8);
                // 4. Update TestRequest status
                var request = await _dbcontext.TestRequests
                    .FirstOrDefaultAsync(r => r.TestRequestId == model.TestRequestId);

                if (request != null)
                {
                    request.Status = "Samples Received";
                }
                await _dbcontext.SaveChangesAsync();

                return RedirectToAction("Details", new { id = model.TestRequestId });
            }

            return View();

        }
            
        
    }
}
