using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Consumable)
                .OrderByDescending(o => o.OrderId)
                .ToListAsync();

            return View(orders);
        }


        // ============================================================
        // DETAILS - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Consumable)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return PartialView("_Details", order);
        }


        // ============================================================
        // CREATE - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadOrderData();

            var model = new CreateOrderViewModel();

            return PartialView("_Create", model);
        }


        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateOrderViewModel model)
        {
            // ========================================================
            // CLEAN INPUT
            // ========================================================

            model.OrderNumber =
                model.OrderNumber?.Trim() ?? string.Empty;


            // ========================================================
            // BASIC MODEL VALIDATION
            // ========================================================

            if (!ModelState.IsValid)
            {
                await LoadOrderData();

                return PartialView("_Create", model);
            }


            // ========================================================
            // ORDER NUMBER VALIDATION
            // ========================================================

            if (string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number is required.");
            }

            if (model.OrderNumber.Length > 50)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number cannot exceed 50 characters.");
            }


            // ========================================================
            // ORDER NUMBER UNIQUENESS
            // ========================================================

            bool orderNumberExists =
                await _context.Orders.AnyAsync(o =>
                    o.OrderNumber.ToLower()
                    == model.OrderNumber.ToLower());

            if (orderNumberExists)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "This order number already exists.");
            }


            // ========================================================
            // SUPPLIER VALIDATION
            // ========================================================

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.SupplierId == model.SupplierId &&
                    !s.IsDeleted);

            if (supplier == null)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a valid supplier.");
            }


            // ========================================================
            // ORDER ITEMS VALIDATION
            // ========================================================

            if (model.Items == null ||
                !model.Items.Any())
            {
                ModelState.AddModelError(
                    nameof(model.Items),
                    "At least one consumable must be added.");
            }


            if (!ModelState.IsValid)
            {
                await LoadOrderData();

                return PartialView("_Create", model);
            }


            // ========================================================
            // DUPLICATE CONSUMABLE VALIDATION
            // ========================================================

            var duplicateConsumables =
                model.Items
                    .GroupBy(i => i.ConsumableId)
                    .Where(g => g.Count() > 1)
                    .ToList();

            if (duplicateConsumables.Any())
            {
                ModelState.AddModelError(
                    nameof(model.Items),
                    "The same consumable cannot be added more than once.");

                await LoadOrderData();

                return PartialView("_Create", model);
            }


            // ========================================================
            // LOAD SELECTED CONSUMABLES
            // ========================================================

            var consumableIds = model.Items
                .Select(i => i.ConsumableId)
                .Distinct()
                .ToList();

            var consumables = await _context.Consumables
                .Where(c =>
                    consumableIds.Contains(c.ConsumableId) &&
                    !c.IsDeleted)
                .ToListAsync();


            // ========================================================
            // VERIFY ALL CONSUMABLES EXIST
            // ========================================================

            if (consumables.Count != consumableIds.Count)
            {
                ModelState.AddModelError(
                    nameof(model.Items),
                    "One or more selected consumables are invalid.");

                await LoadOrderData();

                return PartialView("_Create", model);
            }


            // ========================================================
            // VALIDATE EACH ORDER ITEM
            // ========================================================

            foreach (var item in model.Items)
            {
                var consumable = consumables
                    .First(c =>
                        c.ConsumableId == item.ConsumableId);


                // ----------------------------------------------------
                // QUANTITY
                // ----------------------------------------------------

                if (item.QuantityOrdered <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.Items),
                        $"Quantity for '{consumable.Name}' must be greater than zero.");
                }


                // ----------------------------------------------------
                // SUPPLIER
                // ----------------------------------------------------

                if (consumable.SupplierId != model.SupplierId)
                {
                    ModelState.AddModelError(
                        nameof(model.Items),
                        $"'{consumable.Name}' is not supplied by the selected supplier.");
                }
            }


            // ========================================================
            // FINAL VALIDATION CHECK
            // ========================================================

            if (!ModelState.IsValid)
            {
                await LoadOrderData();

                return PartialView("_Create", model);
            }


            // ========================================================
            // CREATE ORDER + ORDER ITEMS
            // ========================================================

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // ----------------------------------------------------
                // CREATE ORDER
                // ----------------------------------------------------

                var order = new OrderModel
                {
                    OrderNumber = model.OrderNumber,
                    SupplierId = model.SupplierId,
                    Status = OrderStatus.Ordered
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();


                // ----------------------------------------------------
                // CREATE ORDER ITEMS
                // ----------------------------------------------------

                foreach (var item in model.Items)
                {
                    var orderItem = new OrderItemModel
                    {
                        OrderId = order.OrderId,
                        ConsumableId = item.ConsumableId,
                        QuantityOrdered =
                            item.QuantityOrdered,
                        Status = OrderItemStatus.Ordered
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();


                // ----------------------------------------------------
                // COMMIT
                // ----------------------------------------------------

                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Order created successfully."
                });
            }
            catch
            {
                // ----------------------------------------------------
                // ROLLBACK
                // ----------------------------------------------------

                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message =
                        "The order could not be created. Please try again."
                });
            }
        }


        // ============================================================
        // EDIT - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Consumable)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == id);

            if (order == null)
                return NotFound();

            // --------------------------------------------------------
            // Completed and cancelled orders should not be edited.
            // --------------------------------------------------------

            if (order.Status == OrderStatus.Complete ||
                order.Status == OrderStatus.Cancelled)
            {
                return BadRequest(
                    "Completed or cancelled orders cannot be edited.");
            }

            await LoadOrderData();

            return PartialView("_Edit", order);
        }


        // ============================================================
        // EDIT - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            OrderModel model)
        {
            if (id != model.OrderId)
                return BadRequest();


            // ========================================================
            // LOAD EXISTING ORDER
            // ========================================================

            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == id);

            if (existingOrder == null)
                return NotFound();


            // ========================================================
            // STATUS PROTECTION
            // ========================================================

            if (existingOrder.Status == OrderStatus.Complete ||
                existingOrder.Status == OrderStatus.Cancelled)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Completed or cancelled orders cannot be edited."
                });
            }


            // ========================================================
            // CLEAN INPUT
            // ========================================================

            model.OrderNumber =
                model.OrderNumber?.Trim() ?? string.Empty;


            // ========================================================
            // ORDER NUMBER VALIDATION
            // ========================================================

            if (string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number is required.");
            }


            // ========================================================
            // UNIQUE ORDER NUMBER
            // ========================================================

            bool duplicateOrderNumber =
                await _context.Orders.AnyAsync(o =>
                    o.OrderId != id &&
                    o.OrderNumber.ToLower()
                    == model.OrderNumber.ToLower());

            if (duplicateOrderNumber)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "This order number already exists.");
            }


            // ========================================================
            // SUPPLIER VALIDATION
            // ========================================================

            bool supplierExists =
                await _context.Suppliers.AnyAsync(s =>
                    s.SupplierId == model.SupplierId &&
                    !s.IsDeleted);

            if (!supplierExists)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a valid supplier.");
            }


            // ========================================================
            // VALIDATE MODEL
            // ========================================================

            if (!ModelState.IsValid)
            {
                await LoadOrderData();

                return PartialView("_Edit", model);
            }


            // ========================================================
            // UPDATE BASIC ORDER INFORMATION
            // ========================================================

            existingOrder.OrderNumber =
                model.OrderNumber;

            existingOrder.SupplierId =
                model.SupplierId;


            // ========================================================
            // SAVE
            // ========================================================

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Order updated successfully."
            });
        }


        // ============================================================
        // RECEIVE ITEM
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveItem(int id)
        {
            var item = await _context.OrderItems
                .Include(i => i.Order)
                .Include(i => i.Consumable)
                .FirstOrDefaultAsync(i =>
                    i.OrderItemId == id);

            if (item == null)
                return NotFound();


            // ========================================================
            // ITEM STATUS CHECK
            // ========================================================

            if (item.Status != OrderItemStatus.Ordered)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This order item has already been processed."
                });
            }


            // ========================================================
            // REQUIRED RELATIONSHIPS
            // ========================================================

            if (item.Order == null ||
                item.Consumable == null)
            {
                return BadRequest();
            }


            // ========================================================
            // ORDER STATUS CHECK
            // ========================================================

            if (item.Order.Status == OrderStatus.Cancelled)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Items from a cancelled order cannot be received."
                });
            }


            // ========================================================
            // UPDATE STOCK
            // ========================================================

            try
            {
                checked
                {
                    item.Consumable.QuantityOnHand +=
                        item.QuantityOrdered;
                }
            }
            catch (OverflowException)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "The stock quantity is too large to process."
                });
            }


            // ========================================================
            // UPDATE ORDER ITEM
            // ========================================================

            item.Status =
                OrderItemStatus.Received;

            item.DateReceived =
                DateTime.Now;


            // ========================================================
            // UPDATE ORDER STATUS
            // ========================================================

            await UpdateOrderStatus(item.Order);


            // ========================================================
            // SAVE
            // ========================================================

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message =
                    "Item received and stock updated successfully."
            });
        }


        // ============================================================
        // CANCEL ITEM - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> CancelItem(int id)
        {
            var item = await _context.OrderItems
                .Include(i => i.Order)
                .Include(i => i.Consumable)
                .FirstOrDefaultAsync(i =>
                    i.OrderItemId == id);

            if (item == null)
                return NotFound();


            if (item.Status != OrderItemStatus.Ordered)
            {
                return BadRequest(
                    "This item can no longer be cancelled.");
            }

            return PartialView("_CancelItem", item);
        }


        // ============================================================
        // CANCEL ITEM - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelItem(
            int id,
            string cancellationReason)
        {
            cancellationReason =
                cancellationReason?.Trim() ?? string.Empty;


            // ========================================================
            // REASON VALIDATION
            // ========================================================

            if (string.IsNullOrWhiteSpace(
                cancellationReason))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "A cancellation reason is required."
                });
            }


            if (cancellationReason.Length > 500)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Cancellation reason cannot exceed 500 characters."
                });
            }


            // ========================================================
            // LOAD ITEM
            // ========================================================

            var item = await _context.OrderItems
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i =>
                    i.OrderItemId == id);

            if (item == null)
                return NotFound();


            // ========================================================
            // STATUS VALIDATION
            // ========================================================

            if (item.Status != OrderItemStatus.Ordered)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Only ordered items can be cancelled."
                });
            }


            if (item.Order == null)
                return BadRequest();


            if (item.Order.Status ==
                OrderStatus.Cancelled)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This order has already been cancelled."
                });
            }


            // ========================================================
            // CANCEL ITEM
            // ========================================================

            item.Status =
                OrderItemStatus.Cancelled;

            item.CancellationReason =
                cancellationReason;

            item.DateCancelled =
                DateTime.Now;


            // ========================================================
            // UPDATE ORDER STATUS
            // ========================================================

            await UpdateOrderStatus(item.Order);


            // ========================================================
            // SAVE
            // ========================================================

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message =
                    "Order item cancelled successfully."
            });
        }


        // ============================================================
        // CANCEL ORDER - POPUP
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == id);

            if (order == null)
                return NotFound();


            if (order.Status == OrderStatus.Complete ||
                order.Status == OrderStatus.Cancelled)
            {
                return BadRequest(
                    "This order can no longer be cancelled.");
            }


            return PartialView("_Cancel", order);
        }


        // ============================================================
        // CANCEL ORDER - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(
            int id,
            string cancellationReason)
        {
            cancellationReason =
                cancellationReason?.Trim() ?? string.Empty;


            // ========================================================
            // VALIDATE REASON
            // ========================================================

            if (string.IsNullOrWhiteSpace(
                cancellationReason))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "A cancellation reason is required."
                });
            }


            if (cancellationReason.Length > 500)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Cancellation reason cannot exceed 500 characters."
                });
            }


            // ========================================================
            // LOAD ORDER
            // ========================================================

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == id);

            if (order == null)
                return NotFound();


            // ========================================================
            // STATUS VALIDATION
            // ========================================================

            if (order.Status == OrderStatus.Complete)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "A completed order cannot be cancelled."
                });
            }


            if (order.Status == OrderStatus.Cancelled)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This order has already been cancelled."
                });
            }


            // ========================================================
            // CANCEL ORDER
            // ========================================================

            order.Status =
                OrderStatus.Cancelled;

            order.CancellationReason =
                cancellationReason;

            order.DateCancelled =
                DateTime.Now;


            // ========================================================
            // CANCEL REMAINING ORDER ITEMS
            // ========================================================

            foreach (var item in order.OrderItems)
            {
                // ----------------------------------------------------
                // Only items that have not already been received
                // can be cancelled.
                // ----------------------------------------------------

                if (item.Status ==
                    OrderItemStatus.Ordered)
                {
                    item.Status =
                        OrderItemStatus.Cancelled;

                    item.CancellationReason =
                        cancellationReason;

                    item.DateCancelled =
                        DateTime.Now;
                }
            }


            // ========================================================
            // SAVE
            // ========================================================

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message =
                    "Order cancelled successfully."
            });
        }


        // ============================================================
        // UPDATE ORDER STATUS
        // ============================================================

        private async Task UpdateOrderStatus(
            OrderModel order)
        {
            var items = await _context.OrderItems
                .Where(i =>
                    i.OrderId == order.OrderId)
                .ToListAsync();


            if (!items.Any())
            {
                order.Status =
                    OrderStatus.Ordered;

                return;
            }


            // ========================================================
            // CHECK ITEM STATES
            // ========================================================

            bool hasOrdered =
                items.Any(i =>
                    i.Status ==
                    OrderItemStatus.Ordered);

            bool hasReceived =
                items.Any(i =>
                    i.Status ==
                    OrderItemStatus.Received);

            bool hasCancelled =
                items.Any(i =>
                    i.Status ==
                    OrderItemStatus.Cancelled);


            // ========================================================
            // ORDERED
            // ========================================================

            if (hasOrdered &&
                !hasReceived &&
                !hasCancelled)
            {
                order.Status =
                    OrderStatus.Ordered;

                return;
            }


            // ========================================================
            // PARTIALLY COMPLETE
            // ========================================================

            if (hasOrdered && hasReceived)
            {
                order.Status =
                    OrderStatus.PartiallyComplete;

                return;
            }


            // ========================================================
            // PARTIALLY COMPLETE
            // ========================================================
            //
            // Example:
            //
            // Item 1 = Received
            // Item 2 = Cancelled
            //
            // The order is not "fully received", but there are
            // no remaining ordered items.
            //
            // We consider this complete because there is nothing
            // left outstanding.
            // ========================================================

            if (!hasOrdered &&
                (hasReceived || hasCancelled))
            {
                order.Status =
                    OrderStatus.Complete;

                if (!order.DateComplete.HasValue)
                {
                    order.DateComplete =
                        DateTime.Now;
                }

                return;
            }


            // ========================================================
            // FALLBACK
            // ========================================================

            order.Status =
                OrderStatus.Ordered;
        }


        // ============================================================
        // LOAD SUPPLIERS AND CONSUMABLES
        // ============================================================

        private async Task LoadOrderData()
        {
            ViewBag.Suppliers =
                await _context.Suppliers
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.SupplierName)
                    .ToListAsync();


            ViewBag.Consumables =
                await _context.Consumables
                    .Where(c => !c.IsDeleted)
                    .Include(c => c.Supplier)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
        }
    }
}
