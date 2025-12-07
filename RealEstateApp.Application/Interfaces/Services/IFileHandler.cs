using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IFileHandler
    {
        Task<string?> UploadAsync(IFormFile? file, string id, string folderName, bool isEditMode = false, string? imagePath = "");
        bool Delete(string id, string folderName);
        bool DeleteFile(string imagePath);
    }
}

