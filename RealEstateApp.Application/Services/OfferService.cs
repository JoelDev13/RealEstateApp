using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IPropertyRepository _propertyRepository;

        public OfferService(
            IOfferRepository offerRepository,
            IPropertyRepository propertyRepository)
        {
            _offerRepository = offerRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<Offer> CreateOfferAsync(string clientId, int propertyId, decimal amount)
        {
            // Valida que la propiedad existe y esta disponible
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException("Propiedad no encontrada");

            if (property.Status == PropertyStatus.Vendida)
                throw new InvalidOperationException("No se pueden crear ofertas para propiedades vendidas");

            // Valida si puede crear una oferta
            if (!await CanCreateOfferAsync(clientId, propertyId))
                throw new InvalidOperationException("No se puede crear oferta para esta propiedad");

            var offer = new Offer
            {
                PropertyId = propertyId,
                ClientId = clientId,
                Amount = amount,
                OfferDate = DateTime.UtcNow,
                Status = OfferStatus.Pendiente
            };

            return await _offerRepository.AddAsync(offer);
        }

        public async Task<List<Offer>> GetClientOffersForPropertyAsync(string clientId, int propertyId)
        {
            return await _offerRepository.GetByClientAndPropertyAsync(clientId, propertyId);
        }

        public async Task<bool> CanCreateOfferAsync(string clientId, int propertyId)
        {
            // Verifica si la propiedad tiene oferta aceptada
            if (await HasAcceptedOfferAsync(propertyId))
                return false;

            // Verifica si el cliente tiene oferta pendiente
            if (await HasPendingOfferAsync(clientId, propertyId))
                return false;

            return true;
        }

        public async Task<bool> HasAcceptedOfferAsync(int propertyId)
        {
            return await _offerRepository.HasAcceptedOfferAsync(propertyId);
        }

        public async Task<bool> HasPendingOfferAsync(string clientId, int propertyId)
        {
            return await _offerRepository.HasPendingOfferAsync(clientId, propertyId);
        }

        // Operaciones del agente
        public async Task<List<Offer>> GetOffersByPropertyAsync(int propertyId)
        {
            return await _offerRepository.GetByPropertyAsync(propertyId);
        }

        public async Task<List<Offer>> GetOffersByClientForPropertyAsync(string clientId, int propertyId)
        {
            return await _offerRepository.GetByClientAndPropertyAsync(clientId, propertyId);
        }

        public async Task AcceptOfferAsync(Guid offerId, string agentId)
        {
            var offer = await _offerRepository.GetByIdAsync(offerId);
            if (offer == null)
                throw new Exception("Oferta no encontrada");

            // Verifica que el agente sea dueño de la propiedad
            if (!await ValidateAgentOwnsPropertyAsync(offer.PropertyId, agentId))
                throw new Exception("No tienes permiso para aceptar esta oferta");

            // Verifica que la oferta esté pendiente
            if (offer.Status != OfferStatus.Pendiente)
                throw new Exception("Solo se pueden aceptar ofertas pendientes");

            // Verifica que la propiedad no esté vendida
            var property = await _propertyRepository.GetByIdAsync(offer.PropertyId);
            if (property.Status == PropertyStatus.Vendida)
                throw new Exception("La propiedad ya ha sido vendida");

            offer.Status = OfferStatus.Aceptada;
            await _offerRepository.UpdateAsync(offer);

            var allOffers = await _offerRepository.GetByPropertyAsync(offer.PropertyId);
            foreach (var otherOffer in allOffers.Where(o => o.Id != offerId && o.Status == OfferStatus.Pendiente))
            {
                otherOffer.Status = OfferStatus.Rechazada;
                await _offerRepository.UpdateAsync(otherOffer);
            }

            property.Status = PropertyStatus.Vendida;
            await _propertyRepository.UpdateAsync(property);
        }
        public async Task RejectOfferAsync(Guid offerId, string agentId)
        {
            var offer = await _offerRepository.GetByIdAsync(offerId);
            if (offer == null)
                throw new KeyNotFoundException("Oferta no encontrada");

            // Valida que el agente es dueño de la propiedad
            if (!await ValidateAgentOwnsPropertyAsync(offer.PropertyId, agentId))
                throw new UnauthorizedAccessException("No es el propietario de esta propiedad");

            offer.Status = OfferStatus.Rechazada;
            await _offerRepository.UpdateAsync(offer);
        }

        public async Task<List<Offer>> GetPendingOffersByPropertyAsync(int propertyId)
        {
            return await _offerRepository.GetPendingByPropertyAsync(propertyId);
        }

        public async Task<List<string>> GetClientsWithOffersAsync(int propertyId)
        {
            return await _offerRepository.GetClientsWithOffersAsync(propertyId);
        }

        public async Task<bool> ValidateAgentOwnsPropertyAsync(int propertyId, string agentId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            return property?.AgentId == agentId;
        }
        public async Task<Offer?> GetOfferByIdAsync(Guid offerId)
        {
            return await _offerRepository.GetByIdAsync(offerId);
        }
    }
}
