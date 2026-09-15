using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Models.Account;
using Microsoft.AspNetCore.Authorization;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "Technician ")]
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
        //loading the pending test requests
        //that need their samples received and sending them
        public async Task<IActionResult> Index()
        {
            var requests = await _context.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.TestType)
                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)
                .Where(r => r.Status == TestRequestStatus.Submitted.ToString())
                .ToListAsync();

            return View(requests);
        }
        // GET: Technician/ReceiveSamples/Receive
        // Its job is to load the Receive Sample page
        // and populate it with the correct
        // request, patient, doctor, and sample information.
        // GET: Technician/ReceiveSamples/Receive
        [HttpGet]
        public async Task<IActionResult> Receive(string? requestNumber)
        {
            var model = new ReceiveSampleViewModel();

            var technician = await _userManager.GetUserAsync(User);

            if (technician != null)
            {
                model.TechnicianName = technician.FullName;
            }

            await LoadSampleTypesAsync(model);

            if (string.IsNullOrEmpty(requestNumber))
            {
                ModelState.AddModelError(
                    "RequestNumber",
                    "Request number is required."
                );

                return PartialView("_ReceiveSampleModal", model);
            }

            var request = await _context.TestRequests
     .Include(r => r.Patient)
     .Include(r => r.Doctor)
     .Include(r => r.Samples)
         .ThenInclude(s => s.SampleType)
     .FirstOrDefaultAsync(r =>
         r.RequestNumber == requestNumber);

            if (request == null)
            {
                ModelState.AddModelError(
                    "RequestNumber",
                    "Test request was not found."
                );

                return PartialView("_ReceiveSampleModal", model);
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
                model.SampleTypeName = sample.SampleType?.Name;
            }

            model.DateTimeReceived = DateTime.Now;

            return PartialView("_ReceiveSampleModal", model);
        }


        // POST: Technician/ReceiveSamples/Receive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(ReceiveSampleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
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

                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
            }

            var sample = request.Samples
                .FirstOrDefault(s => s.SampleId == model.SampleID);

            if (sample == null)
            {
                ModelState.AddModelError(
                    "Barcode",
                    "Sample was not found."
                );

                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
            }

            // Verify barcode
            if (sample.Barcode != model.Barcode)
            {
                ModelState.AddModelError(
                    "Barcode",
                    "The barcode does not match the sample."
                );

                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
            }

            // Verify sample type
            if (sample.SampleTypeId != model.SampleTypeId)
            {
                ModelState.AddModelError(
                    "SampleTypeId",
                    "The sample type does not match the request."
                );

                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
            }

            // Get logged-in technician
            var technician = await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to identify the logged-in technician."
                );

                await LoadSampleTypesAsync(model);

                return PartialView("_ReceiveSampleModal", model);
            }

            // Record receiving information
            sample.TechnicianId = technician.Id;
            sample.DateReceived = DateTime.Now;

            // Update request status
            request.Status = TestRequestStatus.SamplesReceived.ToString();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Sample successfully received, sample: " +
                sample.Barcode +
                " has been verified.";

            return RedirectToAction(nameof(Index));
        }


        private async Task LoadSampleTypesAsync(
            ReceiveSampleViewModel model)
        {
            model.SampleTypes = await _context.SampleTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SampleTypeId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }
    }
}
