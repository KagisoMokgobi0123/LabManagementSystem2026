using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TechnicianTestTypeController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<RegisterUser> _userManager;

        public TechnicianTestTypeController(
            ApplicationDbContext dbcontext,
            UserManager<RegisterUser> userManager)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
        }
        //GET: Technician List
        public async Task<IActionResult> Index()
        {
            // Get users in "Technician" role
            var technicians = await _userManager.GetUsersInRoleAsync("Technician");

            // Load their test types
            var technicianIds = technicians.Select(t => t.Id).ToList();

            var techniciansWithTests = await _dbcontext.Users
                .Where(u => technicianIds.Contains(u.Id))
                .Include(u => u.TechnicianTestTypes)
                    .ThenInclude(tt => tt.TestType)
                .ToListAsync();

            return View(techniciansWithTests);
        }

        // GET: AssignTestTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.TestTypes = _dbcontext.TestTypes.ToList();
            return View();
        }
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userId, List<int> selectedTestTypes)
        {   
            // 🔍 Find existing user
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();
            // 🔴 Rule: At least one test type
            if (selectedTestTypes == null || !selectedTestTypes.Any())
            {
                ModelState.AddModelError("", "At least one test type must be selected.");
            }
            if (ModelState.IsValid)
            {
                // Update technician-specific fields
                user.MustChangePassword = true;

                // Remove existing test type assignments
                var existing = _dbcontext.TechnicianTestTypes
               .Where(tt => tt.TechnicianId == user.Id)
               .ToList();

                _dbcontext.TechnicianTestTypes.RemoveRange(existing);

                //Assign new test types
                var testTypes = selectedTestTypes ?? new List<int>();

                foreach (var testTypeId in testTypes)
                {
                    bool exists = _dbcontext.TechnicianTestTypes
                        .Any(tt => tt.TechnicianId == user.Id &&
                                   tt.TestTypeId == testTypeId);

                    if (exists)
                    {
                        TempData["Error"] = "One or more selected test types are already assigned to this technician.";
                        return RedirectToAction("Edit", new { id = user.Id });
                    }

                    _dbcontext.TechnicianTestTypes.Add(new TechnicianTestTypeModel
                    {
                        TechnicianId = user.Id,
                        TestTypeId = testTypeId
                    });
                }
                await _dbcontext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.TestTypes = _dbcontext.TestTypes.ToList();
            return View(user);
        } 
        // GET: AssignTestTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AssignTestTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AssignTestTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssignTestTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
