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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var consumables = await _context.Consumables
                .Include(c => c.Supplier)
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();

            await LoadSuppliers();

            return View(consumables);
        }


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

            return Json(new
            {
                success = true,
                consumable = new
                {
                    consumable.ConsumableId,
                    consumable.Name,
                    consumable.QuantityOnHand,
                    consumable.ReorderLevel,
                    StockStatus = consumable.StockStatus.ToString(),
                    consumable.SupplierId,

                    SupplierName =
                        consumable.Supplier != null
                            ? consumable.Supplier.SupplierName
                            : "No supplier"
                }
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ConsumableModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;

            await ValidateConsumable(model);

            if (!ModelState.IsValid)
            {
                await LoadSuppliers();

                var consumables =
                    await GetActiveConsumables();

                return View("Index", consumables);
            }


            // Check duplicated names

            bool duplicateName =
                await _context.Consumables
                    .AnyAsync(c =>
                        !c.IsDeleted &&
                        c.Name.ToLower() ==
                        model.Name.ToLower());

            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A consumable with this name already exists.");

                await LoadSuppliers();

                var consumables =
                    await GetActiveConsumables();

                return View("Index", consumables);
            }


            model.ConsumableId = 0;
            model.IsDeleted = false;

            model.QuantityOnHand =
                Math.Max(0, model.QuantityOnHand);


            // determine Initail status

            model.StockStatus =
                DetermineStockStatus(
                    model.QuantityOnHand,
                    model.ReorderLevel);


            _context.Consumables.Add(model);

            await _context.SaveChangesAsync();


            //create history

            var initialHistory =
                new StockStatusHistoryModel
                {
                    ConsumableId =
                        model.ConsumableId,

                    StockStatus =
                        model.StockStatus,

                    DateRecorded =
                        DateTime.UtcNow
                };

            _context.StockStatusHistories
                .Add(initialHistory);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Consumable '{model.Name}' added successfully.";


            return RedirectToAction(
                nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            ConsumableModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;


            var existing =
                await _context.Consumables
                    .FirstOrDefaultAsync(c =>
                        c.ConsumableId ==
                        model.ConsumableId &&
                        !c.IsDeleted);


            if (existing == null)
                return NotFound();


            await ValidateConsumable(model);



            bool duplicateName =
                await _context.Consumables
                    .AnyAsync(c =>
                        c.ConsumableId !=
                        model.ConsumableId &&
                        !c.IsDeleted &&
                        c.Name.ToLower() ==
                        model.Name.ToLower());


            if (duplicateName)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A consumable with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                await LoadSuppliers();

                var consumables =
                    await GetActiveConsumables();

                return View("Index", consumables);
            }


            existing.Name =
                model.Name;

            existing.ReorderLevel =
                model.ReorderLevel;

            existing.SupplierId =
                model.SupplierId;


            await UpdateStockStatusAndHistory(
                existing);


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Consumable '{existing.Name}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(
            int id,
            string adjustmentType,
            int quantity)
        {
            if (quantity < 0)
            {
                TempData["ErrorMessage"] =
                    "Quantity cannot be negative.";

                return RedirectToAction(
                    nameof(Index));
            }


            var consumable =
                await _context.Consumables
                    .FirstOrDefaultAsync(c =>
                        c.ConsumableId == id &&
                        !c.IsDeleted);


            if (consumable == null)
            {
                TempData["ErrorMessage"] =
                    "Consumable could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            int oldQuantity =
                consumable.QuantityOnHand;


            // calculate new quantity

            switch (
                adjustmentType?.ToLower())
            {
                case "increase":

                    consumable.QuantityOnHand =
                        checked(
                            consumable.QuantityOnHand +
                            quantity);

                    break;


                case "decrease":

                    if (quantity >
                        consumable.QuantityOnHand)
                    {
                        TempData["ErrorMessage"] =
                            "Stock cannot become negative.";

                        return RedirectToAction(
                            nameof(Index));
                    }

                    consumable.QuantityOnHand -=
                        quantity;

                    break;


                case "set":

                    consumable.QuantityOnHand =
                        quantity;

                    break;


                default:

                    TempData["ErrorMessage"] =
                        "Invalid stock adjustment type.";

                    return RedirectToAction(
                        nameof(Index));
            }


            // update status 

            await UpdateStockStatusAndHistory(
                consumable);


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Stock for '{consumable.Name}' updated from " +
                $"{oldQuantity} to " +
                $"{consumable.QuantityOnHand}.";


            return RedirectToAction(
                nameof(Index));
        }


        //soft delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var consumable =
                await _context.Consumables
                    .FirstOrDefaultAsync(c =>
                        c.ConsumableId == id &&
                        !c.IsDeleted);


            if (consumable == null)
            {
                TempData["ErrorMessage"] =
                    "Consumable could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            // check status

            bool hasActiveOrders =
                await _context.OrderItems
                    .AnyAsync(i =>
                        i.ConsumableId == id &&
                        i.Order != null &&
                        i.Order.Status !=
                        OrderStatus.Cancelled);


            if (hasActiveOrders)
            {
                TempData["ErrorMessage"] =
                    "This consumable cannot be deleted because it is linked to an active order.";

                return RedirectToAction(
                    nameof(Index));
            }


            consumable.IsDeleted = true;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Consumable '{consumable.Name}' deleted successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Reorder()
        {
            var consumables =
                await _context.Consumables
                    .Include(c => c.Supplier)
                    .Where(c =>
                        !c.IsDeleted &&
                        (
                            c.StockStatus ==
                                StockStatus.ReorderNow
                            ||
                            c.StockStatus ==
                                StockStatus.OutOfStock
                        ))
                    .OrderBy(c =>
                        c.StockStatus ==
                            StockStatus.OutOfStock
                            ? 0
                            : 1)
                    .ThenBy(c =>
                        c.QuantityOnHand)
                    .ToListAsync();


            return View(consumables);
        }



        private StockStatus DetermineStockStatus(
            int quantityOnHand,
            int reorderLevel)
        {
            // out of stock

            if (quantityOnHand <= 0)
            {
                return StockStatus.OutOfStock;
            }


            decimal reorderThreshold =
                reorderLevel * 1.10m;


            if (quantityOnHand <=
                reorderThreshold)
            {
                return StockStatus.ReorderNow;
            }


            return StockStatus.Sufficient;
        }


        private async Task UpdateStockStatusAndHistory(
            ConsumableModel consumable)
        {
            StockStatus newStatus =
                DetermineStockStatus(
                    consumable.QuantityOnHand,
                    consumable.ReorderLevel);


            // No status change.

            if (consumable.StockStatus ==
                newStatus)
            {
                return;
            }


            consumable.StockStatus =
                newStatus;


            // add records

            var history =
                new StockStatusHistoryModel
                {
                    ConsumableId =
                        consumable.ConsumableId,

                    StockStatus =
                        newStatus,

                    DateRecorded =
                        DateTime.UtcNow
                };


            _context.StockStatusHistories
                .Add(history);
        }


        private async Task ValidateConsumable(
            ConsumableModel model)
        {
            if (string.IsNullOrWhiteSpace(
                model.Name))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Consumable name is required.");
            }


            if (model.Name.Length > 150)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Consumable name cannot exceed 150 characters.");
            }


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

                return;
            }


            bool supplierExists =
                await _context.Suppliers
                    .AnyAsync(s =>
                        s.SupplierId ==
                        model.SupplierId &&
                        !s.IsDeleted);


            if (!supplierExists)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "The selected supplier is invalid.");
            }
        }


        private async Task LoadSuppliers()
        {
            ViewBag.Suppliers =
                await _context.Suppliers
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s =>
                        s.SupplierName)
                    .ToListAsync();
        }



        private async Task<List<ConsumableModel>> GetActiveConsumables()
        {
            return await _context.Consumables
                .Include(c => c.Supplier)
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
