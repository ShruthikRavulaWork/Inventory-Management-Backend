using InventoryAPI.DTOs;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("suppliers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _itemService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] string searchField = "ItemName")
        {
            var result = await _itemService.GetAllItemsAsync(pageNumber, pageSize, searchTerm, searchField);
            return Ok(result);
        }

        [HttpGet("supplier")]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> GetSupplierItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var supplierId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _itemService.GetItemsBySupplierAsync(supplierId, pageNumber, pageSize, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItem([FromForm] ItemCreateDto itemDto)
        {
            try
            {
                var itemId = await _itemService.CreateItemAsync(itemDto);
                var newItem = await _itemService.GetItemByIdAsync(itemId);
                return CreatedAtAction(nameof(GetItemById), new
                {
                    id = itemId
                }, newItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, [FromForm] ItemUpdateDto itemDto)
        {
            try
            {
                var success = await _itemService.UpdateItemAsync(id, itemDto);
                if (!success)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("supplier/{id}")]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> UpdateSupplierItem(int id, [FromBody] SupplierItemUpdateDto itemDto)
        {
            var supplierId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var success = await _itemService.UpdateSupplierItemAsync(id, supplierId, itemDto);
            if (!success)
                return Forbid("You can only update your own items.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var success = await _itemService.DeleteItemAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}