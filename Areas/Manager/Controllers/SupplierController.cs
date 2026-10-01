using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX
        // ============================================================

        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return View(suppliers);
        }

        // ============================================================
        // DETAILS - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Consumables
                    .Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == id &&
                    !s.IsDeleted);

            if (supplier == null)
                return NotFound();

            return PartialView("_Details", supplier);
        }

        // ============================================================
        // CREATE - POPUP
        // ============================================================

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SupplierModel model)
        {
            model.SupplierName =
                model.SupplierName?.Trim() ?? string.Empty;

            model.ContactPerson =
                model.ContactPerson?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            bool duplicateName = await _context.Suppliers
                .AnyAsync(s =>
                    !s.IsDeleted &&
                    s.SupplierName.ToLower()
                    == model.SupplierName.ToLower());

            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "A supplier with this name already exists.");
            }

            if (!ModelState.IsValid)
                return PartialView("_Create", model);

            _context.Suppliers.Add(model);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Supplier added successfully."
            });
        }

        // ============================================================
        // EDIT - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == id &&
                    !s.IsDeleted);

            if (supplier == null)
                return NotFound();

            return PartialView("_Edit", supplier);
        }

        // ============================================================
        // EDIT - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SupplierModel model)
        {
            if (id != model.SupplierId)
                return BadRequest();

            var existing = await _context.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == id &&
                    !s.IsDeleted);

            if (existing == null)
                return NotFound();

            model.SupplierName =
                model.SupplierName?.Trim() ?? string.Empty;

            model.ContactPerson =
                model.ContactPerson?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;

            bool duplicateName = await _context.Suppliers
                .AnyAsync(s =>
                    s.SupplierId != id &&
                    !s.IsDeleted &&
                    s.SupplierName.ToLower()
                    == model.SupplierName.ToLower());

            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "A supplier with this name already exists.");
            }

            if (!ModelState.IsValid)
                return PartialView("_Edit", model);

            existing.SupplierName = model.SupplierName;
            existing.ContactPerson = model.ContactPerson;
            existing.Email = model.Email;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Supplier updated successfully."
            });
        }

        // ============================================================
        // DELETE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == id &&
                    !s.IsDeleted);

            if (supplier == null)
                return NotFound();

            return PartialView("_Delete", supplier);
        }

        // ============================================================
        // DELETE - SOFT DELETE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == id &&
                    !s.IsDeleted);

            if (supplier == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Supplier could not be found."
                });
            }

            bool hasActiveConsumables =
                await _context.Consumables.AnyAsync(c =>
                    c.SupplierId == id &&
                    !c.IsDeleted);

            if (hasActiveConsumables)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This supplier cannot be deleted because active consumables are linked to it."
                });
            }

            supplier.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Supplier deleted successfully."
            });
        }
    }
}
