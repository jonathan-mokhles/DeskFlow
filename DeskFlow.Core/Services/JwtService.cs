using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.DTOs.AccountDTOs;
using DeskFlow.Core.ServicesContracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DeskFlow.Core.Settings;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace DeskFlow.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwt;
        private readonly IIdentityService _identityService;
        public JwtService(IOptions<JwtSettings> jwt, IIdentityService identityService)
        {
            _jwt = jwt.Value;
            _identityService = identityService;
        }

        public async Task<AuthResponseDTO> GenerateToken(ApplicationUser user,IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));
            ArgumentNullException.ThrowIfNull(roles, nameof(roles));
            ArgumentException.ThrowIfNullOrEmpty(user.Id, nameof(user.Id));
            ArgumentException.ThrowIfNullOrEmpty(user.Email, nameof(user.Email));
            ArgumentException.ThrowIfNullOrEmpty(user.DepartmentId.ToString(), nameof(user.DepartmentId));
            ArgumentException.ThrowIfNullOrEmpty(_jwt.SecretKey, nameof(_jwt.SecretKey));
            ArgumentException.ThrowIfNullOrEmpty(_jwt.TokenDurationInMinutes.ToString(), nameof(_jwt.TokenDurationInMinutes));
            ArgumentException.ThrowIfNullOrEmpty(_jwt.RefreshTokenDurationInMinutes.ToString(), nameof(_jwt.RefreshTokenDurationInMinutes));

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id), // subject is userId	
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Json token Id
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()?? "User"),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("DeptId", user.DepartmentId.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));  
            var expiration = DateTime.UtcNow.AddMinutes(_jwt.TokenDurationInMinutes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var JwtToken = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenDurationInMinutes);
            await _identityService.UpdateUserAsync(user);
            
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
            ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
            ArgumentException.ThrowIfNullOrEmpty(_jwt.SecretKey, nameof(_jwt.SecretKey));

            var tokenValticketIdationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)),
                ValidateLifetime = false 
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claims =  tokenHandler.ValidateToken(token, tokenValticketIdationParameters, out SecurityToken securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ValidationException("Invalid token");
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
