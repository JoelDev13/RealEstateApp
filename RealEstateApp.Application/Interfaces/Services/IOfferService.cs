using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IOfferService
    {
        Task<Offer> CreateOfferAsync(string clientId, int propertyId, decimal amount);
        Task<List<Offer>> GetClientOffersForPropertyAsync(string clientId, int propertyId);
        Task<bool> CanCreateOfferAsync(string clientId, int propertyId);
        Task<bool> HasAcceptedOfferAsync(int propertyId);
        Task<bool> HasPendingOfferAsync(string clientId, int propertyId);

        // Agent operations
        Task<List<Offer>> GetOffersByPropertyAsync(int propertyId);
        Task<List<Offer>> GetOffersByClientForPropertyAsync(string clientId, int propertyId);
        Task AcceptOfferAsync(Guid offerId, string agentId);
        Task RejectOfferAsync(Guid offerId, string agentId);
        Task<List<Offer>> GetPendingOffersByPropertyAsync(int propertyId);
        Task<List<string>> GetClientsWithOffersAsync(int propertyId);
        Task<Offer?> GetOfferByIdAsync(Guid offerId);
        Task<bool> ValidateAgentOwnsPropertyAsync(int propertyId, string agentId);
    }
}
