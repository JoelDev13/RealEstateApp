namespace RealEstateApp.WebApp.Helpers
{

    public static class FileHandler
    {
        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
        };

        private const long MaxFileSize = 30L * 1024 * 1024;

        public static string? Upload(
            IFormFile? file,
            string id,
            string folderName,
            bool isEditMode = false,
            string? imagePath = "")
        {
            if (isEditMode && file == null)
                return imagePath;

            if (file == null)
                return string.Empty;

            if (file.Length <= 0 || file.Length > MaxFileSize)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                return null;
            }

            var basePath = $"Images/{folderName}/{id}";
            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                basePath);

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullFilePath = Path.Combine(physicalPath, fileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
            {
                var oldFileName = imagePath.Split('/').Last();
                var oldFilePath = Path.Combine(physicalPath, oldFileName);

                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            return $"/{basePath}/{fileName}".Replace("\\", "/");
        }

        public static bool DeleteFolder(string id, string folderName)
        {
            var basePath = $"Images/{folderName}/{id}";
            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                basePath);

            if (!Directory.Exists(physicalPath))
                return false;

            Directory.Delete(physicalPath, true);
            return true;
        }

        public static bool DeleteFile(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return false;

            var relative = imagePath.TrimStart('/').Replace("\\", "/");

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relative);

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }
    }
}
