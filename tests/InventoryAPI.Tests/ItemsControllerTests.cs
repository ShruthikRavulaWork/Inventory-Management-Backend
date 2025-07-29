using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InventoryAPI.Tests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemService> _itemServiceMock;
        private readonly ItemsController _controller;

        public ItemsControllerTests()
        {
            _itemServiceMock = new Mock<IItemService>();
            _controller = new ItemsController(_itemServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetAllItems_WhenCalled_ReturnsOkObjectResultWithPagedItems()
        {
            // Arrange
            var pagedResult = new PagedResult<Item>
            {
                Items = new List<Item> { new Item(), new Item() },
                TotalCount = 2
            };
            _itemServiceMock.Setup(s => s.GetAllItemsAsync(1, 10, null, "ItemName")).ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllItems(1, 10, null, "ItemName");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(pagedResult);
        }

        [Fact]
        public async Task GetItemById_WithExistingId_ReturnsOkObjectResultWithItem()
        {
            // Arrange
            var item = new Item { ItemID = 1, Name = "Test Item" };
            _itemServiceMock.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync(item);

            // Act
            var result = await _controller.GetItemById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(item);
        }

        [Fact]
        public async Task GetItemById_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            _itemServiceMock.Setup(s => s.GetItemByIdAsync(99)).ReturnsAsync((Item)null);

            // Act
            var result = await _controller.GetItemById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateItem_WithValidData_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var itemDto = new ItemCreateDto { Name = "New Item" };
            var newItem = new Item { ItemID = 10, Name = "New Item" };
            _itemServiceMock.Setup(s => s.CreateItemAsync(itemDto)).ReturnsAsync(10);
            _itemServiceMock.Setup(s => s.GetItemByIdAsync(10)).ReturnsAsync(newItem);

            // Act
            var result = await _controller.CreateItem(itemDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Value.Should().BeEquivalentTo(newItem);
        }

        [Fact]
        public async Task CreateItem_WithInvalidFileType_ReturnsBadRequest()
        {
            // Arrange
            var itemDto = new ItemCreateDto();
            _itemServiceMock.Setup(s => s.CreateItemAsync(itemDto))
                .ThrowsAsync(new System.ArgumentException("Only .png files are allowed."));

            // Act
            var result = await _controller.CreateItem(itemDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            // Anonymous types are useful for checking JSON results
            badRequestResult.Value.Should().BeEquivalentTo(new { message = "Only .png files are allowed." });
        }

        [Fact]
        public async Task UpdateItem_WithExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var itemDto = new ItemUpdateDto();
            _itemServiceMock.Setup(s => s.UpdateItemAsync(1, itemDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateItem(1, itemDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateItem_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var itemDto = new ItemUpdateDto();
            _itemServiceMock.Setup(s => s.UpdateItemAsync(99, itemDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateItem(99, itemDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteItem_WithExistingId_ReturnsNoContentResult()
        {
            // Arrange
            _itemServiceMock.Setup(s => s.DeleteItemAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteItem(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItem_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            _itemServiceMock.Setup(s => s.DeleteItemAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteItem(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}