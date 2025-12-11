using RealEstateApp.Application.Dtos.Agent;
using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAgentService
    {
        Task<bool> UpdateProfileAsync(string agentId, AgentProfileDto model);
        Task<AgentProfileDto> GetProfileAsync(string agentId);
        Task<List<UserDto>> GetAllActiveAgentsAsync();
        Task<UserDto?> GetAgentByIdAsync(string agentId);
        Task<UserDto?> GetClientByIdAsync(string clientId);
    }
}
