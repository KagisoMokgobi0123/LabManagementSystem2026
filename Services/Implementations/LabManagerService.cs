using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Models.Inventory.Enums;
using NMB_HLabSys.Models.Laboratory;
using NMB_HLabSys.Models.Laboratory.Enums;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NMB_HLabSys.Services.Implementations
{
    public class LabManagerService : ILabManagerService
    {
        private readonly ApplicationDbContext _db;

        public LabManagerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            return new DashboardViewModel
            {
                TotalUsers = await _db.Users.CountAsync(),
                TotalConsumables = await _db.Consumables.CountAsync(),
                PendingOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
                LowStockItems = await _db.ManageConsumables
                    .CountAsync(mc => mc.QuantityOnHand <= mc.ReorderLevel),
                RecentUsers = await _db.Users
                    .OrderByDescending(u => u.CreatedDate)
                    .Take(5)
                    .ToListAsync()
            };
        }

        // ---------- Categories ----------
        public async Task<IReadOnlyList<Category>> GetCategoriesAsync() =>
            await _db.Categories.OrderBy(c => c.Name).ToListAsync();

        public async Task<Category> GetCategoryAsync(int id) =>
            await _db.Categories.FindAsync(id);

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity == null) return;

            entity.Status = CategoryStatus.Deleted;
            await _db.SaveChangesAsync();
        }

        // ---------- Sample Types ----------
        public async Task<IReadOnlyList<SampleType>> GetSampleTypesAsync() =>
            await _db.SampleTypes.OrderBy(s => s.Name).ToListAsync();

        public async Task<SampleType> GetSampleTypeAsync(int id) =>
            await _db.SampleTypes.FindAsync(id);

        public async Task<SampleType> CreateSampleTypeAsync(SampleType sampleType)
        {
            _db.SampleTypes.Add(sampleType);
            await _db.SaveChangesAsync();
            return sampleType;
        }

        public async Task UpdateSampleTypeAsync(SampleType sampleType)
        {
            _db.SampleTypes.Update(sampleType);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSampleTypeAsync(int id)
        {
            var entity = await _db.SampleTypes.FindAsync(id);
            if (entity == null) return;

            entity.Status = SampleTypeStatus.Deleted;
            await _db.SaveChangesAsync();
        }

        // ---------- Test Types ----------
        public async Task<IReadOnlyList<TestType>> GetTestTypesAsync() =>
            await _db.TestTypes
                .Include(t => t.Category)
                .Include(t => t.SampleType)
                .OrderBy(t => t.Name)
                .ToListAsync();

        public async Task<TestType> GetTestTypeAsync(int id) =>
            await _db.TestTypes
                .Include(t => t.Category)
                .Include(t => t.SampleType)
                .FirstOrDefaultAsync(t => t.TestTypeId == id);

        public async Task<TestType> CreateTestTypeAsync(TestType testType)
        {
            _db.TestTypes.Add(testType);
            await _db.SaveChangesAsync();
            return testType;
        }

        public async Task UpdateTestTypeAsync(TestType testType)
        {
            _db.TestTypes.Update(testType);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTestTypeAsync(int id)
        {
            var entity = await _db.TestTypes.FindAsync(id);
            if (entity == null) return;

            entity.Status = TestTypeStatus.Deleted;
            await _db.SaveChangesAsync();
        }

        // ---------- Suppliers ----------
        public async Task<IReadOnlyList<Supplier>> GetSuppliersAsync() =>
            await _db.Suppliers.OrderBy(s => s.Name).ToListAsync();

        public async Task<Supplier> GetSupplierAsync(int id) =>
            await _db.Suppliers.FindAsync(id);

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            _db.Suppliers.Add(supplier);
            await _db.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            _db.Suppliers.Update(supplier);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var entity = await _db.Suppliers.FindAsync(id);
            if (entity == null) return;

            entity.Status = SupplierStatus.Deleted;
            await _db.SaveChangesAsync();
        }

        // ---------- Consumables (catalogue) ----------
        public async Task<IReadOnlyList<Consumable>> GetConsumablesAsync() =>
            await _db.Consumables.Include(c => c.Category).OrderBy(c => c.Name).ToListAsync();

        public async Task<Consumable> GetConsumableAsync(int id) =>
            await _db.Consumables.Include(c => c.Category).FirstOrDefaultAsync(c => c.ConsumableId == id);

        public async Task<Consumable> CreateConsumableAsync(Consumable consumable)
        {
            _db.Consumables.Add(consumable);
            await _db.SaveChangesAsync();
            return consumable;
        }

        public async Task UpdateConsumableAsync(Consumable consumable)
        {
            _db.Consumables.Update(consumable);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteConsumableAsync(int id)
        {
            var entity = await _db.Consumables.FindAsync(id);
            if (entity == null) return;

            entity.Status = ConsumableStatus.Deleted;
            await _db.SaveChangesAsync();
        }

        // ---------- Manage Consumables (stock) ----------
        public async Task<IReadOnlyList<ManageConsumable>> GetManagedConsumablesAsync() =>
            await _db.ManageConsumables
                .Include(mc => mc.Consumable)
                .Include(mc => mc.Supplier)
                .OrderBy(mc => mc.Consumable.Name)
                .ToListAsync();
        public async Task<IReadOnlyList<ManageConsumable>> GetConsumablesBySupplierAsync(int supplierId)
        {
            return await _db.ManageConsumables
                .Include(mc => mc.Consumable)
                .Where(mc => mc.SupplierId == supplierId)
                .OrderBy(mc => mc.Consumable.Name)
                .ToListAsync();
        }
        public async Task<ManageConsumable> CreateManagedConsumableAsync(ManageConsumable managed)
        {
            var exists = await _db.ManageConsumables.AnyAsync(mc =>
                mc.ConsumableId == managed.ConsumableId &&
                mc.SupplierId == managed.SupplierId);

            if (exists)
                throw new InvalidOperationException("This consumable is already being managed by the selected supplier.");

            managed.DateUpdated = DateTime.UtcNow;

            _db.ManageConsumables.Add(managed);

            await _db.SaveChangesAsync();

            return managed;
        }
        public async Task UpdateManagedConsumableAsync(ManageConsumable managed)
        {
            var existing = await _db.ManageConsumables
                .FirstOrDefaultAsync(mc => mc.ManageConsumableId == managed.ManageConsumableId);

            if (existing == null)
                return;

            existing.SupplierId = managed.SupplierId;
            existing.ConsumableId = managed.ConsumableId;
            existing.ReorderLevel = managed.ReorderLevel;
            existing.QuantityOnHand = managed.QuantityOnHand;
            existing.Status = managed.Status;
            existing.IsActive = managed.IsActive;
            existing.DateUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        // ---------- Orders ----------
        public async Task<IReadOnlyList<Order>> GetOrdersAsync(bool completedOnly = false)
        {
            var query = _db.Orders
                .Include(o => o.Consumable)
                .Include(o => o.Supplier)
                .AsQueryable();

            if (completedOnly)
                query = query.Where(o => o.Status == OrderStatus.Completed);

            return await query.OrderByDescending(o => o.OrderedDate).ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order> CreateOrderPreviewAsync(OrderPreviewViewModel preview)
        {
            // Each line item in the preview cart becomes its own Order row
            // against the chosen supplier, mirroring the manageconsumables.html workflow.
            Order last = null;
            foreach (var item in preview.Items)
            {
                last = new Order
                {
                    ConsumableId = item.ConsumableId,
                    SupplierId = preview.SupplierId,
                    QtyOrdered = item.Quantity,
                    OrderedDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    DeliveryStatus = DeliveryStatus.AwaitingDelivery
                };
                _db.Orders.Add(last);
            }
            await _db.SaveChangesAsync();
            return last;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) return;
            order.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task CancelOrderAsync(int orderId, string reason)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) return;
            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = reason;
            order.CancellationDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        // ---------- Calendar ----------
        public async Task<IReadOnlyList<CalendarEvent>> GetCalendarEventsAsync() =>
            await _db.CalendarEvents.OrderBy(e => e.EventDate).ToListAsync();

        public async Task<CalendarEvent> SaveCalendarEventAsync(CalendarEvent calendarEvent)
        {
            if (calendarEvent.CalenderId == 0)
            {
                _db.CalendarEvents.Add(calendarEvent);
            }
            else
            {
                _db.CalendarEvents.Update(calendarEvent);
            }
            await _db.SaveChangesAsync();
            return calendarEvent;
        }

        public async Task DeleteCalendarEventAsync(int id)
        {
            var entity = await _db.CalendarEvents.FindAsync(id);
            if (entity == null) return;
            _db.CalendarEvents.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
