using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.MedicalConditionCount = await _context.MedicalConditions.CountAsync(x => x.Status == Status.Active);
            ViewBag.AllergyCount = await _context.Allergies.CountAsync(x => x.Status == Status.Active);
            ViewBag.MedicationCount = await _context.Medications.CountAsync(x => x.Status == Status.Active);
            ViewBag.SampleTypeCount = await _context.SampleTypes.CountAsync(x => x.IsActive);
            ViewBag.ConditionCategoryCount = await _context.MedicalConditionCategories.CountAsync(x => x.Status == Status.Active);
            ViewBag.AllergyCategoryCount = await _context.AllergyCategories.CountAsync(x => x.Status == Status.Active);
            ViewBag.MedicationCategoryCount = await _context.MedicationCategories.CountAsync(x => x.Status == Status.Active);
            return View();
        }
    }
}