using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduPlanManager.Services
{

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public TokenService(IConfiguration configuration, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public bool IsAccessTokenExpired(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;
            var expirationDate = jsonToken?.ValidTo;
            return expirationDate.HasValue && expirationDate.Value < DateTime.UtcNow;
        }
        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null && user.RefreshTokenExpiryTime > DateTime.Now)
            {
                return GenerateAccessToken(user);
            }
            return null;
        }
        public string GenerateAccessToken(User user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var role = roles.FirstOrDefault() ?? "";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)            
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
