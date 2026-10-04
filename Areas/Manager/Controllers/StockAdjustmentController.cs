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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _context.Orders
                    .AsNoTracking()
                    .Include(o => o.Supplier)
                    .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Consumable)
                    .OrderByDescending(o => o.OrderId)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"StockAdjustment Index Error: {ex}");

                TempData["ErrorMessage"] =
                    "Unable to load orders.";

                return View(new List<OrderModel>());
            }
        }


        // ============================================================
        // ORDER DETAILS
        // Used by the popup
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
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == id);

                if (order == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "The selected order could not be found."
                    });
                }


                // ----------------------------------------------------
                // Build a DTO instead of returning EF entities.
                //
                // This prevents circular-reference JSON problems
                // involving:
                //
                // Order -> OrderItems -> Order
                //
                // and:
                //
                // Consumable -> OrderItems -> Consumable
                // ----------------------------------------------------

                var result = new
                {
                    success = true,

                    order = new
                    {
                        orderId = order.OrderId,

                        orderNumber = order.OrderNumber,

                        supplierName =
                            order.Supplier?.SupplierName
                            ?? "Unknown supplier",

                        status =
                            (int)order.Status,

                        statusText =
                            GetOrderStatusText(order.Status),

                        items = order.OrderItems
                            .OrderBy(i => i.OrderItemId)
                            .Select(i => new
                            {
                                orderItemId =
                                    i.OrderItemId,

                                consumableId =
                                    i.ConsumableId,

                                consumableName =
                                    i.Consumable?.Name
                                    ?? "Unknown consumable",

                                quantityOrdered =
                                    i.QuantityOrdered,

                                quantityOnHand =
                                    i.Consumable?.QuantityOnHand
                                    ?? 0,

                                reorderLevel =
                                    i.Consumable?.ReorderLevel
                                    ?? 0,

                                stockStatus =
                                    i.Consumable != null
                                        ? (int)i.Consumable.StockStatus
                                        : 0,

                                status =
                                    (int)i.Status,

                                statusText =
                                    GetItemStatusText(i.Status),

                                dateReceived =
                                    i.DateReceived
                            })
                            .ToList()
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "==================================================");

                Console.Error.WriteLine(
                    "StockAdjustment Details Error");

                Console.Error.WriteLine(
                    $"Order ID: {id}");

                Console.Error.WriteLine(
                    ex.ToString());

                Console.Error.WriteLine(
                    "==================================================");


                return Json(new
                {
                    success = false,
                    message =
                        "Unable to load order details."
                });
            }
        }


        // ============================================================
        // DELIVER INDIVIDUAL ITEM
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliverItem(
            int id)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var item = await _context.OrderItems
                    .Include(i => i.Order)
                    .Include(i => i.Consumable)
                    .FirstOrDefaultAsync(i =>
                        i.OrderItemId == id);

                if (item == null)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "The selected order item could not be found."
                    });
                }


                if (item.Order == null)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "The order associated with this item could not be found."
                    });
                }


                if (item.Consumable == null)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "The consumable associated with this item could not be found."
                    });
                }


                if (item.Status !=
                    OrderItemStatus.Ordered)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "This item has already been delivered or cancelled."
                    });
                }


                if (item.Order.Status ==
                    OrderStatus.Cancelled)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Items from a cancelled order cannot be delivered."
                    });
                }


                // ====================================================
                // UPDATE INVENTORY
                // ====================================================

                checked
                {
                    item.Consumable.QuantityOnHand +=
                        item.QuantityOrdered;
                }


                // ====================================================
                // RECALCULATE STOCK STATUS
                // ====================================================

                item.Consumable.StockStatus =
                    CalculateStockStatus(
                        item.Consumable.QuantityOnHand,
                        item.Consumable.ReorderLevel);


                // ====================================================
                // MARK ITEM RECEIVED
                // ====================================================

                item.Status =
                    OrderItemStatus.Received;

                item.DateReceived =
                    DateTime.Now;


                // ====================================================
                // CREATE STOCK ADJUSTMENT RECORD
                // ====================================================

                var adjustment =
                    new StockAdjustmentModel
                    {
                        ConsumableId =
                            item.ConsumableId,

                        AdjustmentType =
                            StockAdjustmentType.Increase,

                        Quantity =
                            item.QuantityOrdered,

                        AdjustmentDate =
                            DateTime.Now
                    };

                _context.StockAdjustments.Add(
                    adjustment);


                // ====================================================
                // CREATE STOCK STATUS HISTORY
                // ====================================================

                var history =
                    new StockStatusHistoryModel
                    {
                        ConsumableId =
                            item.ConsumableId,

                        StockStatus =
                            item.Consumable.StockStatus,

                        QuantityOnHand =
                            item.Consumable.QuantityOnHand,

                        ReorderLevel =
                            item.Consumable.ReorderLevel,

                        DateRecorded =
                            DateTime.Now
                    };

                _context.StockStatusHistories.Add(
                    history);


                // ====================================================
                // UPDATE ORDER STATUS
                // ====================================================

                await UpdateOrderStatus(
                    item.Order);


                // ====================================================
                // SAVE
                // ====================================================

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                return Json(new
                {
                    success = true,
                    message =
                        "Item delivered and inventory updated successfully."
                });
            }
            catch (OverflowException)
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message =
                        "The resulting stock quantity is too large."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.Error.WriteLine(
                    "==================================================");

                Console.Error.WriteLine(
                    "DeliverItem Error");

                Console.Error.WriteLine(
                    $"Order Item ID: {id}");

                Console.Error.WriteLine(
                    ex.ToString());

                Console.Error.WriteLine(
                    "==================================================");

                return Json(new
                {
                    success = false,
                    message =
                        "Unable to deliver the selected item."
                });
            }
        }


        // ============================================================
        // COMPLETE ENTIRE ORDER
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder(
            int id)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Consumable)
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == id);

                if (order == null)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "The selected order could not be found."
                    });
                }


                if (order.Status ==
                    OrderStatus.Cancelled)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "A cancelled order cannot be completed."
                    });
                }


                if (order.Status ==
                    OrderStatus.Complete)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "This order has already been completed."
                    });
                }


                var pendingItems =
                    order.OrderItems
                        .Where(i =>
                            i.Status ==
                            OrderItemStatus.Ordered)
                        .ToList();


                if (!pendingItems.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "There are no outstanding items to deliver."
                    });
                }


                // ====================================================
                // PROCESS EVERY OUTSTANDING ITEM
                // ====================================================

                foreach (var item in pendingItems)
                {
                    if (item.Consumable == null)
                    {
                        throw new InvalidOperationException(
                            $"Consumable for order item " +
                            $"{item.OrderItemId} could not be found.");
                    }


                    // ------------------------------------------------
                    // UPDATE QUANTITY
                    // ------------------------------------------------

                    checked
                    {
                        item.Consumable.QuantityOnHand +=
                            item.QuantityOrdered;
                    }


                    // ------------------------------------------------
                    // UPDATE STOCK STATUS
                    // ------------------------------------------------

                    item.Consumable.StockStatus =
                        CalculateStockStatus(
                            item.Consumable.QuantityOnHand,
                            item.Consumable.ReorderLevel);


                    // ------------------------------------------------
                    // UPDATE ITEM
                    // ------------------------------------------------

                    item.Status =
                        OrderItemStatus.Received;

                    item.DateReceived =
                        DateTime.Now;


                    // ------------------------------------------------
                    // STOCK ADJUSTMENT
                    // ------------------------------------------------

                    _context.StockAdjustments.Add(
                        new StockAdjustmentModel
                        {
                            ConsumableId =
                                item.ConsumableId,

                            AdjustmentType =
                                StockAdjustmentType.Increase,

                            Quantity =
                                item.QuantityOrdered,

                            AdjustmentDate =
                                DateTime.Now
                        });


                    // ------------------------------------------------
                    // STOCK HISTORY
                    // ------------------------------------------------

                    _context.StockStatusHistories.Add(
                        new StockStatusHistoryModel
                        {
                            ConsumableId =
                                item.ConsumableId,

                            StockStatus =
                                item.Consumable.StockStatus,

                            QuantityOnHand =
                                item.Consumable.QuantityOnHand,

                            ReorderLevel =
                                item.Consumable.ReorderLevel,

                            DateRecorded =
                                DateTime.Now
                        });
                }


                // ====================================================
                // ORDER IS NOW COMPLETE
                // ====================================================

                order.Status =
                    OrderStatus.Complete;

                order.DateComplete =
                    DateTime.Now;


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                return Json(new
                {
                    success = true,
                    message =
                        "Entire order delivered and inventory updated successfully."
                });
            }
            catch (OverflowException)
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message =
                        "The resulting stock quantity is too large."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.Error.WriteLine(
                    "==================================================");

                Console.Error.WriteLine(
                    "CompleteOrder Error");

                Console.Error.WriteLine(
                    $"Order ID: {id}");

                Console.Error.WriteLine(
                    ex.ToString());

                Console.Error.WriteLine(
                    "==================================================");

                return Json(new
                {
                    success = false,
                    message =
                        "Unable to complete the order."
                });
            }
        }


        // ============================================================
        // UPDATE ORDER STATUS
        // ============================================================

        private async Task UpdateOrderStatus(
            OrderModel order)
        {
            var items = await _context.OrderItems
                .Where(i =>
                    i.OrderId ==
                    order.OrderId)
                .ToListAsync();


            if (!items.Any())
            {
                order.Status =
                    OrderStatus.Ordered;

                return;
            }


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


            // --------------------------------------------------------
            // NOTHING HAS BEEN DELIVERED
            // --------------------------------------------------------

            if (hasOrdered &&
                !hasReceived &&
                !hasCancelled)
            {
                order.Status =
                    OrderStatus.Ordered;

                return;
            }


            // --------------------------------------------------------
            // SOME DELIVERED, SOME STILL ORDERED
            // --------------------------------------------------------

            if (hasOrdered &&
                hasReceived)
            {
                order.Status =
                    OrderStatus.PartiallyComplete;

                return;
            }


            // --------------------------------------------------------
            // NO ITEMS REMAIN ORDERED
            // --------------------------------------------------------

            if (!hasOrdered &&
                (hasReceived ||
                 hasCancelled))
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


            order.Status =
                OrderStatus.Ordered;
        }


        // ============================================================
        // STOCK STATUS
        // ============================================================

        private StockStatus CalculateStockStatus(
            int quantityOnHand,
            int reorderLevel)
        {
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


        // ============================================================
        // ORDER STATUS TEXT
        // ============================================================

        private string GetOrderStatusText(
            OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Ordered =>
                    "Ordered",

                OrderStatus.PartiallyComplete =>
                    "Partially Delivered",

                OrderStatus.Complete =>
                    "Delivered",

                OrderStatus.Cancelled =>
                    "Cancelled",

                _ =>
                    "Unknown"
            };
        }


        // ============================================================
        // ITEM STATUS TEXT
        // ============================================================

        private string GetItemStatusText(
            OrderItemStatus status)
        {
            return status switch
            {
                OrderItemStatus.Ordered =>
                    "Ordered",

                OrderItemStatus.Received =>
                    "Delivered",

                OrderItemStatus.Cancelled =>
                    "Cancelled",

                _ =>
                    "Unknown"
            };
        }
    }
}
