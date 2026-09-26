using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Models.Laboratory;

namespace NMB_HLabSys.Services.Interfaces
{
    public interface ILabManagerService
    {
        Task<DashboardViewModel> GetDashboardAsync();

        // Categories
        Task<IReadOnlyList<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        // Sample Types
        Task<IReadOnlyList<SampleType>> GetSampleTypesAsync();
        Task<SampleType> GetSampleTypeAsync(int id);
        Task<SampleType> CreateSampleTypeAsync(SampleType sampleType);
        Task UpdateSampleTypeAsync(SampleType sampleType);
        Task DeleteSampleTypeAsync(int id);

        // Test Types
        Task<IReadOnlyList<TestType>> GetTestTypesAsync();
        Task<TestType> GetTestTypeAsync(int id);
        Task<TestType> CreateTestTypeAsync(TestType testType);
        Task UpdateTestTypeAsync(TestType testType);
        Task DeleteTestTypeAsync(int id);

        // Suppliers
        Task<IReadOnlyList<Supplier>> GetSuppliersAsync();
        Task<Supplier> GetSupplierAsync(int id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);

        // Consumables (catalogue)
        Task<IReadOnlyList<Consumable>> GetConsumablesAsync();
        Task<Consumable> GetConsumableAsync(int id);
        Task<Consumable> CreateConsumableAsync(Consumable consumable);
        Task UpdateConsumableAsync(Consumable consumable);
        Task DeleteConsumableAsync(int id);

        // Manage Consumables (stock)
        Task<IReadOnlyList<ManageConsumable>> GetManagedConsumablesAsync();
        Task<IReadOnlyList<ManageConsumable>> GetConsumablesBySupplierAsync(int supplierId);
        Task<ManageConsumable> CreateManagedConsumableAsync(ManageConsumable managed);

        Task UpdateManagedConsumableAsync(ManageConsumable managed);

        // Orders
        Task<IReadOnlyList<Order>> GetOrdersAsync(bool completedOnly = false);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> CreateOrderPreviewAsync(OrderPreviewViewModel preview);
        Task UpdateOrderStatusAsync(int orderId, Models.Inventory.Enums.OrderStatus status);
        Task CancelOrderAsync(int orderId, string reason);

        // Calendar
        Task<IReadOnlyList<CalendarEvent>> GetCalendarEventsAsync();
        Task<CalendarEvent> SaveCalendarEventAsync(CalendarEvent calendarEvent);
        Task DeleteCalendarEventAsync(int id);
    }
}
