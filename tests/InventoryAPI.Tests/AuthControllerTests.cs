using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using System.Threading.Tasks;

namespace InventoryAPI.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_WithNewUsername_ReturnsOkObjectResult()
        {
            // Arrange
            var registerDto = new UserRegisterDto { Username = "newUser", Password = "password" };
            var user = new User { UserID = 1, Username = "newUser", Role = "Supplier" };
            _authServiceMock.Setup(s => s.RegisterUser(registerDto)).ReturnsAsync(user);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Register_WithExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new UserRegisterDto { Username = "existingUser", Password = "password" };
            _authServiceMock.Setup(s => s.RegisterUser(registerDto)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Username already exists.");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkObjectResultWithToken()
        {
            // Arrange
            var loginDto = new UserLoginDto { Username = "test", Password = "password" };
            var authResponse = new AuthResponseDto { Token = "some.jwt.token", Username = "test", Role = "Admin" };
            _authServiceMock.Setup(s => s.AuthenticateUser(loginDto)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginDto = new UserLoginDto { Username = "wrong", Password = "user" };
            _authServiceMock.Setup(s => s.AuthenticateUser(loginDto)).ReturnsAsync((AuthResponseDto)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Value.Should().Be("Invalid credentials.");
        }
    }
}