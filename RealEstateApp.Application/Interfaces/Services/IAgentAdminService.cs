using RealEstateApp.Application.Dtos.Agents;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAgentAdminService
    {
        Task<Result<List<AgentAdminDto>>> GetAgentsAsync();
        Task<Result> ToggleStatusAsync(string agentId);
        Task<Result> DeleteAgentWithPropertiesAsync(string agentId);
    }
}
