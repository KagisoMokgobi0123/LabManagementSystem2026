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
    [Authorize(Roles = "Technician")]
    public class ReceiveSamplesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;

        public ReceiveSamplesController(ApplicationDbContext context, UserManager<RegisterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      
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
        //verified samples 
        public async Task<IActionResult> VerifiedSamples()
        {
            var requests = await _context.TestRequests

                .AsNoTracking()

                .Include(r => r.Patient)

                .Include(r => r.Doctor)

                .Include(r => r.TestType)

                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)

                .Include(r => r.Samples)
                    .ThenInclude(s => s.Technician)

                // The sample itself determines whether it has
                // completed laboratory reception.
                .Where(r =>
                    r.Samples.Any(s =>
                        s.IsReceived &&
                        !s.IsDeleted))

                .OrderByDescending(r => r.RequestDate)

                .ToListAsync();


            return View(requests);
        }        // Its job is to load the Receive Sample page
        // and populate it with the correct
        // request, patient, doctor, and sample information.
        // GET: Technician/ReceiveSamples/Receive
        [HttpGet]
        public async Task<IActionResult> Receive(string? requestNumber)
        {
            var model = new ReceiveSampleViewModel
            {
                DateTimeReceived = DateTime.Now
            };

            // ============================================
            // 1. GET LOGGED-IN TECHNICIAN
            // ============================================
            var technician = await _userManager.GetUserAsync(User);

            model.TechnicianName =
                technician?.FullName ?? "Unknown Technician";


            // ============================================
            // 2. VALIDATE REQUEST NUMBER
            // ============================================
            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                ModelState.AddModelError(
                    nameof(model.RequestNumber),
                    "Request number is required.");

                return PartialView("_ReceiveSampleModal", model);
            }


            // ============================================
            // 3. FIND TEST REQUEST
            // ============================================
            var request = await _context.TestRequests
                .AsNoTracking()
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r =>
                    r.RequestNumber == requestNumber);

            if (request == null)
            {
                ModelState.AddModelError(
                    nameof(model.RequestNumber),
                    "Test request was not found.");

                return PartialView("_ReceiveSampleModal", model);
            }


            // ============================================
            // 4. POPULATE REQUEST DETAILS
            // ============================================
            model.TestRequestId = request.TestRequestId;
            model.RequestNumber = request.RequestNumber;

            model.PatientName =
                request.Patient?.FullName ?? "N/A";

            model.DoctorName =
                request.Doctor?.FullName ?? "N/A";


            // ============================================
            // 5. FIND SAMPLE FOR THIS REQUEST
            // ============================================
            var sample = await _context.Samples
                .AsNoTracking()
                .Include(s => s.SampleType)
                .FirstOrDefaultAsync(s =>
                    s.TestRequestId == request.TestRequestId &&
                    !s.IsDeleted);

            if (sample == null)
            {
                ModelState.AddModelError(
                    "",
                    $"No sample is linked to request {request.RequestNumber}.");

                return PartialView("_ReceiveSampleModal", model);
            }


            // ============================================
            // 6. POPULATE SAMPLE DETAILS
            // ============================================
            model.SampleID = sample.SampleId;

            model.Barcode =
                sample.Barcode;

            model.SampleTypeId =
                sample.SampleTypeId;

            model.SampleTypeName =
                sample.SampleType?.Name ?? "Unknown Sample Type";


            // ============================================
            // 7. DATE/TIME SAMPLE IS BEING RECEIVED
            // ============================================
            model.DateTimeReceived = DateTime.Now;


            // ============================================
            // 8. RETURN MODAL
            // ============================================
            return PartialView("_ReceiveSampleModal", model);
        }

        // POST: Technician/ReceiveSamples/Receive
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Receive(
            ReceiveSampleViewModel model)
        {
            try
            {
                // ====================================================
                // 1. VERIFICATION CHECKLIST
                // ====================================================

                if (!model.VerifyRequestNumber ||
                    !model.VerifyTechnician ||
                    !model.VerifyBarcode ||
                    !model.VerifySampleType ||
                    !model.VerifyDateTime)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Verification Required",

                        message =
                            "Please verify all items before receiving the sample.",

                        icon =
                            "warning"
                    });
                }


                // ====================================================
                // 2. MODEL VALIDATION
                // ====================================================

                if (!ModelState.IsValid)
                {
                    var errors = ModelState

                        .Where(x =>
                            x.Value != null &&
                            x.Value.Errors.Count > 0)

                        .SelectMany(x =>
                            x.Value!.Errors)

                        .Select(x =>
                            x.ErrorMessage)

                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x))

                        .ToList();


                    return Json(new
                    {
                        success = false,

                        title =
                            "Validation Error",

                        message =
                            errors.Any()
                                ? string.Join("<br>", errors)
                                : "Please check the sample information and try again.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 3. GET REQUEST
                // ====================================================

                var request = await _context.TestRequests

                    .Include(r => r.Patient)

                    .Include(r => r.Doctor)

                    .Include(r => r.Samples)

                        .ThenInclude(s =>
                            s.SampleType)

                    .FirstOrDefaultAsync(r =>
                        r.RequestNumber ==
                        model.RequestNumber);


                if (request == null)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Request Not Found",

                        message =
                            "The test request could not be found.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 4. FIND SAMPLE
                // ====================================================

                var sample = request.Samples

                    .FirstOrDefault(s =>
                        s.SampleId ==
                        model.SampleID &&
                        !s.IsDeleted);


                if (sample == null)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Sample Not Found",

                        message =
                            "The selected sample could not be found.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 5. ALREADY RECEIVED?
                // ====================================================

                if (sample.IsReceived)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Already Verified",

                        message =
                            $"Sample {sample.Barcode} has already been received and verified.",

                        icon =
                            "info"
                    });
                }


                // ====================================================
                // 6. VERIFY BARCODE
                // ====================================================

                if (!string.Equals(
                    sample.Barcode?.Trim(),
                    model.Barcode?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Barcode Verification Failed",

                        message =
                            "The barcode does not match the sample registered for this request.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 7. VERIFY SAMPLE TYPE
                // ====================================================

                if (sample.SampleTypeId !=
                    model.SampleTypeId)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Sample Type Mismatch",

                        message =
                            "The sample type does not match the requested sample.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 8. GET LOGGED-IN TECHNICIAN
                // ====================================================

                var technician =
                    await _userManager.GetUserAsync(User);


                if (technician == null)
                {
                    return Json(new
                    {
                        success = false,

                        title =
                            "Technician Not Found",

                        message =
                            "Unable to identify the logged-in laboratory technician.",

                        icon =
                            "error"
                    });
                }


                // ====================================================
                // 9. UPDATE SAMPLE
                // ====================================================

                var receivedAt =
                    DateTime.Now;


                sample.TechnicianId =
                    technician.Id;


                sample.DateReceived =
                    receivedAt;


                sample.IsReceived =
                    true;


                sample.Status =
                    "Samples Received";


                // ====================================================
                // 10. UPDATE REQUEST
                // ====================================================

                request.Status =
                    TestRequestStatus
                        .SamplesReceived
                        .ToString();


                // ====================================================
                // 11. SAVE DATABASE
                // ====================================================

                await _context.SaveChangesAsync();


                // ====================================================
                // 12. SUCCESS JSON FOR SWEETALERT
                // ====================================================

                return Json(new
                {
                    success = true,

                    title =
                        "Sample Verified!",

                    message =
                        $"Sample {sample.Barcode} has been successfully verified and received.",

                    icon =
                        "success",

                    barcode =
                        sample.Barcode,

                    technician =
                        technician.FullName,

                    receivedAt =
                        receivedAt.ToString(
                            "dd MMM yyyy HH:mm")
                });
            }


            // ========================================================
            // DATABASE UPDATE ERROR
            // ========================================================

            catch (DbUpdateException ex)
            {
                var message =
                    ex.Message;


                if (ex.InnerException != null)
                {
                    message +=
                        "<br><br>" +
                        "<strong>Database Error:</strong><br>" +
                        ex.InnerException.Message;
                }


                return Json(new
                {
                    success = false,

                    title =
                        "Database Error",

                    message =
                        message,

                    icon =
                        "error"
                });
            }


            // ========================================================
            // GENERAL ERROR
            // ========================================================

            catch (Exception ex)
            {
                var message =
                    ex.Message;


                if (ex.InnerException != null)
                {
                    message +=
                        "<br><br>" +
                        "<strong>Inner Exception:</strong><br>" +
                        ex.InnerException.Message;
                }


                return Json(new
                {
                    success = false,

                    title =
                        "Unable to Receive Sample",

                    message =
                        message,

                    icon =
                        "error"
                });
            }
        }
    }
}
