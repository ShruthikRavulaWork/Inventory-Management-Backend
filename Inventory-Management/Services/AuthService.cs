using Dapper;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbService _dbService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IDbService dbService, IConfiguration config, ILogger<AuthService> logger)
        {
            _dbService = dbService;
            _config = config;
            _logger = logger;
        }

        public async Task<User?> RegisterUser(UserRegisterDto userDto)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                var parameters = new
                {
                    Username = userDto.Username,
                    PasswordHash = passwordHash
                };

                var result = await connection.QuerySingleOrDefaultAsync<User>("sp_RegisterUser", parameters, commandType: CommandType.StoredProcedure);

                if (result != null && result.UserID == -1)
                    return null; 
                _logger.LogInformation("User {Username} registered successfully.", userDto.Username);
                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while registering user {Username}.", userDto.Username);
                throw; 
            }
        }

        public async Task<User?> CreateAdmin(UserRegisterDto userDto)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                var parameters = new
                {
                    Username = userDto.Username,
                    PasswordHash = passwordHash
                };
                var user = await connection.QuerySingleOrDefaultAsync<User>("sp_CreateAdmin", parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Admin user {Username} created or updated.", userDto.Username);
                return user;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred while creating admin user {Username}.", userDto.Username);
                throw;
            }
        }

        public async Task<AuthResponseDto?> AuthenticateUser(UserLoginDto userDto)
        {
            try
            {
                using var connection = _dbService.CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<User>("sp_GetUserByUsername", new
                {
                    Username = userDto.Username
                }, commandType: CommandType.StoredProcedure);

                if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user {Username}.", userDto.Username);
                    return null;
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {Username} authenticated successfully.", user.Username);
                return new AuthResponseDto { Token = token, Username = user.Username, Role = user.Role };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A database error occurred during login attempt for user {Username}.", userDto.Username);
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}