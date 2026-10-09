using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using static NMB_HLabSys.Areas.Manager.Models.LabManagerDashboardViewModel;

namespace NMBHLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null) { return Challenge(); }

            var model = new LabManagerDashboardViewModel
            { //Manager profile

                ManagerName = currentUser.FullName ?? "Laboratory Manager",

                ManagerEmail = currentUser.Email ?? string.Empty,

                // Active test types
                TestTypeCount = await _context.TestTypes.AsNoTracking().CountAsync(t => t.IsActive == TestTypeModel.RecordStatus.Active),

                // Registered staff
                TechnicianCount = await _context.TechnicianProfiles.AsNoTracking().CountAsync(),

                DoctorCount = await _context.DoctorProfiles.AsNoTracking().CountAsync(),
                ConsumableCount = await _context.Consumables.AsNoTracking().CountAsync(c => !c.IsDeleted),

                SupplierCount = await _context.Suppliers.AsNoTracking().CountAsync(s => !s.IsDeleted),

                LowStockCount = await _context.Consumables.AsNoTracking().CountAsync(c => !c.IsDeleted && c.QuantityOnHand <= c.ReorderLevel),

                OpenOrderCount = await _context.Orders.AsNoTracking().CountAsync(o => o.Status == OrderStatus.Ordered || o.Status == OrderStatus.PartiallyComplete)
            };

            model.TestCategorySummaries = await _context.TestTypes.AsNoTracking().Where(t => t.IsActive == TestTypeModel.RecordStatus.Active && t.TestCategory != null).GroupBy(t => t.TestCategory!.Description).Select(g => new TestCategorySummaryViewModel { CategoryName = g.Key, TestCount = g.Count() }).OrderByDescending(g => g.TestCount).ToListAsync();
            var stockItems = await _context.Consumables.AsNoTracking().Where(c => !c.IsDeleted).Include(c => c.Supplier).OrderBy(c => c.QuantityOnHand).ThenBy(c => c.Name).Take(10).ToListAsync();

            model.Consumables = stockItems.Select(c => new ConsumableStockViewModel
            {
                ConsumableId = c.ConsumableId,

                ConsumableName = c.Name,

                SupplierName = c.Supplier != null ? c.Supplier.SupplierName : "Not assigned",

                Quantity = c.QuantityOnHand,

                ReorderLevel = c.ReorderLevel,

                Status = c.QuantityOnHand <= 0 ? "Out of Stock" : c.QuantityOnHand <= c.ReorderLevel ? "Low Stock" : c.QuantityOnHand <= c.ReorderLevel * 1.25 ? "Reorder Soon" : "Sufficient"
            }).ToList();

            return View(model);
        }
    }
}