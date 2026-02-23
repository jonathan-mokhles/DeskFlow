using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.shared
{
    public  class ApiErrorResponse
    {
        public string Message { get; set; } = default!;
        public List<string> Errors { get; set; } = new List<string>();
        public string TraceId { get; set; } = default!;

    }
}
