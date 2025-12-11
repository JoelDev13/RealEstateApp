using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IOfferRepository : IRepository<Offer>
    {
        Task<List<Offer>> GetByPropertyAsync(int propertyId);
        Task<List<Offer>> GetByClientAsync(string clientId);
        Task<List<Offer>> GetByClientAndPropertyAsync(string clientId, int propertyId);
        Task<List<Offer>> GetPendingByPropertyAsync(int propertyId);
        Task<List<string>> GetClientsWithOffersAsync(int propertyId);
        Task<bool> HasAcceptedOfferAsync(int propertyId);
        Task<bool> HasPendingOfferAsync(string clientId, int propertyId);
        Task<Offer?> GetPendingOfferByClientAndPropertyAsync(string clientId, int propertyId);
        new Task<Offer> AddAsync(Offer offer);
        new Task<Offer> UpdateAsync(Offer offer);
        new Task<Offer?> GetByIdAsync(Guid id);
    }
}
