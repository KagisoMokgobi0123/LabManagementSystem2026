using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class ReceiveSamplesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;

        public ReceiveSamplesController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Technician/ReceiveSamples
        public IActionResult Index()
        {
            return View();
        }

        // GET: Technician/ReceiveSamples/Receive
        // Its job is to load the Receive Sample page
        // and populate it with the correct
        // request, patient, doctor, and sample information.
        [HttpGet]
        public async Task<IActionResult> Receive(string? requestNumber)
        {   

            var model = new ReceiveSampleViewModel();
            var technician = await _userManager.GetUserAsync(User);

            if (technician != null)
            {
                model.TechnicianName = technician.FullName;
            }

            model.SampleTypes = await _context.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(requestNumber))
            {
                var request = await _context.TestRequests
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .Include(r => r.Samples)
                    .FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);

                if (request == null)
                {
                    ModelState.AddModelError(
                        "RequestNumber",
                        "Test request was not found."
                    );

                    return View(model);
                }

                var sample = request.Samples.FirstOrDefault();

                model.TestRequestId = request.TestRequestId;
                model.RequestNumber = request.RequestNumber;

                if (request.Patient != null)
                {
                    model.PatientName = request.Patient.FullName;
                }

                if (request.Doctor != null)
                {
                    model.DoctorName = request.Doctor.FullName;
                }

                if (sample != null)
                {
                    model.SampleID = sample.SampleId;
                    model.Barcode = sample.Barcode;
                    model.SampleTypeId = sample.SampleTypeId;
                }
                // Set the receiving date/time
                model.DateTimeReceived = DateTime.Now;
            }

            return View(model);
        }

        // POST: Technician/ReceiveSamples/Receive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(ReceiveSampleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SampleTypes = await _context.SampleTypes
                    .Select(s => new SelectListItem
                    {
                        Value = s.SampleTypeId.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync();

                return View(model);
            }

            var request = await _context.TestRequests
                .Include(r => r.Samples)
                .FirstOrDefaultAsync(r =>
                    r.RequestNumber == model.RequestNumber);

            if (request == null)
            {
                ModelState.AddModelError(
                    "RequestNumber",
                    "Test request was not found."
                );

                return View(model);
            }

            var sample = request.Samples
                .FirstOrDefault(s => s.SampleId == model.SampleID);

            if (sample == null)
            {
                ModelState.AddModelError(
                    "Barcode",
                    "Sample was not found."
                );

                return View(model);
            }

            // Verify barcode
            if (sample.Barcode != model.Barcode)
            {
                ModelState.AddModelError(
                    "Barcode",
                    "The barcode does not match the sample."
                );

                return View(model);
            }

            // Verify sample type
            if (sample.SampleTypeId != model.SampleTypeId)
            {
                ModelState.AddModelError(
                    "SampleTypeId",
                    "The sample type does not match the request."
                );

                return View(model);
            }

            // Get logged-in technician
            var technicianId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;

            if (string.IsNullOrEmpty(technicianId))
            {
                ModelState.AddModelError(
                    "",
                    "Unable to identify the logged-in technician."
                );

                return View(model);
            }

            // Record receiving information
            sample.TechnicianId = technicianId;
            sample.DateReceived = DateTime.Now;

            // Update request status
            request.Status = TestRequestStatus.SamplesReceived.ToString();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Sample successfully received, sample:" + sample.Barcode + " has been verified.";

            return RedirectToAction(nameof(Index));
        }
    }
}
