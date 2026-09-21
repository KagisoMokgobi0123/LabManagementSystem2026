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
        public async Task<IActionResult> Receive(ReceiveSampleViewModel model)
        {
            try
            {
                // ============================================================
                // 1. VALIDATE REQUIRED POSTED VALUES
                // ============================================================

                if (string.IsNullOrWhiteSpace(model.RequestNumber))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Request Number Missing",
                        message = "The request number was not submitted.",
                        icon = "error"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.Barcode))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Barcode Missing",
                        message = "The sample barcode was not submitted.",
                        icon = "error"
                    });
                }

                if (model.SampleTypeId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Type Missing",
                        message = "The sample type was not submitted correctly.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 2. LOAD TEST REQUEST
                // ============================================================

                var request = await _context.TestRequests
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .FirstOrDefaultAsync(r =>
                        r.RequestNumber == model.RequestNumber);

                if (request == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Request Not Found",
                        message =
                            $"Test request {model.RequestNumber} could not be found.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 3. NORMALIZE SUBMITTED BARCODE
                // ============================================================

                var submittedBarcode =
                    model.Barcode.Trim();


                // ============================================================
                // 4. FIND SAMPLE DIRECTLY FROM DATABASE
                //
                // Do NOT depend on the posted SampleID.
                // The physical sample is identified using:
                //
                //      TestRequestId + Barcode
                //
                // ============================================================

                var sample = await _context.Samples
                    .Include(s => s.SampleType)
                    .FirstOrDefaultAsync(s =>
                        s.TestRequestId == request.TestRequestId &&
                        !s.IsDeleted &&
                        s.Barcode == submittedBarcode);


                // ============================================================
                // 5. FALLBACK CASE-INSENSITIVE BARCODE LOOKUP
                // ============================================================

                if (sample == null)
                {
                    var requestSamples = await _context.Samples
                        .Include(s => s.SampleType)
                        .Where(s =>
                            s.TestRequestId == request.TestRequestId &&
                            !s.IsDeleted)
                        .ToListAsync();

                    sample = requestSamples
                        .FirstOrDefault(s =>
                            !string.IsNullOrWhiteSpace(s.Barcode) &&
                            string.Equals(
                                s.Barcode.Trim(),
                                submittedBarcode,
                                StringComparison.OrdinalIgnoreCase));
                }


                if (sample == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Not Found",
                        message =
                            $"No active sample with barcode {submittedBarcode} " +
                            $"is linked to request {request.RequestNumber}.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 6. PREVENT DUPLICATE RECEPTION
                // ============================================================

                if (sample.IsReceived)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Already Received",
                        message =
                            $"Sample {sample.Barcode} has already been received into the laboratory.",
                        icon = "info"
                    });
                }


                // ============================================================
                // 7. VERIFY DATABASE BARCODE
                // ============================================================

                if (!string.Equals(
                    sample.Barcode?.Trim(),
                    submittedBarcode,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Barcode Verification Failed",
                        message =
                            "The submitted barcode does not match the barcode stored in the database.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 8. VERIFY SAMPLE TYPE
                // ============================================================

                if (sample.SampleTypeId != model.SampleTypeId)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Type Mismatch",
                        message =
                            $"The submitted sample type does not match the " +
                            $"sample type registered for {sample.Barcode}.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 9. GET LOGGED-IN TECHNICIAN
                // ============================================================

                var technician =
                    await _userManager.GetUserAsync(User);

                if (technician == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Technician Not Found",
                        message =
                            "Unable to identify the logged-in laboratory technician.",
                        icon = "error"
                    });
                }


                // ============================================================
                // 10. RECEIVE SAMPLE
                // ============================================================

                var receivedAt = DateTime.Now;

                sample.TechnicianId =
                    technician.Id;

                sample.DateReceived =
                    receivedAt;

                sample.IsReceived =
                    true;

                sample.Status =
                    "Samples Received";


                // ============================================================
                // 11. UPDATE REQUEST STATUS
                // ============================================================

                request.Status =
                    TestRequestStatus
                        .SamplesReceived
                        .ToString();


                // ============================================================
                // 12. SAVE DATABASE CHANGES
                // ============================================================

                await _context.SaveChangesAsync();


                // ============================================================
                // 13. RETURN SWEETALERT SUCCESS RESPONSE
                // ============================================================

                return Json(new
                {
                    success = true,

                    title = "Sample Received!",

                    message =
                        $"Sample {sample.Barcode} has been successfully " +
                        $"verified and received into the laboratory.",

                    icon = "success",

                    requestNumber =
                        request.RequestNumber,

                    barcode =
                        sample.Barcode,

                    sampleType =
                        sample.SampleType?.Name ?? "N/A",

                    technician =
                        technician.FullName,

                    receivedAt =
                        receivedAt.ToString("dd MMM yyyy HH:mm"),

                    status =
                        sample.Status
                });
            }


            // ================================================================
            // DATABASE ERROR
            // ================================================================

            catch (DbUpdateException ex)
            {
                var message =
                    ex.InnerException?.Message ??
                    ex.Message;

                return Json(new
                {
                    success = false,
                    title = "Database Error",
                    message =
                        "The sample could not be saved to the database.<br><br>" +
                        message,
                    icon = "error"
                });
            }


            // ================================================================
            // GENERAL ERROR
            // ================================================================

            catch (Exception ex)
            {
                var message =
                    ex.InnerException?.Message ??
                    ex.Message;

                return Json(new
                {
                    success = false,
                    title = "Unable to Receive Sample",
                    message = message,
                    icon = "error"
                });
            }
        }
    }
}
