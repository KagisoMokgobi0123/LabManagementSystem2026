using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class StockAdjustmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockAdjustmentController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX
        // ============================================================

        public async Task<IActionResult> Index()
        {
            var adjustments =
                await _context.StockAdjustments
                    .Include(a => a.Consumable)
                    .Where(a => !a.Consumable!.IsDeleted)
                    .OrderByDescending(a =>
                        a.AdjustmentDate)
                    .ToListAsync();

            return View(adjustments);
        }

        // ============================================================
        // CREATE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create(
            int? consumableId)
        {
            await LoadConsumables();

            var model = new StockAdjustmentModel();

            if (consumableId.HasValue)
                model.ConsumableId =
                    consumableId.Value;

            return PartialView("_Create", model);
        }

        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StockAdjustmentModel model)
        {
            var consumable =
                await _context.Consumables
                    .FirstOrDefaultAsync(c =>
                        c.ConsumableId ==
                        model.ConsumableId &&
                        !c.IsDeleted);

            if (consumable == null)
            {
                ModelState.AddModelError(
                    nameof(model.ConsumableId),
                    "Please select a valid consumable.");
            }

            if (model.Quantity < 0)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    "Quantity cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                await LoadConsumables();
                return PartialView("_Create", model);
            }

            // ========================================================
            // APPLY ADJUSTMENT
            // ========================================================

            switch (model.AdjustmentType)
            {
                case StockAdjustmentType.Increase:

                    checked
                    {
                        consumable!.QuantityOnHand +=
                            model.Quantity;
                    }

                    break;

                case StockAdjustmentType.Decrease:

                    if (model.Quantity >
                        consumable!.QuantityOnHand)
                    {
                        ModelState.AddModelError(
                            nameof(model.Quantity),
                            "Quantity cannot reduce stock below zero.");

                        await LoadConsumables();

                        return PartialView(
                            "_Create",
                            model);
                    }

                    consumable.QuantityOnHand -=
                        model.Quantity;

                    break;

                case StockAdjustmentType.Set:

                    consumable!.QuantityOnHand =
                        model.Quantity;

                    break;

                default:

                    ModelState.AddModelError(
                        nameof(model.AdjustmentType),
                        "Invalid adjustment type.");

                    await LoadConsumables();

                    return PartialView(
                        "_Create",
                        model);
            }

            model.AdjustmentDate = DateTime.Now;

            _context.StockAdjustments.Add(model);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Stock updated successfully."
            });
        }

        // ============================================================
        // LOAD CONSUMABLES
        // ============================================================

        private async Task LoadConsumables()
        {
            ViewBag.Consumables =
                await _context.Consumables
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
        }
    }
}
