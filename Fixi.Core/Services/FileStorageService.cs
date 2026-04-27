using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Fixi.Core.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task DeleteFileAsync(string filePath)
        {
            string rootPath = _env.WebRootPath ?? _env.ContentRootPath;
            string physicalPath = Path.Combine(rootPath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            return Task.CompletedTask;
        }

        public Task<(Stream FileStream, string MimeType)> GetFileAsync(string filePath)
        {
            string rootPath = _env.WebRootPath ?? _env.ContentRootPath;
            string physicalPath = Path.Combine(rootPath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(physicalPath))
            {
                throw new FileNotFoundException("File not found.", physicalPath);
            }

            var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            string extension = Path.GetExtension(physicalPath);
            string mimeType = GetMimeType(extension);

            return Task.FromResult(((Stream)stream, mimeType));
        }


        public Task<string> SaveFileAsync(IFormFile file, int ticketId)
        {
            List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".xlsx" };
            string extension = Path.GetExtension(file.FileName);
            if(extension == null || !allowedExtensions.Contains(extension.ToLower()))
            {
                throw new InvalidOperationException("Invalid file type. Allowed types are: " + string.Join(", ", allowedExtensions));
            }
            var sizeLimit = 10 * 1024 * 1024; // 10 MB
            if(file.Length > sizeLimit)
            {
                throw new InvalidOperationException("File size exceeds the limit of 10 MB.");
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "Attachments", ticketId.ToString());

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string physicalPath = Path.Combine(uploadsFolder, fileName);
            string filePath = $"Attachments/{ticketId}/{fileName}";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Task.FromResult(filePath);
        }


        private string GetMimeType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream",
            };
        }
    }
}
