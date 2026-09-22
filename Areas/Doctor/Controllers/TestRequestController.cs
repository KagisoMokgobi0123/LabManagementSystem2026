using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Technician.Data;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Services.Interfaces;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    public class TestRequestController : Controller
    {
        private readonly UserManager<RegisterViewModel> _userManager;
        private readonly SignInManager<RegisterViewModel> _signInManager;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IEmailService _emailService;

        public TestRequestController(
            UserManager<RegisterViewModel> userManager,
            SignInManager<RegisterViewModel> signInManager,
            ApplicationDbContext dbcontext,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbcontext = dbcontext;
            _emailService = emailService;
        }
        //[HttpGet]
        //public IActionResult CreateTestRequest()
        //{
        //    var model = new TestRequestsModel
        //    {
        //        RequestDate = DateTime.Now,
        //        TestTypes = new List<TestRequestTestTypeModel>(),
        //        Samples = new List<SampleModel>()
        //    };

        //    return View(model);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateTestRequest(TestRequestsModel model)
        //{
        //    // VALIDATION
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    if (model.TestTypes == null || !model.TestTypes.Any())
        //    {
        //        ModelState.AddModelError("", "At least one test type is required.");
        //        return View(model);
        //    }

        //    // STEP 1 — SAVE TEST REQUEST
        //    model.RequestDate = DateTime.Now;
        //    model.Status = "Submitted";

        //    _dbcontext.TestRequests.Add(model);
        //    await _dbcontext.SaveChangesAsync();

        //    // STEP 2 — GENERATE REQUEST NUMBER
        //    model.RequestNumber = $"NMB-REQ-{model.TestRequestId:D6}";

        //    await _dbcontext.SaveChangesAsync();

        //    // STEP 3 — SAVE TEST REQUEST TEST TYPES
        //    foreach (var test in model.TestTypes)
        //    {
        //        test.TestRequestId = model.TestRequestId;
        //        test.Status = "Submitted";

        //        _dbcontext.TestRequestTestTypes.Add(test);
        //    }

        //    await _dbcontext.SaveChangesAsync();

        //    // STEP 4 — DETERMINE REQUIRED SAMPLE TYPES
        //    var sampleTypeIds = await _dbcontext.TestTypes
        //        .Where(t => model.TestTypes.Select(x => x.TestTypeId)
        //        .Contains(t.TestTypeId))
        //        .Select(t => t.SampleTypeId)
        //        .Distinct()
        //        .ToListAsync();

        //    // STEP 5 — CREATE SAMPLES
        //    foreach (var sampleTypeId in sampleTypeIds)
        //    {
        //        var sample = new SampleModel
        //        {
        //            TestRequestId = model.TestRequestId,
        //            SampleTypeId = sampleTypeId,
        //            Status = "Submitted",
        //            DateReceived = DateTime.Now,
        //            IsReceived = false
        //        };

        //        _dbcontext.Samples.Add(sample);
        //    }

        //    await _dbcontext.SaveChangesAsync();

        //    // STEP 6 — GENERATE SAMPLE BARCODES
        //    var samples = await _dbcontext.Samples
        //        .Where(s => s.TestRequestId == model.TestRequestId)
        //        .ToListAsync();

        //    foreach (var sample in samples)
        //    {
        //        sample.Barcode = $"NMB-SMP-{sample.SampleId:D6}";
        //    }

        //    await _dbcontext.SaveChangesAsync();

        //    return RedirectToAction("Success");
        //}

    }
}
