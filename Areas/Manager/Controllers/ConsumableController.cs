using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    public class ConsumableController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;

        public ConsumableController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var consumables = await _dbcontext.Consumables.ToListAsync();
            return View(consumables);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumableModel consumable)
        {
            // 🔴 Validation: Name must be unique
            if (_dbcontext.Consumables.Any(c => c.Name == consumable.Name))
            {
                ModelState.AddModelError("Name", "Consumable name must be unique.");
            }

            // 🔴 Validation: Values must not be negative
            if (consumable.ReorderLevel < 0)
            {
                ModelState.AddModelError("ReorderLevel", "Reorder level cannot be negative.");
            }

            if (consumable.QuantityOnHand < 0)
            {
                ModelState.AddModelError("QuantityOnHand", "Quantity cannot be negative.");
            }

            if (ModelState.IsValid)
            {
                _dbcontext.Consumables.Add(consumable);
                await _dbcontext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(consumable);
        }
    }
}
