using RealEstateApp.Application.Dtos.Agent;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAgentService
    {
        Task<bool> UpdateProfileAsync(string agentId, AgentProfileDto model);
        Task<AgentProfileDto> GetProfileAsync(string agentId);
    }
}
