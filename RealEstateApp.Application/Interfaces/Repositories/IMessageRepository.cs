using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> GetChatBetweenUsersAsync(string userId1, string userId2, int propertyId);
        Task<List<Message>> GetPropertyMessagesForUserAsync(string userId, int propertyId);
        Task<List<Message>> GetPropertyMessagesAsync(int propertyId);
        Task<List<string>> GetUniqueChatPartnersAsync(string userId, int propertyId);
        Task<List<string>> GetClientsWithMessagesAsync(int propertyId);
    }
}
