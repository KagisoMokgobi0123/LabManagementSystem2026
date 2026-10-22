
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class SentEmailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SentEmailsController> _logger;

        public SentEmailsController(
            ApplicationDbContext context,
            ILogger<SentEmailsController> logger)
        {
            _context = context;
            _logger = logger;
        }

#if DEBUG
        // GET: Technician/SentEmails/PrepareReport
        [HttpGet]
        public IActionResult PrepareReport()
        {
            return View();
        }

        // POST: Technician/SentEmails/PrepareReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareReport(
            int testRequestId,
            [FromServices]
            LaboratoryReportDeliveryService reportDeliveryService)
        {
            if (testRequestId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Please enter a valid Test Request ID.";

                return RedirectToAction(nameof(PrepareReport));
            }

            try
            {
                bool prepared =
                    await reportDeliveryService.PrepareReportAsync(
                        testRequestId);

                if (prepared)
                {
                    TempData["SuccessMessage"] =
                        "Verified laboratory PDF generated and saved " +
                        "to the outgoing email record. " +
                        "No email has been sent.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Report preparation was blocked. " +
                        "Confirm that the request is completed, " +
                        "all results are independently verified, " +
                        "the doctor has an email address, " +
                        "and a Pending email record exists.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "PDF preparation failed for request {TestRequestId}",
                    testRequestId);

                TempData["ErrorMessage"] =
                    "PDF preparation failed. Check the application logs.";
            }

            return RedirectToAction(nameof(Index));
        }
#endif

        // GET: Technician/SentEmails
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var emails = await _context.TechnicianSentEmails
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(emails);
        }

        // GET: Technician/SentEmails/DownloadPdf/5
        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            if (id <= 0)
                return BadRequest();

            var email = await _context.TechnicianSentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.SentEmailId == id);

            if (email == null ||
                email.PdfAttachment == null ||
                email.PdfAttachment.Length == 0)
            {
                return NotFound();
            }

            var fileName =
                string.IsNullOrWhiteSpace(email.PdfFileName)
                    ? $"LaboratoryReport_{email.TestRequestId}.pdf"
                    : email.PdfFileName;

            return File(
                email.PdfAttachment,
                "application/pdf",
                fileName);
        }
    }
}
