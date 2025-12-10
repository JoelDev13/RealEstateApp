using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class MessageRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly MessageRepository _messageRepository;

        public MessageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _messageRepository = new MessageRepository(_context);
            
            // Seed required data
            SeedTestData().Wait();
        }

        private async Task SeedTestData()
        {
            // Agrega PropertyTypes y SaleTypes necesarios
            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            
            await _context.PropertyTypes.AddAsync(propertyType);
            await _context.SaleTypes.AddAsync(saleType);
            
            // Agrega propiedades de prueba
            var property1 = new Property 
            { 
                Id = 1, 
                Code = "PROP001", 
                Price = 100000, 
                Description = "Test", 
                SizeInSquareMeters = 100,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent-456"
            };
            
            var property2 = new Property 
            { 
                Id = 2, 
                Code = "PROP002", 
                Price = 200000, 
                Description = "Test 2", 
                SizeInSquareMeters = 150,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent-456"
            };
            
            await _context.Properties.AddAsync(property1);
            await _context.Properties.AddAsync(property2);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddAsync_AddsMessageToDatabase()
        {
            // Arrange
            var message = new Message
            {
                Id = Guid.NewGuid(),
                PropertyId = 1,
                SenderId = "user-123",
                ReceiverId = "agent-456",
                Content = "Hola, estoy interesado en esta propiedad",
                SentDate = DateTime.UtcNow
            };

            // Act
            await _messageRepository.AddAsync(message);
            await _context.SaveChangesAsync();

            // Assert
            var savedMessage = await _context.Messages.FindAsync(message.Id);
            Assert.NotNull(savedMessage);
            Assert.Equal(message.Content, savedMessage.Content);
            Assert.Equal(message.SenderId, savedMessage.SenderId);
            Assert.Equal(message.ReceiverId, savedMessage.ReceiverId);
        }

        [Fact]
        public async Task GetChatBetweenUsersAsync_ReturnsMessagesInCorrectOrder()
        {
            // Arrange
            var userId1 = "user-123";
            var userId2 = "agent-456";
            var propertyId = 1;

            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = userId1,
                    ReceiverId = userId2,
                    Content = "Primer mensaje",
                    SentDate = DateTime.UtcNow.AddMinutes(-10)
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = userId2,
                    ReceiverId = userId1,
                    Content = "Segundo mensaje",
                    SentDate = DateTime.UtcNow.AddMinutes(-5)
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = userId1,
                    ReceiverId = userId2,
                    Content = "Tercer mensaje",
                    SentDate = DateTime.UtcNow
                }
            };

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _messageRepository.GetChatBetweenUsersAsync(userId1, userId2, propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Primer mensaje", result[0].Content);
            Assert.Equal("Tercer mensaje", result[2].Content);
        }

        [Fact]
        public async Task GetChatBetweenUsersAsync_FiltersMessagesByProperty()
        {
            // Arrange
            var userId1 = "user-123";
            var userId2 = "agent-456";
            var propertyId1 = 1;
            var propertyId2 = 2;

            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId1,
                    SenderId = userId1,
                    ReceiverId = userId2,
                    Content = "Mensaje propiedad 1",
                    SentDate = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId2,
                    SenderId = userId1,
                    ReceiverId = userId2,
                    Content = "Mensaje propiedad 2",
                    SentDate = DateTime.UtcNow
                }
            };

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _messageRepository.GetChatBetweenUsersAsync(userId1, userId2, propertyId1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Mensaje propiedad 1", result[0].Content);
        }

        [Fact]
        public async Task GetPropertyMessagesAsync_ReturnsAllPropertyMessages()
        {
            // Arrange
            var propertyId = 1;

            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "client-1",
                    ReceiverId = "agent-1",
                    Content = "Mensaje 1",
                    SentDate = DateTime.UtcNow.AddHours(-2)
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "client-2",
                    ReceiverId = "agent-1",
                    Content = "Mensaje 2",
                    SentDate = DateTime.UtcNow.AddHours(-1)
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = 2,
                    SenderId = "client-3",
                    ReceiverId = "agent-1",
                    Content = "Mensaje propiedad diferente",
                    SentDate = DateTime.UtcNow
                }
            };

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _messageRepository.GetPropertyMessagesAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Equal(propertyId, m.PropertyId));
        }

        [Fact]
        public async Task GetClientsWithMessagesAsync_ReturnsUniqueClients()
        {
            // Arrange
            var propertyId = 1;
            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "client-1",
                    ReceiverId = "agent-1",
                    Content = "Mensaje 1",
                    SentDate = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "client-1",
                    ReceiverId = "agent-1",
                    Content = "Mensaje 2",
                    SentDate = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "client-2",
                    ReceiverId = "agent-1",
                    Content = "Mensaje 3",
                    SentDate = DateTime.UtcNow
                }
            };

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _messageRepository.GetClientsWithMessagesAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Dos clientes únicos
            Assert.Contains("client-1", result);
            Assert.Contains("client-2", result);
        }

        [Fact]
        public async Task GetUniqueChatPartnersAsync_ReturnsUniquePartners()
        {
            // Arrange
            var userId = "user-123";
            var propertyId = 1;

            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = userId,
                    ReceiverId = "agent-1",
                    Content = "Mensaje 1",
                    SentDate = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = "agent-1",
                    ReceiverId = userId,
                    Content = "Respuesta 1",
                    SentDate = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    SenderId = userId,
                    ReceiverId = "agent-2",
                    Content = "Mensaje 2",
                    SentDate = DateTime.UtcNow
                }
            };

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _messageRepository.GetUniqueChatPartnersAsync(userId, propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("agent-1", result);
            Assert.Contains("agent-2", result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
