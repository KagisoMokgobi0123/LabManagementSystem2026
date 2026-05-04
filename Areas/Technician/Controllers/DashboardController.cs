using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public static class Status
    {
        public const string Pending = "Pending";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string SamplesReceived = "Samples Received";
        public const string Verified = "Verified";
        public const string ReturnedForReview = "To be reviewed";

    }
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

                var model = new TechnicianDashboardViewModel
                {
                    SelectedTests = await _dbcontext.TestRequestTestTypes
                        .Where(t => t.TechnicianId == userId && t.Status == Status.InProgress)
                        .ToListAsync(),

                    PendingSelection = await _dbcontext.TestRequestTestTypes
                        .Where(t => t.TechnicianId == null && t.Status == Status.Pending)
                        .ToListAsync(),

                    PendingVerification = await _dbcontext.TestRequestTestTypes
                        .Include(t => t.TestRequest)
                        .Where(t => t.Status == Status.Completed)
                        .ToListAsync(),

                    ReturnedTests = await _dbcontext.TestRequestTestTypes
                        .Where(t => t.TechnicianId == userId && t.Status == Status.ReturnedForReview)
                        .ToListAsync(),

                    UrgentTests = await _dbcontext.TestRequestTestTypes
                        .Include(t => t.TestRequest)
                        .Where(t => t.TestRequest.Urgency == Urgency.STAT
                                    && t.Status != Status.Verified)
                        .ToListAsync(),

                    OverdueTests = await _dbcontext.TestRequestTestTypes
                        .Include(t => t.TestRequest)
                        .Where(t => t.DueDate < DateTime.UtcNow
                                    && t.Status != Status.Verified)
                        .ToListAsync(),

                    NearDueTests = await _dbcontext.TestRequestTestTypes
                        .Include(t => t.TestRequest)
                        .Where(t => t.DueDate <= DateTime.UtcNow.AddHours(2)
                                    && t.Status != Status.Verified)
                        .ToListAsync()
                };

                return View(model);
            }
        }
    }

