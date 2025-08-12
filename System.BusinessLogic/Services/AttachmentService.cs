using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.BusinessLogic.InterFaces;

namespace System.BusinessLogic.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly List<string> allowedExtensions = new List<string> { ".png", ".jpg", ".jpeg" };
        private const int maxSize = 2_097_152; 

        public async Task<string?> UploadAsync(IFormFile file, string folderName)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return null;

            if (file.Length == 0 || file.Length > maxSize)
                return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var FolderPath = Path.Combine("Files", folderName, fileName).Replace("\\", "/");
            return FolderPath;
        }
    }
}
