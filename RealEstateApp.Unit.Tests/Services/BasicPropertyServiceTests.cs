using AutoMapper;
using Moq;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Services
{
    public class BasicPropertyServiceTests
    {
        private readonly Mock<IPropertyRepository> _mockPropertyRepository;
        private readonly Mock<IPropertyTypeRepository> _mockPropertyTypeRepository;
        private readonly Mock<ISaleTypeRepository> _mockSaleTypeRepository;
        private readonly Mock<IImprovementRepository> _mockImprovementRepository;
        private readonly Mock<IOfferRepository> _mockOfferRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PropertyService _propertyService;

        public BasicPropertyServiceTests()
        {
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _mockPropertyTypeRepository = new Mock<IPropertyTypeRepository>();
            _mockSaleTypeRepository = new Mock<ISaleTypeRepository>();
            _mockImprovementRepository = new Mock<IImprovementRepository>();
            _mockOfferRepository = new Mock<IOfferRepository>();
            _mockMapper = new Mock<IMapper>();

            _propertyService = new PropertyService(
                _mockPropertyRepository.Object,
                _mockPropertyTypeRepository.Object,
                _mockSaleTypeRepository.Object,
                _mockImprovementRepository.Object,
                _mockOfferRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithValidId_CallsRepository()
        {
            // Arrange
            var propertyId = "1";
            var expectedProperty = new Property
            {
                Id = 1,
                Code = "PROP001",
                Status = Domain.Enums.PropertyStatus.Disponible
            };

            var expectedDto = new PropertyDto
            {
                Id = 1,
                Code = "PROP001"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedProperty);

            _mockMapper
                .Setup(m => m.Map<PropertyDto>(It.IsAny<Property>()))
                .Returns(expectedDto);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PROP001", result.Code);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAvailablePropertiesAsync_ReturnsProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { Code = "PROP001" },
                new Property { Code = "PROP002" }
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetAvailablePropertiesAsync())
                .ReturnsAsync(properties);

            // Act
            var result = await _propertyService.GetAvailablePropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockPropertyRepository.Verify(repo => repo.GetAvailablePropertiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAgentPropertiesAsync_WithAgentId_ReturnsProperties()
        {
            // Arrange
            var agentId = "agent-123";
            var properties = new List<Property>
            {
                new Property { Code = "PROP001" },
                new Property { Code = "PROP002" }
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetAvailableByAgentAsync(agentId))
                .ReturnsAsync(properties);

            // Act
            var result = await _propertyService.GetAgentPropertiesAsync(agentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockPropertyRepository.Verify(repo => repo.GetAvailableByAgentAsync(agentId), Times.Once);
        }
    }
}
