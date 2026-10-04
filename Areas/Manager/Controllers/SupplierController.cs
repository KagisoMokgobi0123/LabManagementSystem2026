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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.Consumables
                    .Where(c => !c.IsDeleted))
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
            // Trim values before validation and duplicate checking.
            model.SupplierName =
                model.SupplierName?.Trim() ?? string.Empty;

            model.ContactPerson =
                model.ContactPerson?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;


            // Validate supplier.
            await ValidateSupplier(model);


            // Prevent duplicate active supplier names.
            bool duplicateName =
                await _context.Suppliers
                    .AnyAsync(s =>
                        !s.IsDeleted &&
                        s.SupplierName.ToLower() ==
                        model.SupplierName.ToLower());


            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "A supplier with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model);
            }


            // Make sure this is treated as a new entity.
            model.SupplierId = 0;

            model.IsDeleted = false;


            _context.Suppliers.Add(model);

            await _context.SaveChangesAsync();


            return Json(new
            {
                success = true,
                message = $"Supplier '{model.SupplierName}' added successfully."
            });
        }


        // ============================================================
        // EDIT - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier =
                await _context.Suppliers
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
            // Make sure the route ID and submitted ID match.
            if (id != model.SupplierId)
            {
                return BadRequest();
            }


            // Load the existing active supplier.
            var existing =
                await _context.Suppliers
                    .FirstOrDefaultAsync(s =>
                        s.SupplierId == id &&
                        !s.IsDeleted);


            if (existing == null)
                return NotFound();


            // Trim values before validation and duplicate checking.
            model.SupplierName =
                model.SupplierName?.Trim() ?? string.Empty;

            model.ContactPerson =
                model.ContactPerson?.Trim() ?? string.Empty;

            model.Email =
                model.Email?.Trim() ?? string.Empty;


            // Validate supplier.
            await ValidateSupplier(model);


            // Prevent duplicate active supplier names.
            bool duplicateName =
                await _context.Suppliers
                    .AnyAsync(s =>
                        s.SupplierId != id &&
                        !s.IsDeleted &&
                        s.SupplierName.ToLower() ==
                        model.SupplierName.ToLower());


            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "A supplier with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }


            // Update editable fields only.
            existing.SupplierName =
                model.SupplierName;

            existing.ContactPerson =
                model.ContactPerson;

            existing.Email =
                model.Email;


            // Keep the existing deletion state unchanged.
            existing.IsDeleted = false;


            await _context.SaveChangesAsync();


            return Json(new
            {
                success = true,
                message =
                    $"Supplier '{existing.SupplierName}' updated successfully."
            });
        }


        // ============================================================
        // DELETE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier =
                await _context.Suppliers
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
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var supplier =
                await _context.Suppliers
                    .FirstOrDefaultAsync(s =>
                        s.SupplierId == id &&
                        !s.IsDeleted);


            if (supplier == null)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Supplier could not be found."
                });
            }


            // Do not allow deletion when active consumables
            // reference this supplier.
            bool hasActiveConsumables =
                await _context.Consumables
                    .AnyAsync(c =>
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


            // Soft delete.
            supplier.IsDeleted = true;


            await _context.SaveChangesAsync();


            return Json(new
            {
                success = true,
                message =
                    $"Supplier '{supplier.SupplierName}' deleted successfully."
            });
        }


        // ============================================================
        // VALIDATION
        // ============================================================

        private async Task ValidateSupplier(
            SupplierModel model)
        {
            // Supplier name is required.
            if (string.IsNullOrWhiteSpace(
                model.SupplierName))
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "Supplier name is required.");
            }


            // Supplier name length.
            if (model.SupplierName.Length > 150)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierName),
                    "Supplier name cannot exceed 150 characters.");
            }


            // Contact person is optional, but if supplied,
            // remove unnecessary whitespace through trimming above.
            if (model.ContactPerson.Length > 150)
            {
                ModelState.AddModelError(
                    nameof(model.ContactPerson),
                    "Contact person cannot exceed 150 characters.");
            }


            // Email is optional, but if supplied it must be valid.
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var emailAttribute =
                    new System.ComponentModel.DataAnnotations.EmailAddressAttribute();

                if (!emailAttribute.IsValid(model.Email))
                {
                    ModelState.AddModelError(
                        nameof(model.Email),
                        "Please enter a valid email address.");
                }
            }


            if (model.Email.Length > 254)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email address cannot exceed 254 characters.");
            }


            await Task.CompletedTask;
        }


        // ============================================================
        // LOAD ACTIVE SUPPLIERS
        // ============================================================

        private async Task<List<SupplierModel>>
            GetActiveSuppliers()
        {
            return await _context.Suppliers
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }
    }


}