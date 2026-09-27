using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Data
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class ReceiveSamplesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReceiveSamplesController> _logger;

        public ReceiveSamplesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ReceiveSamplesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // ============================================================
        // INDEX
        // GET: /Technician/ReceiveSamples
        //
        // Displays submitted test requests whose samples still
        // need to be received by the laboratory.
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var requests = await _context.TestRequests
                .AsNoTracking()
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.TestTypes)
                    .ThenInclude(t => t.SampleType)
                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)
                .Where(r =>
                    r.Status == TestRequestStatus.Submitted.ToString())
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }


        // ============================================================
        // RECEIVED SAMPLES
        // GET: /Technician/ReceiveSamples/VerifiedSamples
        //
        // Displays samples that have already been received.
        // ============================================================

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> VerifiedSamples()
        {
            var requests = await _context.TestRequests
                .AsNoTracking()

                .Include(r => r.Patient)
                .Include(r => r.Doctor)

                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)

                .Include(r => r.Samples)
                    .ThenInclude(s => s.Technician)

                .Where(r =>
                    r.Samples.Any(s =>
                        s.IsReceived &&
                        !s.IsDeleted))

                .OrderByDescending(r => r.RequestDate)

                .ToListAsync();

            return View(requests);
        }

        // ============================================================
        // RECEIVE SAMPLE - GET
        // GET: /Technician/ReceiveSamples/Receive
        //
        // Loads the verification modal for the selected request.
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Receive(string? requestNumber)
        {
            var model = new ReceiveSampleViewModel
            {
                DateTimeReceived = DateTime.Now
            };


            // --------------------------------------------------------
            // Validate request number
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                ModelState.AddModelError(
                    nameof(model.RequestNumber),
                    "A request number is required.");

                return PartialView(
                    "_ReceiveSampleModal",
                    model);
            }

            requestNumber = requestNumber.Trim();


            // --------------------------------------------------------
            // Get logged-in technician
            // --------------------------------------------------------

            var technician =
                await _userManager.GetUserAsync(User);

            if (technician == null)
            {
                model.TechnicianName =
                    "Unknown Technician";

                ModelState.AddModelError(
                    "",
                    "Unable to identify the logged-in laboratory technician.");

                return PartialView(
                    "_ReceiveSampleModal",
                    model);
            }

            model.TechnicianName =
                technician.FullName;


            // --------------------------------------------------------
            // Load test request
            // --------------------------------------------------------

            var request = await _context.TestRequests
                .AsNoTracking()
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.TestTypes)
                    .ThenInclude(t => t.SampleType)
                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)
                .FirstOrDefaultAsync(r =>
                    r.RequestNumber == requestNumber);


            if (request == null)
            {
                model.RequestNumber =
                    requestNumber;

                ModelState.AddModelError(
                    nameof(model.RequestNumber),
                    $"Test request {requestNumber} was not found.");

                return PartialView(
                    "_ReceiveSampleModal",
                    model);
            }


            // --------------------------------------------------------
            // Populate request information
            // --------------------------------------------------------

            model.TestRequestId =
                request.TestRequestId;

            model.RequestNumber =
                request.RequestNumber;

            model.PatientName =
                request.Patient?.FullName ?? "N/A";

            model.DoctorName =
                request.Doctor?.FullName ?? "N/A";


            // --------------------------------------------------------
            // Find active sample linked to request
            // --------------------------------------------------------

            var sample = request.Samples
                .FirstOrDefault(s => !s.IsDeleted);

            if (sample != null)
            {
                model.SampleID = sample.SampleId;

                model.Barcode =
                    !string.IsNullOrWhiteSpace(sample.Barcode)
                        ? sample.Barcode.Trim()
                        : string.Empty;

                model.SampleTypeId = sample.SampleTypeId;

                model.SampleTypeName =
                    sample.SampleType?.Name ?? "N/A";

                if (sample.IsReceived)
                {
                    ModelState.AddModelError(
                        "",
                        $"Sample {model.Barcode} has already been received.");
                }
            }
            else
            {
                model.SampleID = 0;
                model.Barcode = string.Empty;
                model.SampleTypeId = 0;
                model.SampleTypeName = "N/A";

                ModelState.AddModelError(
                    "",
                    $"No active sample is linked to request " +
                    $"{request.RequestNumber}.");
            }


            // --------------------------------------------------------
            // Validate barcode
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.Barcode))
            {
                ModelState.AddModelError(
                    nameof(model.Barcode),
                    $"No barcode is available for request " +
                    $"{request.RequestNumber}.");
            }


            // --------------------------------------------------------
            // Validate sample type
            // --------------------------------------------------------

            if (model.SampleTypeId <= 0)
            {
                model.SampleTypeName =
                    "Sample type not configured";

                ModelState.AddModelError(
                    nameof(model.SampleTypeId),
                    "No sample type has been configured for this test.");
            }


            model.DateTimeReceived =
                DateTime.Now;


            return PartialView(
                "_ReceiveSampleModal",
                model);
        }


        // ============================================================
        // RECEIVE SAMPLE - POST
        // POST: /Technician/ReceiveSamples/Receive
        //
        // Verifies and receives the physical sample.
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(
            ReceiveSampleViewModel model)
        {
            try
            {
                // ----------------------------------------------------
                // 1. Verify the five required reception checks
                // ----------------------------------------------------

                if (!model.VerifyRequestNumber ||
                    !model.VerifyTechnician ||
                    !model.VerifyBarcode ||
                    !model.VerifySampleType ||
                    !model.VerifyDateTime)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Verification Incomplete",
                        message =
                            "Please complete all five verification " +
                            "checks before receiving the sample.",
                        icon = "warning"
                    });
                }


                // ----------------------------------------------------
                // 2. Validate posted request number
                // ----------------------------------------------------

                if (string.IsNullOrWhiteSpace(model.RequestNumber))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Request Number Missing",
                        message =
                            "The test request number was not submitted.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 3. Validate barcode
                // ----------------------------------------------------

                if (string.IsNullOrWhiteSpace(model.Barcode))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Barcode Missing",
                        message =
                            "The sample barcode was not submitted.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 4. Validate sample type
                // ----------------------------------------------------

                if (model.SampleTypeId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Type Missing",
                        message =
                            "The sample type was not submitted correctly.",
                        icon = "error"
                    });
                }


                var requestNumber =
                    model.RequestNumber.Trim();

                var submittedBarcode =
                    model.Barcode.Trim();


                // ----------------------------------------------------
                // 5. Load request
                // ----------------------------------------------------

                var request = await _context.TestRequests
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .Include(r => r.TestTypes)
                    .FirstOrDefaultAsync(r =>
                        r.RequestNumber == requestNumber);


                if (request == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Request Not Found",
                        message =
                            $"Test request {requestNumber} " +
                            "could not be found.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 6. Ensure request is still awaiting reception
                // ----------------------------------------------------

                if (!string.Equals(
                    request.Status,
                    TestRequestStatus.Submitted.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        success = false,
                        title = "Invalid Request Status",
                        message =
                            $"Request {request.RequestNumber} is no longer " +
                            "awaiting sample reception.",
                        icon = "warning"
                    });
                }


                // ----------------------------------------------------
                // 7. Find sample from database
                //
                // Do not trust SampleID from the browser.
                // Match using TestRequestId + Barcode.
                // ----------------------------------------------------

                var samples = await _context.Samples
                    .Include(s => s.SampleType)
                    .Where(s =>
                        s.TestRequestId == request.TestRequestId &&
                        !s.IsDeleted)
                    .ToListAsync();


                var sample = samples.FirstOrDefault(s =>
                    !string.IsNullOrWhiteSpace(s.Barcode) &&
                    string.Equals(
                        s.Barcode.Trim(),
                        submittedBarcode,
                        StringComparison.OrdinalIgnoreCase));


                if (sample == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Not Found",
                        message =
                            $"No active sample with barcode " +
                            $"{submittedBarcode} is linked to request " +
                            $"{request.RequestNumber}.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 8. Prevent duplicate reception
                // ----------------------------------------------------

                if (sample.IsReceived)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Already Received",
                        message =
                            $"Sample {sample.Barcode} has already been " +
                            "received into the laboratory.",
                        icon = "info"
                    });
                }


                // ----------------------------------------------------
                // 9. Verify barcode against database
                // ----------------------------------------------------

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
                            "The submitted barcode does not match the " +
                            "barcode registered for this sample.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 10. Verify sample type against database
                // ----------------------------------------------------

                if (sample.SampleTypeId != model.SampleTypeId)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Sample Type Mismatch",
                        message =
                            "The selected sample type does not match " +
                            "the sample type registered in the database.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 11. Get logged-in technician
                // ----------------------------------------------------

                var technician =
                    await _userManager.GetUserAsync(User);


                if (technician == null)
                {
                    return Json(new
                    {
                        success = false,
                        title = "Technician Not Found",
                        message =
                            "Unable to identify the logged-in " +
                            "laboratory technician.",
                        icon = "error"
                    });
                }


                // ----------------------------------------------------
                // 12. Receive sample
                // ----------------------------------------------------

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


                // ----------------------------------------------------
                // 13. Update request status
                // ----------------------------------------------------

                request.Status =
                    TestRequestStatus
                        .SamplesReceived
                        .ToString();


                // ----------------------------------------------------
                // 14. Save
                // ----------------------------------------------------

                await _context.SaveChangesAsync();


                _logger.LogInformation(
                    "Technician {TechnicianId} received sample {Barcode} " +
                    "for request {RequestNumber}.",
                    technician.Id,
                    sample.Barcode,
                    request.RequestNumber);


                // ----------------------------------------------------
                // 15. SweetAlert response
                // ----------------------------------------------------

                return Json(new
                {
                    success = true,

                    title = "Sample Received!",

                    message =
                        $"Sample <strong>{sample.Barcode}</strong> " +
                        $"has been successfully verified and received " +
                        $"into the laboratory.",

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
                        receivedAt.ToString(
                            "dd MMM yyyy HH:mm"),

                    status =
                        sample.Status
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while receiving sample.");

                return Json(new
                {
                    success = false,
                    title = "Database Error",
                    message =
                        "The sample could not be received because " +
                        "a database error occurred.",
                    icon = "error"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while receiving sample.");

                return Json(new
                {
                    success = false,
                    title = "Unable to Receive Sample",
                    message =
                        "An unexpected error occurred while " +
                        "receiving the sample.",
                    icon = "error"
                });
            }
        }
    }
}
