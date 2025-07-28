using Dapper;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IDbService _dbService;
        private readonly IFileService _fileService;
        private readonly ILogger<ItemService> _logger;

        public ItemService(IDbService dbService, IFileService fileService, ILogger<ItemService> logger)
        {
            _dbService = dbService;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IEnumerable<object>> GetAllSuppliersAsync()
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                return await connection.QueryAsync("sp_GetAllSuppliers", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while fetching all suppliers.");
                throw;
            }
        }

        public async Task<int> CreateItemAsync(ItemCreateDto itemDto)
        {
            var imagePath = itemDto.Image != null ? await _fileService.SaveImageAsync(itemDto.Image) : null;
            try
            {
                using var connection = _dbService.CreateConnection();
                var parameters = new
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Quantity = itemDto.Quantity,
                    ImagePath = imagePath,
                    SupplierID = itemDto.SupplierID
                };
                var itemId = await connection.QuerySingleAsync<int>("sp_CreateItem", parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully created item with ID {ItemID}", itemId);
                return itemId;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while creating item {ItemName}.", itemDto.Name);
                if (imagePath != null)
                    _fileService.DeleteImage(imagePath); 
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var item = await GetItemByIdAsync(id); 
                if (item?.ImagePath != null)
                {
                    _fileService.DeleteImage(item.ImagePath);
                }
                var affectedRows = await connection.ExecuteAsync("sp_DeleteItem", new
                {
                    ItemID = id
                }, commandType: CommandType.StoredProcedure);
                if (affectedRows > 0)
                    _logger.LogInformation("Deleted item with ID {ItemID}", id);
                return affectedRows > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while deleting item with ID {ItemID}.", id);
                throw;
            }
        }

        public async Task<PagedResult<Item>> GetAllItemsAsync(int pageNumber, int pageSize, string? searchTerm, string? searchField)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var parameters = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SearchField = searchField
                };
                var items = await connection.QueryAsync<Item>("sp_GetAllItems", parameters, commandType: CommandType.StoredProcedure);
                return new PagedResult<Item> { Items = items.ToList(), TotalCount = items.FirstOrDefault()?.TotalCount ?? 0 };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while fetching all items.");
                throw;
            }
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<Item>("sp_GetItemByID", new
                {
                    ItemID = id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while fetching item with ID {ItemID}.", id);
                throw;
            }
        }

        public async Task<PagedResult<Item>> GetItemsBySupplierAsync(int supplierId, int pageNumber, int pageSize, string? searchTerm)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var parameters = new
                {
                    SupplierID = supplierId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm
                };
                var items = await connection.QueryAsync<Item>("sp_GetItemsBySupplierID", parameters, commandType: CommandType.StoredProcedure);
                return new PagedResult<Item> { Items = items.ToList(), TotalCount = items.FirstOrDefault()?.TotalCount ?? 0 };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while fetching items for supplier ID {SupplierID}.", supplierId);
                throw;
            }
        }

        public async Task<bool> UpdateItemAsync(int id, ItemUpdateDto itemDto)
        {
            var existingItem = await GetItemByIdAsync(id);
            if (existingItem == null)
                return false;

            var imagePath = existingItem.ImagePath;
            if (itemDto.Image != null)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    _fileService.DeleteImage(imagePath);
                }
                imagePath = await _fileService.SaveImageAsync(itemDto.Image);
            }

            try
            {
                using var connection = _dbService.CreateConnection();
                var parameters = new
                {
                    ItemID = id,
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Quantity = itemDto.Quantity,
                    ImagePath = imagePath,
                    SupplierID = itemDto.SupplierID
                };
                var affectedRows = await connection.ExecuteAsync("sp_UpdateItem", parameters, commandType: CommandType.StoredProcedure);
                if (affectedRows > 0)
                    _logger.LogInformation("Updated item with ID {ItemID}", id);
                return affectedRows > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while updating item with ID {ItemID}.", id);
                if (itemDto.Image != null && imagePath != null)
                    _fileService.DeleteImage(imagePath); // Cleanup
                throw;
            }
        }

        public async Task<bool> UpdateSupplierItemAsync(int itemId, int supplierId, SupplierItemUpdateDto itemDto)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var parameters = new
                {
                    ItemID = itemId,
                    SupplierID = supplierId,
                    Price = itemDto.Price,
                    Quantity = itemDto.Quantity
                };
                var result = await connection.QuerySingleAsync<int>("sp_UpdateItemPriceAndQuantity", parameters, commandType: CommandType.StoredProcedure);
                if (result > 0)
                    _logger.LogInformation("Supplier {SupplierID} updated price/quantity for item {ItemID}", supplierId, itemId);
                return result > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while supplier {SupplierID} attempted to update item {ItemID}.", supplierId, itemId);
                throw;
            }
        }
    }
}