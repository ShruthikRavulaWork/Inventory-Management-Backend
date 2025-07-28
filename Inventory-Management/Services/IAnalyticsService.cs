using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IAnalyticsService
    {
        Task<IEnumerable<LeastStockItemDto>> GetTop5LeastStockItemsAsync();
        Task<IEnumerable<SupplierStockDto>> GetTop5LeastSupplierStockAsync();
    }
}