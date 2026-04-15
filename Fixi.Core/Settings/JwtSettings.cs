using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int TokenDurationInMinutes { get; set; }
        public int RefreshTokenDurationInMinutes { get; set; }
    }
}
