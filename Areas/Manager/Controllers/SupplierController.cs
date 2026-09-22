using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
     [Authorize(Roles = "LabManager")]
    public class SupplierController : Controller
    {
        private readonly ILabManagerService _service;

        public SupplierController(ILabManagerService service)
        {
            _service = service;
        }

        // GET: /Manager/Supplier  (supplier.html)
        public async Task<IActionResult> Index()
        {
            var suppliers = await _service.GetSuppliersAsync();
            return View(suppliers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _service.CreateSupplierAsync(supplier);
            TempData["Success"] = $"Supplier '{supplier.Name}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            Debug.WriteLine($"SupplierId = {supplier.SupplierId}");
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _service.UpdateSupplierAsync(supplier);
            TempData["Success"] = $"Supplier '{supplier.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSupplierAsync(id);
            TempData["Success"] = "Supplier removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
