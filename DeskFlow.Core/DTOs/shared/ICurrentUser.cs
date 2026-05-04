using DeskFlow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.shared
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        RoleEnum Role { get; }
        int DeptId { get; }
    }
}
