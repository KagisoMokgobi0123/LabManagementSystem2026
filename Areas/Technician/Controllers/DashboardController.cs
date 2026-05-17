using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
   
    
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;
        private readonly IHubContext<LabHub> _hubContext;
        public DashboardController(
        ApplicationDbContext dbcontext,
        UserManager<RegisterUser> userManager,
        IHubContext<LabHub> hubContext)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _hubContext = hubContext;
        }
       
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var model = new TechnicianDashboardViewModel
            {
                SelectedTests = await _dbcontext.TestRequestTests
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.TechnicianId == userId && t.Status == "InProgress")  
                    .ToListAsync(),

                PendingSelection = await _dbcontext.TestRequestTests
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.TechnicianId == null && t.Status == "Submitted")
                    .ToListAsync(),

                PendingVerification = await _dbcontext.TestRequestTests
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.Status == "Completed" && t.TechnicianId != userId)
                    .ToListAsync(),

                ReturnedTests = await _dbcontext.TestRequestTests  // Fix: Use consistent DbSet name
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.TechnicianId == userId && t.Status == "ToBeReviewed")  // Fix: Use TestStatus enum
                    .ToListAsync(),

                UrgentTests = await _dbcontext.TestRequestTests  // Fix: Use consistent DbSet name
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.TestRequest != null &&
                               t.TestRequest.Urgency == Urgency.Stat &&
                               t.Status != "Verified")
                    .ToListAsync(),

                OverdueTests = await _dbcontext.TestRequestTests  // Fix: Use consistent DbSet name
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.DueDate < DateTime.UtcNow &&
                               t.Status != "Verified" &&
                               t.Status != "Completed")
                    .ToListAsync(),

                NearDueTests = await _dbcontext.TestRequestTests  // Fix: Use consistent DbSet name
                    .Include(t => t.TestRequest)
                    .Include(t => t.TestType)
                    .Where(t => t.DueDate <= DateTime.UtcNow.AddHours(2) &&
                               t.DueDate > DateTime.UtcNow &&
                               t.Status != "Verified" &&
                               t.Status != "Completed")
                    .ToListAsync()
            };

            // Add counts
            model.TotalAssigned = model.SelectedTests.Count;
            model.TotalCompleted = await _dbcontext.TestRequestTests  
                .CountAsync(t => t.TechnicianId == userId && t.Status == "Completed");
            model.TotalVerified = await _dbcontext.TestRequestTests  
                .CountAsync(t => t.TechnicianId == userId && t.Status == "Verified");

            return View(model);
        }
    }
}

