using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class OfferRepository : Repository<Offer>, IOfferRepository
    {
        public OfferRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Offer>> GetByPropertyAsync(int propertyId)
        {
            return await _context.Offers
                .Where(o => o.PropertyId == propertyId)
                .Include(o => o.Property)
                .OrderByDescending(o => o.OfferDate)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetByClientAsync(string clientId)
        {
            return await _context.Offers
                .Where(o => o.ClientId == clientId)
                .Include(o => o.Property)
                    .ThenInclude(p => p.PropertyType)
                .Include(o => o.Property)
                    .ThenInclude(p => p.SaleType)
                .OrderByDescending(o => o.OfferDate)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetByClientAndPropertyAsync(string clientId, int propertyId)
        {
            return await _context.Offers
                .Where(o => o.ClientId == clientId && o.PropertyId == propertyId)
                .Include(o => o.Property)
                .OrderByDescending(o => o.OfferDate)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetPendingByPropertyAsync(int propertyId)
        {
            return await _context.Offers
                .Where(o => o.PropertyId == propertyId && o.Status == OfferStatus.Pendiente)
                .OrderByDescending(o => o.OfferDate)
                .ToListAsync();
        }

        public async Task<List<string>> GetClientsWithOffersAsync(int propertyId)
        {
            return await _context.Offers
                .Where(o => o.PropertyId == propertyId)
                .Select(o => o.ClientId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> HasAcceptedOfferAsync(int propertyId)
        {
            return await _context.Offers
                .AnyAsync(o => o.PropertyId == propertyId && o.Status == OfferStatus.Aceptada);
        }

        public async Task<bool> HasPendingOfferAsync(string clientId, int propertyId)
        {
            return await _context.Offers
                .AnyAsync(o => o.ClientId == clientId && o.PropertyId == propertyId && o.Status == OfferStatus.Pendiente);
        }

        public async Task<Offer?> GetPendingOfferByClientAndPropertyAsync(string clientId, int propertyId)
        {
            return await _context.Offers
                .FirstOrDefaultAsync(o => o.ClientId == clientId && o.PropertyId == propertyId && o.Status == OfferStatus.Pendiente);
        }

        public new async Task<Offer> AddAsync(Offer offer)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public new async Task<Offer> UpdateAsync(Offer offer)
        {
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
            return offer;
        }
    }
}
