using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.AccountDTOs
{
    public record AuthResponseDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
