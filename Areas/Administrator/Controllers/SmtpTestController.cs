
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class SmtpTestController : Controller
    {
        private readonly LaboratoryEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SmtpTestController> _logger;

        public SmtpTestController(
            LaboratoryEmailService emailService,
            IWebHostEnvironment environment,
            ILogger<SmtpTestController> logger)
        {
            _emailService = emailService;
            _environment = environment;
            _logger = logger;
        }

        // =====================================================
        // GET: /Administrator/SmtpTest
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            return View();
        }

        // =====================================================
        // POST: /Administrator/SmtpTest/Send
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            string recipientEmail,
            CancellationToken cancellationToken)
        {
            // This endpoint is strictly for local development.
            if (!_environment.IsDevelopment())
                return NotFound();

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest(
                    "A test recipient email address is required.");
            }

            // Validate the recipient's email format.
            try
            {
                var address = new MailAddress(recipientEmail);

                if (address.Address != recipientEmail.Trim())
                {
                    return BadRequest(
                        "Please enter a valid recipient email address.");
                }
            }
            catch (FormatException)
            {
                return BadRequest(
                    "Please enter a valid recipient email address.");
            }

            try
            {
                // =================================================
                // 1. GENERATE A VALID DEMONSTRATION PDF
                // =================================================

                byte[] pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);

                        page.Header()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .FontSize(18)
                            .Bold();

                        page.Content().Column(column =>
                        {
                            column.Spacing(15);

                            column.Item()
                                .Text("Gmail SMTP Integration Test")
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text(
                                    "DEMONSTRATION DOCUMENT - " +
                                    "NOT A CLINICAL REPORT")
                                .FontSize(12)
                                .Bold();

                            column.Item()
                                .Text(
                                    "This document was generated " +
                                    "to verify the NMB-HLabSys " +
                                    "email attachment functionality.");

                            column.Item()
                                .Text(
                                    "No patient information, " +
                                    "laboratory measurements, or " +
                                    "clinical results are included.");

                            column.Item()
                                .Text(
                                    "Generated: " +
                                    DateTime.UtcNow.ToString(
                                        "yyyy-MM-dd HH:mm:ss") +
                                    " UTC");
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text(
                                "NMB-HLabSys | SMTP Test Only");
                    });
                }).GeneratePdf();

                if (pdfBytes.Length == 0)
                {
                    throw new InvalidOperationException(
                        "The demonstration PDF was empty.");
                }

                // =================================================
                // 2. SEND THE TEST EMAIL WITH PDF ATTACHMENT
                // =================================================

                await _emailService.SendReportAsync(
                    recipientEmail.Trim(),
                    "NMB-HLabSys - Gmail SMTP Test",
                    "Hello,\n\n" +
                    "This is an automated demonstration email " +
                    "from NMB Health Laboratory System.\n\n" +
                    "A demonstration PDF is attached.\n\n" +
                    "This is NOT a clinical laboratory report. " +
                    "No patient information is included.\n\n" +
                    "Regards,\n" +
                    "NMB Health Laboratory System",
                    "NMB-HLabSys-SMTP-Test.pdf",
                    pdfBytes,
                    cancellationToken);

                // =================================================
                // 3. CONFIRM SMTP ACCEPTANCE
                // =================================================

                _logger.LogInformation(
                    "SMTP demonstration email accepted for {Recipient}.",
                    recipientEmail.Trim());

                return Ok(
                    "SUCCESS: Gmail SMTP accepted the test email. " +
                    "Please check the recipient inbox and spam folder " +
                    "for the demonstration PDF.");
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                return StatusCode(
                    499,
                    "The SMTP test request was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP demonstration email failed.");

                return StatusCode(
                    500,
                    "SMTP TEST FAILED. Check the Visual Studio " +
                    "Output window or application logs for details. " +
                    "No laboratory records were changed.");
            }
        }
    }
}
