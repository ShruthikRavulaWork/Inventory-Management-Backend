using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterUser(UserRegisterDto userDto);
        Task<User?> CreateAdmin(UserRegisterDto userDto);
        Task<AuthResponseDto?> AuthenticateUser(UserLoginDto userDto);
    }
}