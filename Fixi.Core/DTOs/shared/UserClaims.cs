using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.shared
{
    public class UserClaims
    {
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int DeptId { get; set; }

    }
}
