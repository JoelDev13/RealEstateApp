using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Dtos.Agent;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AgentService : IAgentService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileHandler _fileHandler;

        public AgentService(UserManager<AppUser> userManager, IFileHandler fileHandler)
        {
            _userManager = userManager;
            _fileHandler = fileHandler;
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
                        var imagePath = await _fileHandler.UploadAsync(
                            model.ProfileImage, 
                            user.Id, 
                            "users", 
                            isEditMode: !string.IsNullOrEmpty(user.ProfilePicture), 
                            imagePath: user.ProfilePicture ?? string.Empty);
                        
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            user.ProfilePicture = imagePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other updates
                        System.Diagnostics.Debug.WriteLine($"Error uploading profile image: {ex.Message}");
                        // Don't throw, just skip image update
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
    }
}

