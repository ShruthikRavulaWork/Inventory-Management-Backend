using Dapper;
using InventoryAPI.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IDbService _dbService;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(IDbService dbService, ILogger<AnalyticsService> logger)
        {
            _dbService = dbService;
            _logger = logger;
        }

        public async Task<IEnumerable<LeastStockItemDto>> GetTop5LeastStockItemsAsync()
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                return await connection.QueryAsync<LeastStockItemDto>(
                    "sp_GetTopLeastStockItems",
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching least stock items.");
                throw;
            }
        }

        public async Task<IEnumerable<SupplierStockDto>> GetTop5LeastSupplierStockAsync()
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                return await connection.QueryAsync<SupplierStockDto>(
                    "sp_GetTopLeastSupplierStock",
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching supplier stock data.");
                throw;
            }
        }
    }
}