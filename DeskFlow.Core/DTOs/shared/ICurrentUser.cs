using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.DTOs.shared
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Role { get; }
        int DeptId { get; }
    }
}
