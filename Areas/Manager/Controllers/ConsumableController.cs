using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ConsumableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumableController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var consumables = await _context.Consumables
                .Include(c => c.Supplier)
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Load suppliers for the Add/Edit dropdowns
            await LoadSuppliers();

            return View(consumables);
        }


        // ============================================================
        // DETAILS - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var consumable = await _context.Consumables
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(c =>
                    c.ConsumableId == id &&
                    !c.IsDeleted);

            if (consumable == null)
                return NotFound();

            return PartialView("_Details", consumable);
        }

        // ============================================================
        // CREATE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadSuppliers();

            return PartialView("_Create");
        }

        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumableModel model)
        {
            await ValidateConsumable(model);

            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return PartialView("_Create", model);
            }

            model.Name = model.Name.Trim();

            // Prevent duplicate names among active records.
            bool duplicateName = await _context.Consumables
                .AnyAsync(c =>
                    !c.IsDeleted &&
                    c.Name.ToLower() == model.Name.ToLower());

            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A consumable with this name already exists.");

                await LoadSuppliers();
                return PartialView("_Create", model);
            }

            model.QuantityOnHand = Math.Max(0, model.QuantityOnHand);

            _context.Consumables.Add(model);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Consumable added successfully."
            });
        }

        // ============================================================
        // EDIT - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var consumable = await _context.Consumables
                .FirstOrDefaultAsync(c =>
                    c.ConsumableId == id &&
                    !c.IsDeleted);

            if (consumable == null)
                return NotFound();

            await LoadSuppliers();

            return PartialView("_Edit", consumable);
        }

        // ============================================================
        // EDIT - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ConsumableModel model)
        {
            if (id != model.ConsumableId)
                return BadRequest();

            var existing = await _context.Consumables
                .FirstOrDefaultAsync(c =>
                    c.ConsumableId == id &&
                    !c.IsDeleted);

            if (existing == null)
                return NotFound();

            await ValidateConsumable(model);

            model.Name = model.Name.Trim();

            bool duplicateName = await _context.Consumables
                .AnyAsync(c =>
                    c.ConsumableId != id &&
                    !c.IsDeleted &&
                    c.Name.ToLower() == model.Name.ToLower());

            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A consumable with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return PartialView("_Edit", model);
            }

            existing.Name = model.Name;
            existing.ReorderLevel = model.ReorderLevel;
            existing.SupplierId = model.SupplierId;

            // QuantityOnHand should NOT normally be edited here.
            // Stock changes should go through StockAdjustmentController.

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Consumable updated successfully."
            });
        }

        // ============================================================
        // DELETE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var consumable = await _context.Consumables
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(c =>
                    c.ConsumableId == id &&
                    !c.IsDeleted);

            if (consumable == null)
                return NotFound();

            return PartialView("_Delete", consumable);
        }

        // ============================================================
        // DELETE - SOFT DELETE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumable = await _context.Consumables
                .FirstOrDefaultAsync(c =>
                    c.ConsumableId == id &&
                    !c.IsDeleted);

            if (consumable == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Consumable could not be found."
                });
            }

            // Do not allow deletion when active orders reference it.
            bool hasActiveOrders = await _context.OrderItems
                .AnyAsync(i =>
                    i.ConsumableId == id &&
                    i.Order!.Status != OrderStatus.Cancelled);

            if (hasActiveOrders)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This consumable cannot be deleted because it is linked to an active order."
                });
            }

            consumable.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Consumable deleted successfully."
            });
        }

        // ============================================================
        // REORDER LIST
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Reorder()
        {
            var consumables = await _context.Consumables
                .Include(c => c.Supplier)
                .Where(c =>
                    !c.IsDeleted &&
                    c.QuantityOnHand <= c.ReorderLevel * 1.10)
                .OrderBy(c => c.QuantityOnHand)
                .ToListAsync();

            return View(consumables);
        }

        // ============================================================
        // VALIDATION
        // ============================================================

        private async Task ValidateConsumable(
            ConsumableModel model)
        {
            if (model.ReorderLevel < 0)
            {
                ModelState.AddModelError(
                    nameof(model.ReorderLevel),
                    "Reorder level cannot be negative.");
            }

            if (model.QuantityOnHand < 0)
            {
                ModelState.AddModelError(
                    nameof(model.QuantityOnHand),
                    "Quantity on hand cannot be negative.");
            }

            if (model.SupplierId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a supplier.");
            }
            else
            {
                bool supplierExists =
                    await _context.Suppliers.AnyAsync(s =>
                        s.SupplierId == model.SupplierId &&
                        !s.IsDeleted);

                if (!supplierExists)
                {
                    ModelState.AddModelError(
                        nameof(model.SupplierId),
                        "The selected supplier is invalid.");
                }
            }
        }

        // ============================================================
        // SUPPLIER DROPDOWN
        // ============================================================

        private async Task LoadSuppliers()
        {
            ViewBag.Suppliers = await _context.Suppliers
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }
    }
}
