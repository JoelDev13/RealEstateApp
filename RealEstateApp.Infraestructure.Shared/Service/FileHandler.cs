using Microsoft.AspNetCore.Http;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Infraestructure.Shared.Service
{
    public class FileHandler : IFileHandler
    {
        public async Task<string?> UploadAsync(IFormFile? file, string id, string folderName, bool isEditMode = false, string? imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            if (file == null)
            {
                return string.Empty;
            }

            // Validate file size (max 5MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                System.Diagnostics.Debug.WriteLine($"File size {file.Length} exceeds maximum allowed size of {maxFileSize} bytes");
                return null;
            }

            // Validate file is an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                System.Diagnostics.Debug.WriteLine($"File extension {fileExtension} is not allowed");
                return null;
            }

            try
            {
                string basePath = Path.Combine("Images", folderName, id);
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", basePath);

                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                Guid guid = Guid.NewGuid();
                FileInfo fileInfo = new(file.FileName);
                string fileName = guid + fileInfo.Extension;

                string fullFilePath = Path.Combine(physicalPath, fileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
                {
                    string normalizedOldPath = imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                    string completeOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedOldPath);

                    if (File.Exists(completeOldPath))
                    {
                        File.Delete(completeOldPath);
                    }
                }

                return $"/Images/{folderName}/{id}/{fileName}";
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use ILogger here)
                System.Diagnostics.Debug.WriteLine($"Error uploading file: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Return null instead of throwing to prevent application crash
                return null;
            }
        }

        public bool Delete(string id, string folderName)
        {
            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName, id);

            if (Directory.Exists(physicalPath))
            {
                Directory.Delete(physicalPath, true);
                return true;
            }

            return false;
        }

        public bool DeleteFile(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return false;

            string normalizedPath = imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedPath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                return true;
            }

            return false;
        }
    }
}

