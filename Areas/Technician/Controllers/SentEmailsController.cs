
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class SentEmailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SentEmailsController(ApplicationDbContext context)
        {
            _context = context;
        }

#if DEBUG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareReport(
            int testRequestId,
            [FromServices]
    NMB_HLabSys.Services.LaboratoryReportDeliveryService
                reportDeliveryService)
        {
            if (testRequestId <= 0)
                return BadRequest();

            // Development testing only.
            // This action does not send an email.

            try
            {
                var prepared =
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
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "PDF preparation failed. Check the application logs.";
            }

            return RedirectToAction(nameof(Index));
        }
#endif

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var emails = await _context.TechnicianSentEmails
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(emails);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var email = await _context.TechnicianSentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.SentEmailId == id);

            if (email == null || email.PdfAttachment == null ||
                email.PdfAttachment.Length == 0)
            {
                return NotFound();
            }

            return File(
                email.PdfAttachment,
                "application/pdf",
                email.PdfFileName);
        }
    }
}
