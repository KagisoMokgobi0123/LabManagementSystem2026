using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Data;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Services;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    public class TestRequestController : Controller
    {
        private readonly UserManager<RegisterUser> _userManager;
        private readonly SignInManager<RegisterUser> _signInManager;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IEmailService _emailService;

        public TestRequestController(
            UserManager<RegisterUser> userManager,
            SignInManager<RegisterUser> signInManager,
            ApplicationDbContext dbcontext,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbcontext = dbcontext;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult CreateTestRequest()
        {
            var model = new TestRequestsModel
            {
                RequestDate = DateTime.Now,
                TestTypes = new List<TestRequestTestTypeModel>(),
                Samples = new List<SampleModel>()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestRequest(TestRequestsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ✅ Validation rules
            if (model.TestTypes == null || !model.TestTypes.Any())
            {
                ModelState.AddModelError("", "At least one test type is required.");
                return View(model);
            }

            if (model.Samples == null || !model.Samples.Any())
            {
                ModelState.AddModelError("", "At least one sample is required.");
                return View(model);
            }

            // ✅ Load patient
            var patient = await _userManager.FindByIdAsync(model.PatientId);
            if (patient == null)
            {
                ModelState.AddModelError("", "Patient not found.");
                return View(model);
            }

            model.Patient = patient;

            // ❗ Always set system values
            model.RequestDate = DateTime.Now;
            model.Status = "Submitted";

            // 1️⃣ Save Test Request first
            _dbcontext.TestRequests.Add(model);
            await _dbcontext.SaveChangesAsync();

            // 2️⃣ Update Test Types
            foreach (var test in model.TestTypes)
            {
                test.TestRequestId = model.TestRequestId;
                test.Status = "Submitted";
            }

            // 3️⃣ Update Samples
            foreach (var sample in model.Samples)
            {
                sample.TestRequestId = model.TestRequestId;
                sample.Status = "Submitted";
                sample.DateReceived = DateTime.Now;
            }
            model.RequestNumber = $"NMB-REQ-{model.TestRequestId:D6}";
            await _dbcontext.SaveChangesAsync();

            // 📧 Email ONLY patient
            await _emailService.SendEmailAsync(
                patient.Email,
                "Test Request Submitted",
                $"Dear {patient.UserName}, your test request {model.RequestNumber} has been submitted."
            );

            return RedirectToAction("Success");
        }
    }
}
