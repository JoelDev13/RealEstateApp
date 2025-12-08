namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAgentProfileService
    {
        Task<object> GetAgentProfileAsync(string agentId);
        Task UpdateAgentProfileAsync(string agentId, object dto);
        Task<bool> ValidateAgentOwnershipAsync(string agentId, string currentUserId);
    }
}
