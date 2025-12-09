using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.Agent;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AgentService : IAgentService
    {
        private readonly UserManager<AppUser> _userManager;

        public AgentService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> UpdateProfileAsync(string agentId, AgentProfileDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(agentId);
                if (user == null)
                {
                    return false;
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    try
                    {
                        var imagePath = await UploadProfileImageAsync(
                            model.ProfileImage,
                            user.Id,
                            user.ProfilePicture
                        );

                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            user.ProfilePicture = imagePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error uploading profile image: {ex.Message}");
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProfileAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<AgentProfileDto> GetProfileAsync(string agentId)
        {
            var user = await _userManager.FindByIdAsync(agentId);
            if (user == null)
            {
                return new AgentProfileDto();
            }

            return new AgentProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }

        private async Task<string?> UploadProfileImageAsync(
            IFormFile file,
            string userId,
            string? currentImagePath)
        {
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length <= 0 || file.Length > maxFileSize)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
            {
                return null;
            }

            var basePath = Path.Combine("Images", "users", userId);
            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                basePath
            );

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(physicalPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            if (!string.IsNullOrWhiteSpace(currentImagePath))
            {
                var relativeOldPath = currentImagePath.TrimStart('/').Replace("\\", "/");
                var oldFullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    relativeOldPath
                );

                if (File.Exists(oldFullPath))
                {
                    File.Delete(oldFullPath);
                }
            }
            var relativePath = "/" + Path.Combine(basePath, fileName).Replace("\\", "/");
            return relativePath;
        }
    }
}
