using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogViewer.Core.DTOs;
using LogViewer.Core.Interfaces;
using LogViewer.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LogViewer.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                return null;

            if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user.Id, user.Username, user.Email);
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Username = user.Username,
                Email = user.Email
            };
        }

        private string GenerateJwtToken(long userId, string username, string email)
        {
            var secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret is not configured");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
