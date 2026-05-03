using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.DTOs.shared
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
