using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.shared
{
    public record ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public string TraceId { get; set; } = string.Empty;

    }
}
