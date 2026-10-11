using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    //Receive Sample controller handles the receiving of samples for test requests.
    //It allows lab technicians to view pending samples, receive them, and view
    //verified/received samples.
    public class ReceiveSamplesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReceiveSamplesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // =========================================================
        // INDEX
        // Display requests that still have samples to be received
        // GET: /Technician/ReceiveSamples/Index
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var requests = await _context.TestRequests
                .AsNoTracking()

                .Include(r => r.Patient)
                .Include(r => r.Doctor)

                .Include(r => r.Samples)
                    .ThenInclude(s => s.SampleType)

                .Where(r =>
                    r.Status == TestRequestStatus.Submitted.ToString() &&
                    r.Samples.Any(s =>
                        !s.IsDeleted &&
                        !s.IsReceived))

                .OrderByDescending(r => r.Urgency)
                .ThenBy(r => r.RequestDate)

                .ToListAsync();

            return View(requests);
        }
            // ============================================================
             // GET - LOAD RECEIVE MODAL
             // ============================================================
            [HttpGet]
            public async Task<IActionResult> Receive(
                int testRequestId,
                int sampleId)
            {
                var currentUser =
                    await _userManager.GetUserAsync(User);

                if (currentUser == null)
                    return Unauthorized();


                var sample =
                    await _context.Samples
                        .Include(s => s.SampleType)
                        .Include(s => s.TestRequest)
                            .ThenInclude(r => r.Patient)
                        .Include(s => s.TestRequest)
                            .ThenInclude(r => r.Doctor)
                        .FirstOrDefaultAsync(s =>
                            s.SampleId == sampleId &&
                            s.TestRequestId == testRequestId &&
                            !s.IsDeleted);


                if (sample == null)
                    return NotFound();


                if (sample.IsReceived)
                    return BadRequest(
                        "This sample has already been received."
                    );


                var request =
                    sample.TestRequest;

                if (request == null)
                    return NotFound();


                var model =
                    new ReceiveSampleViewModel
                    {
                        TestRequestId =
                            request.TestRequestId,

                        SampleID =
                            sample.SampleId,

                        RequestNumber =
                            request.RequestNumber,

                        Barcode =
                            sample.Barcode,

                        SampleTypeId =
                            sample.SampleTypeId,

                        SampleTypeName =
                            sample.SampleType?.Name ??
                            "N/A",

                        PatientName =
                            request.Patient?.FullName ??
                            "N/A",

                        DoctorName =
                            request.Doctor?.FullName ??
                            "N/A",

                        TechnicianName =
                            currentUser.FullName ??
                            currentUser.Email ??
                            "Lab Technician",

                        DateTimeReceived =
                            DateTime.Now
                    };


                return PartialView(
                    "_ReceiveSampleModal",
                    model
                );
            }


            // ============================================================
            // POST - RECEIVE SAMPLE
            // ============================================================

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Receive(
                ReceiveSampleViewModel model)
            {
                try
                {
                    var currentUser =
                        await _userManager.GetUserAsync(User);


                    if (currentUser == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message =
                                "Your login session could not be identified."
                        });
                    }


                    // ========================================================
                    // CHECKLIST
                    // ========================================================

                    if (!model.VerifyRequestNumber ||
                        !model.VerifyTechnician ||
                        !model.VerifyBarcode ||
                        !model.VerifySampleType ||
                        !model.VerifyDateTime)
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                "Please verify all five sample details before receiving the sample."
                        });
                    }


                    // ========================================================
                    // IDS
                    // ========================================================

                    if (model.TestRequestId <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message =
                                "TestRequestId is missing or invalid."
                        });
                    }


                    if (model.SampleID <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message =
                                "SampleID is missing or invalid."
                        });
                    }


                    // ========================================================
                    // SAMPLE
                    // ========================================================

                    var sample =
                        await _context.Samples
                            .Include(s => s.SampleType)
                            .Include(s => s.TestRequest)
                            .FirstOrDefaultAsync(s =>
                                s.SampleId ==
                                    model.SampleID &&

                                s.TestRequestId ==
                                    model.TestRequestId &&

                                !s.IsDeleted);


                    if (sample == null)
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                $"Sample could not be found. " +
                                $"SampleID: {model.SampleID}, " +
                                $"TestRequestId: {model.TestRequestId}."
                        });
                    }


                    // ========================================================
                    // ALREADY RECEIVED
                    // ========================================================

                    if (sample.IsReceived)
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                $"Sample {sample.Barcode} has already been received."
                        });
                    }


                    // ========================================================
                    // BARCODE
                    // ========================================================

                    if (!string.Equals(
                            sample.Barcode,
                            model.Barcode,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                $"Barcode mismatch. Expected " +
                                $"'{sample.Barcode}' but received " +
                                $"'{model.Barcode}'."
                        });
                    }


                    // ========================================================
                    // SAMPLE TYPE
                    // ========================================================

                    if (
                        sample.SampleTypeId !=
                        model.SampleTypeId
                    )
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                "The selected sample type does not match the database record."
                        });
                    }


                    var request =
                        sample.TestRequest;


                    if (request == null)
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                "The test request associated with this sample could not be found."
                        });
                    }


                    // ========================================================
                    // RECEIVE SAMPLE
                    // ========================================================

                    var receivedAt =
                        DateTime.Now;


                    sample.TechnicianId =
                        currentUser.Id;

                    sample.DateReceived =
                        receivedAt;

                    sample.IsReceived =
                        true;

                    sample.Status =
                        "SamplesReceived";


                    // ========================================================
                    // CHECK OTHER SAMPLES
                    // ========================================================

                    var outstandingSamples =
                        await _context.Samples
                            .AnyAsync(s =>
                                s.TestRequestId ==
                                    request.TestRequestId &&

                                s.SampleId !=
                                    sample.SampleId &&

                                !s.IsDeleted &&

                                !s.IsReceived);


                    // ========================================================
                    // REQUEST STATUS
                    // ========================================================

                    if (!outstandingSamples)
                    {
                        request.Status =
                            TestRequestStatus
                                .SamplesReceived
                                .ToString();
                    }


                    // ========================================================
                    // SAVE
                    // ========================================================

                    await _context.SaveChangesAsync();


                    // ========================================================
                    // SUCCESS
                    // ========================================================

                    return Json(new
                    {
                        success = true,

                        message =
                            $"Sample {sample.Barcode} was received successfully.",

                        requestNumber =
                            request.RequestNumber,

                        barcode =
                            sample.Barcode,

                        technician =
                            currentUser.FullName ??
                            currentUser.Email ??
                            "Lab Technician",

                        receivedAt =
                            receivedAt.ToString(
                                "dd MMM yyyy HH:mm"
                            )
                    });
                }

                catch (DbUpdateException ex)
                {
                    var message =
                        ex.InnerException?.Message ??
                        ex.Message;


                    Console.WriteLine(
                        "RECEIVE SAMPLE DATABASE ERROR:"
                    );

                    Console.WriteLine(
                        ex.ToString()
                    );


                    return Json(new
                    {
                        success = false,

                        message =
                            "Database error: " +
                            message
                    });
                }

                catch (Exception ex)
                {
                    Console.WriteLine(
                        "RECEIVE SAMPLE ERROR:"
                    );

                    Console.WriteLine(
                        ex.ToString()
                    );


                    return Json(new
                    {
                        success = false,

                        message =
                            "Unable to receive the sample: " +
                            ex.Message
                    });
                }
            }
            // =========================================================
            // VERIFIED / RECEIVED SAMPLES
            // GET: /Technician/ReceiveSamples/VerifiedSamples
            // =========================================================

            [HttpGet]
        public async Task<IActionResult> VerifiedSamples()
        {
            var samples = await _context.Samples
                .AsNoTracking()

                .Include(s => s.SampleType)

                .Include(s => s.Technician)

                .Include(s => s.TestRequest)
                    .ThenInclude(r => r!.Patient)

                .Include(s => s.TestRequest)
                    .ThenInclude(r => r!.Doctor)

                .Where(s =>
                    !s.IsDeleted &&
                    s.IsReceived)

                .OrderByDescending(s => s.DateReceived)

                .ToListAsync();


            return View(samples);
        }


    }
}