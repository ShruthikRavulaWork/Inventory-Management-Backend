using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("least-stock-items")]
        public async Task<IActionResult> GetLeastStockItems()
        {
            var data = await _analyticsService.GetTop5LeastStockItemsAsync();
            return Ok(data);
        }

        [HttpGet("least-supplier-stock")]
        public async Task<IActionResult> GetLeastSupplierStock()
        {
            var data = await _analyticsService.GetTop5LeastSupplierStockAsync();
            return Ok(data);
        }
    }
}