using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.AccountDTOs;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Fixi.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<AuthResponseDTO> GenerateToken(ApplicationUser user)
        {
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // subject is userID	
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Json token id
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
            new Claim(ClaimTypes.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])); // Secret key from Secrets Manager 
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var JwtToken = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["RefreshToken:DurationInMinutes"]));
            await _userManager.UpdateAsync(user);
            
            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email!,
                Token = token,
                Expiration = expiration,
                Role = roles.FirstOrDefault() ?? "User",
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiration = user.RefreshTokenExpiryTime
            };

        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])),
                ValidateLifetime = false 
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claims =  tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return claims;
        }




        //Generates a Base64 random refresh token.
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
