using RealEstateApp.Application.Dtos.Messages;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(string senderId, string receiverId, int propertyId, string content);
        Task<List<Message>> GetChatBetweenUsersAsync(string userId1, string userId2, int propertyId);
        Task<List<Message>> GetPropertyMessagesForUserAsync(string userId, int propertyId);
        Task<List<string>> GetUniqueChatPartnersAsync(string userId, int propertyId);
        Task<List<MessageDto>> GetAgentChatPartnersAsync(int propertyId, string agentId);
        Task<bool> ValidateAgentOwnsPropertyAsync(int propertyId, string agentId);
        Task<bool> CanSendMessageAsync(string senderId, string receiverId, int propertyId);
    }
}
