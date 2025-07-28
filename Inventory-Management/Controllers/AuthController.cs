using InventoryAPI.DTOs;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var user = await _authService.RegisterUser(request);
            if (user == null)
            {
                return BadRequest("Username already exists.");
            }
            return Ok(new { user.UserID, user.Username, user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var authResponse = await _authService.AuthenticateUser(request);
            if (authResponse == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(authResponse);
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin(UserRegisterDto request)
        {

            var user = await _authService.CreateAdmin(request);
            if (user == null) return BadRequest("Could not create admin.");
            return Ok(new { message = $"Admin user '{user.Username}' created/updated successfully." });
        }
    }
}