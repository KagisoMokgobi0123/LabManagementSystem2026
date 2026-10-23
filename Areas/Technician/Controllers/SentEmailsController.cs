using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "LabTechnician")]
    public class SentEmailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SentEmailsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public SentEmailsController(
            ApplicationDbContext context,
            ILogger<SentEmailsController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // A combined report belongs to a request, potentially involving several
        // technicians. Show it to a technician only if they were assigned an active
        // test in that request. This is not a sender-ownership audit field.
        private IQueryable<int> AccessibleRequestIds(string technicianId)
        {
            return _context.TestRequestTestTypes
                .AsNoTracking()
                .Where(t => t.TechnicianId == technicianId && !t.IsDeleted)
                .Select(t => t.TestRequestId)
                .Distinct();
        }

#if DEBUG
        [HttpGet]
        public IActionResult PrepareReport() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareReport(
            int testRequestId,
            [FromServices] LaboratoryReportDeliveryService reportDeliveryService)
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId)) return Challenge();

            if (testRequestId <= 0 ||
                !await AccessibleRequestIds(technicianId).ContainsAsync(testRequestId))
            {
                TempData["ErrorMessage"] = "This test request is not available to your account.";
                return RedirectToAction(nameof(PrepareReport));
            }

            try
            {
                bool prepared = await reportDeliveryService.PrepareReportAsync(testRequestId);
                TempData[prepared ? "SuccessMessage" : "ErrorMessage"] = prepared
                    ? "Laboratory PDF prepared for the pending email record. Check delivery status."
                    : "Report preparation was blocked. Check the request, results, recipient and pending email status.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF preparation failed for request {TestRequestId}", testRequestId);
                TempData["ErrorMessage"] = "PDF preparation failed. Check the application logs.";
            }

            return RedirectToAction(nameof(Index));
        }
#endif

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId)) return Challenge();

            var accessibleRequestIds = AccessibleRequestIds(technicianId);
            var emails = await _context.TechnicianSentEmails
                .AsNoTracking()
                .Where(e => accessibleRequestIds.Contains(e.TestRequestId))
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(emails);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            if (id <= 0) return BadRequest();
            var technicianId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(technicianId)) return Challenge();

            var accessibleRequestIds = AccessibleRequestIds(technicianId);
            var email = await _context.TechnicianSentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.SentEmailId == id &&
                                          accessibleRequestIds.Contains(e.TestRequestId));

            if (email?.PdfAttachment == null || email.PdfAttachment.Length == 0)
                return NotFound();

            var fileName = string.IsNullOrWhiteSpace(email.PdfFileName)
                ? $"LaboratoryReport_{email.TestRequestId}.pdf"
                : Path.GetFileName(email.PdfFileName);

            return File(email.PdfAttachment, "application/pdf", fileName);
        }
    }
}
