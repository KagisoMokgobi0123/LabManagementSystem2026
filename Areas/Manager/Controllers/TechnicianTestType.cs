//using NMB_HLabSys.Data;
//using NMB_HLabSys.Models.Identity;
//using NMB_HLabSys.Models.Laboratory;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace NMB_HLabSys.Areas.Manager.Controllers
//{
//    [Area("Manager")]
//    [Authorize]
//    public class TechnicianTestTypeController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;


//        public TechnicianTestTypeController(
//            ApplicationDbContext context,
//            UserManager<ApplicationUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }



//        // GET: Manager/TechnicianTestType
//        public async Task<IActionResult> Index()
//        {
//            var assignments =
//                await _context.TechnicianTestTypes
//                .Include(x => x.Technician)
//                .Include(x => x.TestType)
//                .OrderByDescending(x => x.DateAssigned)
//                .ToListAsync();


//            return View(assignments);
//        }



//        // GET: Assign test type to technician
//        [HttpGet]
//        public async Task<IActionResult> Create()
//        {
//            var technicians =
//                await _userManager.GetUsersInRoleAsync("Technician");


//            ViewBag.Technicians = technicians;


//            ViewBag.TestTypes =
//                await _context.TestTypes
//                .Where(x => x.Status == NMB_HLabSys.Models.Laboratory.Enums.TestTypeStatus.Active)
//                .OrderBy(x => x.Name)
//                .ToListAsync();


//            return View();
//        }




//        // POST: Assign test type
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(
//            string technicianId,
//            int testTypeId)
//        {

//            if (string.IsNullOrEmpty(technicianId))
//            {
//                ModelState.AddModelError(
//                    "",
//                    "Please select a technician.");
//            }


//            if (!ModelState.IsValid)
//            {
//                return RedirectToAction(nameof(Create));
//            }



//            var exists =
//                await _context.TechnicianTestTypes
//                .AnyAsync(x =>
//                    x.TechnicianId == technicianId &&
//                    x.TestTypeId == testTypeId);



//            if (exists)
//            {
//                TempData["Error"] =
//                    "This test type is already assigned to this technician.";

//                return RedirectToAction(nameof(Index));
//            }




//            var assignment = new TechnicianTestType
//            {
//                TechnicianId = technicianId,

//                TestTypeId = testTypeId,

//                DateAssigned = DateTime.UtcNow
//            };



//            await _context.TechnicianTestTypes.AddAsync(assignment);


//            await _context.SaveChangesAsync();



//            TempData["Success"] =
//                "Test type assigned successfully.";


//            return RedirectToAction(nameof(Index));
//        }




//        // Remove assignment
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(
//            int id)
//        {
//            var assignment =
//                await _context.TechnicianTestTypes
//                .FirstOrDefaultAsync(x =>
//                    x.TechnicianTestTypeId == id);



//            if (assignment == null)
//            {
//                TempData["Error"] =
//                    "Assignment not found.";

//                return RedirectToAction(nameof(Index));
//            }



//            _context.TechnicianTestTypes
//                .Remove(assignment);


//            await _context.SaveChangesAsync();



//            TempData["Success"] =
//                "Test type removed from technician.";


//            return RedirectToAction(nameof(Index));
//        }
//    }
//}