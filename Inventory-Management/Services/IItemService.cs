using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public interface IItemService
    {
        Task<PagedResult<Item>> GetAllItemsAsync(int pageNumber, int pageSize, string? searchTerm, string? searchField);
        Task<Item?> GetItemByIdAsync(int id);
        Task<int> CreateItemAsync(ItemCreateDto itemDto);
        Task<bool> UpdateItemAsync(int id, ItemUpdateDto itemDto);
        Task<bool> DeleteItemAsync(int id);
        Task<PagedResult<Item>> GetItemsBySupplierAsync(int supplierId, int pageNumber, int pageSize, string? searchTerm);
        Task<bool> UpdateSupplierItemAsync(int itemId, int supplierId, SupplierItemUpdateDto itemDto);
        Task<IEnumerable<object>> GetAllSuppliersAsync();
    }
}