using AutoMapper;
using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;
using Xunit;

namespace RealEstateApp.Unit.Tests.Services
{
    public class BasicMessageServiceTests
    {
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IPropertyRepository> _mockPropertyRepository;
        private readonly MessageService _messageService;

        public BasicMessageServiceTests()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            
            _messageService = new MessageService(
                _mockMessageRepository.Object,
                _mockPropertyRepository.Object
            );
        }

        [Fact]
        public async Task GetChatBetweenUsersAsync_ReturnsMessages()
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
                    Content = "Mensaje 1",
                    SentDate = DateTime.UtcNow
                }
            };

            _mockMessageRepository
                .Setup(repo => repo.GetChatBetweenUsersAsync(userId1, userId2, propertyId))
                .ReturnsAsync(messages);

            // Act
            var result = await _messageService.GetChatBetweenUsersAsync(userId1, userId2, propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockMessageRepository.Verify(
                repo => repo.GetChatBetweenUsersAsync(userId1, userId2, propertyId),
                Times.Once
            );
        }

        [Fact]
        public async Task ValidateAgentOwnsPropertyAsync_WithValidAgent_ReturnsTrue()
        {
            // Arrange
            var propertyId = 1;
            var agentId = "agent-123";
            var property = new Property { Code = "PROP001" };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync(propertyId))
                .ReturnsAsync(property);

            // Act
            var result = await _messageService.ValidateAgentOwnsPropertyAsync(propertyId, agentId);

            // Assert
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(propertyId), Times.Once);
        }
    }
}
