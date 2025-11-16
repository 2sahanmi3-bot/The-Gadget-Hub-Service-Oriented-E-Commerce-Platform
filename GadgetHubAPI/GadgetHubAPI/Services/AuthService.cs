using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GadgetHubAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GadgetHubAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _cfg;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(IConfiguration cfg) { _cfg = cfg; }

        public string HashPassword(User user, string password) =>
            _hasher.HashPassword(user, password);

        public bool VerifyPassword(User user, string password) =>
            _hasher.VerifyHashedPassword(user, user.PasswordHash, password)
                is PasswordVerificationResult.Success;

        public string CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiryMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
