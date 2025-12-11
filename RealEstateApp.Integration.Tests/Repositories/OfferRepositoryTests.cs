using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class OfferRepositoryTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private async Task<Property> CreateTestProperty(ApplicationDbContext context)
        {
            var saleType = new SaleType { Name = "Venta" };
            var propertyType = new PropertyType { Name = "Casa" };

            context.SaleTypes.Add(saleType);
            context.PropertyTypes.Add(propertyType);
            await context.SaveChangesAsync();

            var property = new Property
            {
                Code = "PR001",
                Price = 250000,
                Description = "Casa de prueba",
                SizeInSquareMeters = 120,
                Bedrooms = 3,
                Bathrooms = 2,
                SaleTypeId = saleType.Id,
                PropertyTypeId = propertyType.Id,
                AgentId = "AGENT1"
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            return property;
        }

        private Offer CreateOffer(int propertyId, string clientId, decimal amount, OfferStatus status = OfferStatus.Pendiente)
        {
            return new Offer
            {
                PropertyId = propertyId,
                ClientId = clientId,
                Amount = amount,
                Status = status,
                OfferDate = DateTime.UtcNow
            };
        }

        // ADD & UPDATE
        [Fact]
        public async Task AddAsync_Should_Create_New_Offer()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            var offer = CreateOffer(property.Id, "CLIENT1", 100000);

            await repo.AddAsync(offer);

            Assert.NotEqual(Guid.Empty, offer.Id);

            var saved = await context.Offers.FindAsync(offer.Id);
            Assert.NotNull(saved);
            Assert.Equal("CLIENT1", saved!.ClientId);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Existing_Offer()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            var offer = CreateOffer(property.Id, "CLIENT1", 100000);
            await repo.AddAsync(offer);

            // Update
            offer.Amount = 120000;
            offer.Status = OfferStatus.Aceptada;

            await repo.UpdateAsync(offer);

            var updated = await context.Offers.FindAsync(offer.Id);
            Assert.Equal(120000, updated!.Amount);
            Assert.Equal(OfferStatus.Aceptada, updated.Status);
        }

        // GET BY PROPERTY
        [Fact]
        public async Task GetByPropertyAsync_Should_Return_Offers_For_Property()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "C1", 90000));
            context.Offers.Add(CreateOffer(property.Id, "C2", 95000));
            await context.SaveChangesAsync();

            var offers = await repo.GetByPropertyAsync(property.Id);

            Assert.Equal(2, offers.Count);
        }

        // GET BY CLIENT
        [Fact]
        public async Task GetByClientAsync_Should_Return_Offers_For_Client()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "CLIENT1", 85000));
            context.Offers.Add(CreateOffer(property.Id, "CLIENT1", 90000));
            await context.SaveChangesAsync();

            var offers = await repo.GetByClientAsync("CLIENT1");

            Assert.Equal(2, offers.Count);
        }

        // GET BY CLIENT + PROPERTY
        [Fact]
        public async Task GetByClientAndPropertyAsync_Should_Return_Matching_Offers()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "CLIENT1", 100000));
            context.Offers.Add(CreateOffer(property.Id, "CLIENT1", 105000));
            await context.SaveChangesAsync();

            var offers = await repo.GetByClientAndPropertyAsync("CLIENT1", property.Id);

            Assert.Equal(2, offers.Count);
        }

        // PENDING OFFERS
        [Fact]
        public async Task GetPendingByPropertyAsync_Should_Return_Only_Pending_Offers()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "C1", 100000, OfferStatus.Pendiente));
            context.Offers.Add(CreateOffer(property.Id, "C2", 120000, OfferStatus.Aceptada));
            context.Offers.Add(CreateOffer(property.Id, "C3", 90000, OfferStatus.Pendiente));

            await context.SaveChangesAsync();

            var pending = await repo.GetPendingByPropertyAsync(property.Id);

            Assert.Equal(2, pending.Count);
        }

        // DISTINCT CLIENTS
        [Fact]
        public async Task GetClientsWithOffersAsync_Should_Return_Unique_Clients()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "C1", 80000));
            context.Offers.Add(CreateOffer(property.Id, "C1", 85000));
            context.Offers.Add(CreateOffer(property.Id, "C2", 90000));
            await context.SaveChangesAsync();

            var clients = await repo.GetClientsWithOffersAsync(property.Id);

            Assert.Equal(2, clients.Count);
            Assert.Contains("C1", clients);
            Assert.Contains("C2", clients);
        }

        // STATUS CHECKS
        [Fact]
        public async Task HasAcceptedOfferAsync_Should_Return_True_When_Accepted_Exists()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "C1", 100000, OfferStatus.Aceptada));
            await context.SaveChangesAsync();

            var result = await repo.HasAcceptedOfferAsync(property.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasPendingOfferAsync_Should_Return_True_When_Pending_Exists()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            context.Offers.Add(CreateOffer(property.Id, "CLIENT1", 100000, OfferStatus.Pendiente));
            await context.SaveChangesAsync();

            var result = await repo.HasPendingOfferAsync("CLIENT1", property.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task GetPendingOfferByClientAndPropertyAsync_Should_Return_Offer()
        {
            using var context = CreateContext();
            var repo = new OfferRepository(context);

            var property = await CreateTestProperty(context);

            var pendingOffer = CreateOffer(property.Id, "CLIENT1", 95000, OfferStatus.Pendiente);
            context.Offers.Add(pendingOffer);
            await context.SaveChangesAsync();

            var result = await repo.GetPendingOfferByClientAndPropertyAsync("CLIENT1", property.Id);

            Assert.NotNull(result);
            Assert.Equal(pendingOffer.Id, result!.Id);
        }
    }
}
