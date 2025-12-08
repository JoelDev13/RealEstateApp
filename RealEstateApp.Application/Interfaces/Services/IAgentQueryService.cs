using RealEstateApp.Application.Dtos.Agents;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAgentQueryService
    {
        Task<List<AgentDto>> GetAllAgentsAsync();
        Task<AgentDto?> GetAgentByIdAsync(string id);
        Task<List<AgentPropertyDto>> GetAgentPropertiesAsync(string agentId);
        Task<bool> ChangeAgentStatusAsync(string id, bool isActive);
    }
}
