using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace DeskFlow.Core.DTOs.shared
{
    public class PaginationParams
    {
        [Range(1,100 , ErrorMessage = "Page number must be between 1 and 100.")]
        public int PageNumber { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
