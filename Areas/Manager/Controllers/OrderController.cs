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
            try
            {
                await LoadOrderData();

                var orders = await GetOrders();

                return View(orders);
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to load the orders page.");
            }
        }

        // ============================================================
        // DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _context.Orders
                    .AsNoTracking()
                    .Include(o => o.Supplier)
                    .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Consumable)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected order could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                await LoadOrderData();

                // Keep the existing Index view.
                // The selected order and its related IDs are loaded above.
                return View("Index", await GetOrders());
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to load the selected order.");
            }
        }

        // ============================================================
        // CREATE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateOrderViewModel model)
        {
            try
            {
                model.OrderNumber =
                    model.OrderNumber?.Trim() ?? string.Empty;

                model.Items ??=
                    new List<CreateOrderItemViewModel>();

                // ----------------------------------------------------
                // SERVER-SIDE VALIDATION
                // ----------------------------------------------------

                await ValidateCreateOrder(model);

                if (!ModelState.IsValid)
                {
                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                model.Items = model.Items
                    .Where(i => i != null)
                    .ToList();

                if (!model.Items.Any())
                {
                    ModelState.AddModelError(
                        nameof(model.Items),
                        "At least one consumable must be added.");

                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                // ----------------------------------------------------
                // DUPLICATE CONSUMABLE CHECK
                // ----------------------------------------------------

                bool hasDuplicates = model.Items
                    .GroupBy(i => i.ConsumableId)
                    .Any(g => g.Count() > 1);

                if (hasDuplicates)
                {
                    ModelState.AddModelError(
                        nameof(model.Items),
                        "The same consumable cannot be added more than once.");

                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                // ----------------------------------------------------
                // LOAD SUPPLIER USING SupplierId
                // ----------------------------------------------------

                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s =>
                        s.SupplierId == model.SupplierId &&
                        !s.IsDeleted);

                if (supplier == null)
                {
                    ModelState.AddModelError(
                        nameof(model.SupplierId),
                        "The selected supplier does not exist.");

                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                // ----------------------------------------------------
                // LOAD CONSUMABLES USING ConsumableId
                // ----------------------------------------------------

                var consumableIds = model.Items
                    .Select(i => i.ConsumableId)
                    .Distinct()
                    .ToList();

                var consumables = await _context.Consumables
                    .Where(c =>
                        consumableIds.Contains(c.ConsumableId) &&
                        !c.IsDeleted)
                    .ToListAsync();

                if (consumables.Count != consumableIds.Count)
                {
                    ModelState.AddModelError(
                        nameof(model.Items),
                        "One or more selected consumables could not be found.");

                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                // ----------------------------------------------------
                // VALIDATE EACH CONSUMABLE
                // ----------------------------------------------------

                foreach (var item in model.Items)
                {
                    var consumable = consumables
                        .FirstOrDefault(c =>
                            c.ConsumableId == item.ConsumableId);

                    if (consumable == null)
                    {
                        ModelState.AddModelError(
                            nameof(model.Items),
                            "One or more selected consumables are invalid.");

                        continue;
                    }

                    if (item.QuantityOrdered <= 0)
                    {
                        ModelState.AddModelError(
                            nameof(model.Items),
                            $"Quantity for '{consumable.Name}' must be greater than zero.");
                    }

                    if (consumable.SupplierId != model.SupplierId)
                    {
                        ModelState.AddModelError(
                            nameof(model.Items),
                            $"'{consumable.Name}' does not belong to the selected supplier.");
                    }

                    if (consumable.StockStatus != StockStatus.ReorderNow &&
                        consumable.StockStatus != StockStatus.OutOfStock)
                    {
                        ModelState.AddModelError(
                            nameof(model.Items),
                            $"'{consumable.Name}' does not currently require reordering.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                // ====================================================
                // TRANSACTION
                // ====================================================

                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // ------------------------------------------------
                    // STEP 1: CREATE ORDER
                    // OrderId is generated by the database.
                    // ------------------------------------------------

                    var order = new OrderModel
                    {
                        OrderNumber = model.OrderNumber,
                        SupplierId = supplier.SupplierId,
                        Status = OrderStatus.Ordered,
                        DateComplete = null,
                        CancellationReason = null,
                        DateCancelled = null
                    };

                    _context.Orders.Add(order);

                    await _context.SaveChangesAsync();

                    // order.OrderId now contains the database-generated ID.

                    // ------------------------------------------------
                    // STEP 2: CREATE ORDER ITEMS
                    // Each OrderItemId is generated by the database.
                    // ------------------------------------------------

                    foreach (var item in model.Items)
                    {
                        var orderItem = new OrderItemModel
                        {
                            OrderId = order.OrderId,
                            ConsumableId = item.ConsumableId,
                            QuantityOrdered = item.QuantityOrdered,
                            Status = OrderItemStatus.Ordered,
                            DateReceived = null,
                            CancellationReason = null,
                            DateCancelled = null
                        };

                        _context.OrderItems.Add(orderItem);
                    }

                    // ------------------------------------------------
                    // STEP 3: SAVE ORDER ITEMS
                    // ------------------------------------------------

                    await _context.SaveChangesAsync();

                    // ------------------------------------------------
                    // STEP 4: COMMIT TRANSACTION
                    // ------------------------------------------------

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] =
                        $"Order '{order.OrderNumber}' was created successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                var root = GetRootException(ex);

                Console.Error.WriteLine(
                    "============================================================");
                Console.Error.WriteLine("ORDER CREATE DATABASE ERROR");
                Console.Error.WriteLine($"Message: {ex.Message}");
                Console.Error.WriteLine($"Root Message: {root.Message}");
                Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.Error.WriteLine(
                    "============================================================");

                TempData["ErrorMessage"] =
                    $"The order could not be saved. {root.Message}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to create the order.");
            }
        }

        // ============================================================
        // EDIT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderModel model)
        {
            try
            {
                model.OrderNumber =
                    model.OrderNumber?.Trim() ?? string.Empty;

                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == model.OrderId);

                if (existingOrder == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected order could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                if (existingOrder.Status == OrderStatus.Complete ||
                    existingOrder.Status == OrderStatus.Cancelled)
                {
                    TempData["ErrorMessage"] =
                        "Completed or cancelled orders cannot be edited.";

                    return RedirectToAction(nameof(Index));
                }

                await ValidateEditOrder(model.OrderId, model);

                if (!ModelState.IsValid)
                {
                    await LoadOrderData();
                    return View("Index", await GetOrders());
                }

                existingOrder.OrderNumber = model.OrderNumber;
                existingOrder.SupplierId = model.SupplierId;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    $"Order '{existingOrder.OrderNumber}' updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to update the order.");
            }
        }

        // ============================================================
        // RECEIVE ITEM
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveItem(int id)
        {
            try
            {
                var item = await _context.OrderItems
                    .Include(i => i.Order)
                    .Include(i => i.Consumable)
                    .FirstOrDefaultAsync(i => i.OrderItemId == id);

                if (item == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected order item could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Status != OrderItemStatus.Ordered)
                {
                    TempData["ErrorMessage"] =
                        "This order item has already been processed.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Order == null || item.Consumable == null)
                {
                    TempData["ErrorMessage"] =
                        "The order item is missing required information.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Order.Status == OrderStatus.Cancelled)
                {
                    TempData["ErrorMessage"] =
                        "Items from a cancelled order cannot be received.";

                    return RedirectToAction(nameof(Index));
                }

                checked
                {
                    item.Consumable.QuantityOnHand +=
                        item.QuantityOrdered;
                }

                item.Consumable.StockStatus =
                    CalculateStockStatus(
                        item.Consumable.QuantityOnHand,
                        item.Consumable.ReorderLevel);

                item.Status = OrderItemStatus.Received;
                item.DateReceived = DateTime.Now;

                await UpdateOrderStatus(item.Order);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Item received and stock updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (OverflowException)
            {
                TempData["ErrorMessage"] =
                    "The stock quantity is too large to process.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to receive the order item.");
            }
        }

        // ============================================================
        // CANCEL ITEM
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelItem(
            int id,
            string cancellationReason)
        {
            try
            {
                cancellationReason =
                    cancellationReason?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(cancellationReason))
                {
                    TempData["ErrorMessage"] =
                        "A cancellation reason is required.";

                    return RedirectToAction(nameof(Index));
                }

                if (cancellationReason.Length > 500)
                {
                    TempData["ErrorMessage"] =
                        "Cancellation reason cannot exceed 500 characters.";

                    return RedirectToAction(nameof(Index));
                }

                var item = await _context.OrderItems
                    .Include(i => i.Order)
                    .FirstOrDefaultAsync(i => i.OrderItemId == id);

                if (item == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected order item could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Status != OrderItemStatus.Ordered)
                {
                    TempData["ErrorMessage"] =
                        "Only ordered items can be cancelled.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Order == null)
                {
                    TempData["ErrorMessage"] =
                        "The order could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                if (item.Order.Status == OrderStatus.Cancelled)
                {
                    TempData["ErrorMessage"] =
                        "This order has already been cancelled.";

                    return RedirectToAction(nameof(Index));
                }

                item.Status = OrderItemStatus.Cancelled;
                item.CancellationReason = cancellationReason;
                item.DateCancelled = DateTime.Now;

                await UpdateOrderStatus(item.Order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Order item cancelled successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to cancel the order item.");
            }
        }

        // ============================================================
        // CANCEL ORDER
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(
            int id,
            string cancellationReason)
        {
            try
            {
                cancellationReason =
                    cancellationReason?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(cancellationReason))
                {
                    TempData["ErrorMessage"] =
                        "A cancellation reason is required.";

                    return RedirectToAction(nameof(Index));
                }

                if (cancellationReason.Length > 500)
                {
                    TempData["ErrorMessage"] =
                        "Cancellation reason cannot exceed 500 characters.";

                    return RedirectToAction(nameof(Index));
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    TempData["ErrorMessage"] =
                        "The selected order could not be found.";

                    return RedirectToAction(nameof(Index));
                }

                if (order.Status == OrderStatus.Complete)
                {
                    TempData["ErrorMessage"] =
                        "A completed order cannot be cancelled.";

                    return RedirectToAction(nameof(Index));
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    TempData["ErrorMessage"] =
                        "This order has already been cancelled.";

                    return RedirectToAction(nameof(Index));
                }

                order.Status = OrderStatus.Cancelled;
                order.CancellationReason = cancellationReason;
                order.DateCancelled = DateTime.Now;

                foreach (var item in order.OrderItems)
                {
                    if (item.Status == OrderItemStatus.Ordered)
                    {
                        item.Status = OrderItemStatus.Cancelled;
                        item.CancellationReason = cancellationReason;
                        item.DateCancelled = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Order cancelled successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(
                    ex,
                    "Unable to cancel the order.");
            }
        }

        // ============================================================
        // UPDATE ORDER STATUS
        // ============================================================

        private async Task UpdateOrderStatus(OrderModel order)
        {
            var items = await _context.OrderItems
                .Where(i => i.OrderId == order.OrderId)
                .ToListAsync();

            if (!items.Any())
            {
                order.Status = OrderStatus.Ordered;
                return;
            }

            bool hasOrdered = items.Any(i =>
                i.Status == OrderItemStatus.Ordered);

            bool hasReceived = items.Any(i =>
                i.Status == OrderItemStatus.Received);

            bool hasCancelled = items.Any(i =>
                i.Status == OrderItemStatus.Cancelled);

            if (hasOrdered && !hasReceived && !hasCancelled)
            {
                order.Status = OrderStatus.Ordered;
                return;
            }

            if (hasOrdered && hasReceived)
            {
                order.Status = OrderStatus.PartiallyComplete;
                return;
            }

            if (!hasOrdered && (hasReceived || hasCancelled))
            {
                order.Status = OrderStatus.Complete;

                if (!order.DateComplete.HasValue)
                {
                    order.DateComplete = DateTime.Now;
                }

                return;
            }

            order.Status = OrderStatus.Ordered;
        }

        // ============================================================
        // VALIDATE CREATE
        // ============================================================

        private async Task ValidateCreateOrder(
            CreateOrderViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number is required.");
            }
            else if (model.OrderNumber.Length > 50)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number cannot exceed 50 characters.");
            }

            if (!string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                bool exists = await _context.Orders
                    .AnyAsync(o =>
                        o.OrderNumber.ToLower() ==
                        model.OrderNumber.ToLower());

                if (exists)
                {
                    ModelState.AddModelError(
                        nameof(model.OrderNumber),
                        "This order number already exists.");
                }
            }

            if (model.SupplierId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a valid supplier.");
            }
            else
            {
                bool supplierExists = await _context.Suppliers
                    .AnyAsync(s =>
                        s.SupplierId == model.SupplierId &&
                        !s.IsDeleted);

                if (!supplierExists)
                {
                    ModelState.AddModelError(
                        nameof(model.SupplierId),
                        "Please select a valid supplier.");
                }
            }

            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError(
                    nameof(model.Items),
                    "At least one consumable must be added.");
            }
        }

        // ============================================================
        // VALIDATE EDIT
        // ============================================================

        private async Task ValidateEditOrder(
            int id,
            OrderModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number is required.");
            }
            else if (model.OrderNumber.Length > 50)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Order number cannot exceed 50 characters.");
            }

            if (!string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                bool duplicate = await _context.Orders
                    .AnyAsync(o =>
                        o.OrderId != id &&
                        o.OrderNumber.ToLower() ==
                        model.OrderNumber.ToLower());

                if (duplicate)
                {
                    ModelState.AddModelError(
                        nameof(model.OrderNumber),
                        "This order number already exists.");
                }
            }

            if (model.SupplierId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a valid supplier.");

                return;
            }

            bool supplierExists = await _context.Suppliers
                .AnyAsync(s =>
                    s.SupplierId == model.SupplierId &&
                    !s.IsDeleted);

            if (!supplierExists)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select a valid supplier.");
            }
        }

        // ============================================================
        // LOAD ORDER DATA
        // ============================================================

        private async Task LoadOrderData()
        {
            ViewBag.Suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            ViewBag.Consumables = await _context.Consumables
                .AsNoTracking()
                .Include(c => c.Supplier)
                .Where(c =>
                    !c.IsDeleted &&
                    (c.StockStatus == StockStatus.ReorderNow ||
                     c.StockStatus == StockStatus.OutOfStock))
                .OrderBy(c =>
                    c.StockStatus == StockStatus.OutOfStock ? 0 : 1)
                .ThenBy(c => c.Name)
                .ToListAsync();

            ViewBag.ReorderConsumables =
                await GetReorderConsumables();
        }

        // ============================================================
        // GET ORDERS
        // Loads primary keys, foreign keys and related entities.
        // ============================================================

        private async Task<List<OrderModel>> GetOrders()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Consumable)
                .OrderByDescending(o => o.OrderId)
                .ToListAsync();
        }

        // ============================================================
        // GET REORDER CONSUMABLES
        // ============================================================

        private async Task<List<ConsumableModel>>
            GetReorderConsumables()
        {
            return await _context.Consumables
                .AsNoTracking()
                .Include(c => c.Supplier)
                .Where(c =>
                    !c.IsDeleted &&
                    (c.StockStatus == StockStatus.ReorderNow ||
                     c.StockStatus == StockStatus.OutOfStock))
                .OrderBy(c =>
                    c.StockStatus == StockStatus.OutOfStock ? 0 : 1)
                .ThenBy(c => c.QuantityOnHand)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        // ============================================================
        // CALCULATE STOCK STATUS
        // ============================================================

        private StockStatus CalculateStockStatus(
            int quantityOnHand,
            int reorderLevel)
        {
            if (quantityOnHand <= 0)
            {
                return StockStatus.OutOfStock;
            }

            decimal reorderThreshold = reorderLevel * 1.10m;

            if (quantityOnHand <= reorderThreshold)
            {
                return StockStatus.ReorderNow;
            }

            return StockStatus.Sufficient;
        }

        // ============================================================
        // GET ROOT EXCEPTION
        // ============================================================

        private Exception GetRootException(Exception exception)
        {
            var root = exception;

            while (root.InnerException != null)
            {
                root = root.InnerException;
            }

            return root;
        }

        // ============================================================
        // ERROR HANDLING
        // ============================================================

        private IActionResult HandleException(
            Exception ex,
            string userMessage)
        {
            var rootException = GetRootException(ex);

            Console.Error.WriteLine(
                "============================================================");
            Console.Error.WriteLine(userMessage);
            Console.Error.WriteLine(
                $"Exception: {ex.GetType().FullName}");
            Console.Error.WriteLine($"Message: {ex.Message}");
            Console.Error.WriteLine(
                $"Root Exception: {rootException.GetType().FullName}");
            Console.Error.WriteLine(
                $"Root Message: {rootException.Message}");
            Console.Error.WriteLine(
                $"Stack Trace:{Environment.NewLine}{ex.StackTrace}");
            Console.Error.WriteLine(
                "============================================================");

            TempData["ErrorMessage"] =
                $"{userMessage} {rootException.Message}";

            return RedirectToAction(nameof(Index));
        }
    }
}