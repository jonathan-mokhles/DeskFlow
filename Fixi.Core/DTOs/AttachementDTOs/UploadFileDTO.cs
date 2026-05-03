using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.DTOs.AttachementDTOs
{
    public class UploadFileDTO
    {
        public IFormFile? File { get; set; }
    }
}
