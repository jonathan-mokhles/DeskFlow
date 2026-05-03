using DeskFkow.Core.DTOs.shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.DTOs.UsersDTOs
{
    public class UsersQueryParameters: PaginationParams
    {
        public string? Name { get; set; }
        public int? DepartmentId { get; set; }
    }
}
