using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.shared
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Role { get; }
        int DeptId { get; }
    }
}
