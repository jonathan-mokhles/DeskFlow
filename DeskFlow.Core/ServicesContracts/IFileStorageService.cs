using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public interface IFileStorageService
    {
        public Task<string> SaveFileAsync(IFormFile file, int ticketId);
        Task DeleteFileAsync(string filePath);
        Task<(Stream FileStream, string MimeType)> GetFileAsync(string filePath);

    }
}
