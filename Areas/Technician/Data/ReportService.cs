using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Technician.Data
{
    public class ReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        public ReportService(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task GeneratePdfAndSendEmail(int requestId)
        {
            var request = await _db.TestRequests
                .Include(r => r.Patient)
                .Include(r => r.TestTypes)
                .FirstOrDefaultAsync(r => r.TestRequestId == requestId);

            if (request == null) return;

            // 1. Generate PDF
            var pdfBytes = GeneratePdf(request);

            // 2. Send Email WITH attachment
            await _emailService.SendEmailWithAttachmentAsync(
                request.Doctor.Email,
                "Lab Results Ready",
                "Please find attached the lab results PDF.",
                pdfBytes,
                "LabResults.pdf"
            );
        }
        private byte[] GeneratePdf(TestRequestsModel request)
        {
            // placeholder - we’ll implement next step
            return new byte[0];
        }
    }
}
