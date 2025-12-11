using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class AdminDashboardServiceTests
    {
        private readonly Mock<ISaleTypeRepository> _saleTypeRepositoryMock;
        private readonly Mock<IPropertyTypeRepository> _propertyTypeRepositoryMock;
        private readonly Mock<IImprovementRepository> _improvementRepositoryMock;
        private readonly Mock<IBaseAccountService> _accountServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;

        private readonly AdminDashboardService _service;

        public AdminDashboardServiceTests()
        {
            _saleTypeRepositoryMock = new Mock<ISaleTypeRepository>();
            _propertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>();
            _improvementRepositoryMock = new Mock<IImprovementRepository>();
            _accountServiceMock = new Mock<IBaseAccountService>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();

            _service = new AdminDashboardService(
                _saleTypeRepositoryMock.Object,
                _propertyTypeRepositoryMock.Object,
                _improvementRepositoryMock.Object,
                _accountServiceMock.Object,
                _propertyRepositoryMock.Object);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsCorrectAggregatedCounts()
        {
            // Arrange

            // 1) Datos para SaleTypes, PropertyTypes, Improvements (solo usan Count/Count(x=>))
            var saleTypes = new List<SaleType>
            {
                new SaleType { Id = 1, Name = "Contado", IsActive = true },
                new SaleType { Id = 2, Name = "Financiado", IsActive = true },
                new SaleType { Id = 3, Name = "Renta", IsActive = false }
            };

            var propertyTypes = new List<PropertyType>
            {
                new PropertyType { Id = 1, Name = "Casa", IsActive = true },
                new PropertyType { Id = 2, Name = "Apartamento", IsActive = false }
            };

            var improvements = new List<Improvement>
            {
                new Improvement { Id = 1, Name = "Piscina", IsActive = true },
                new Improvement { Id = 2, Name = "Jardín", IsActive = true },
                new Improvement { Id = 3, Name = "Balcón", IsActive = false }
            };

            _saleTypeRepositoryMock
                .Setup(r => r.Query())
                .Returns(saleTypes.AsQueryable());

            _propertyTypeRepositoryMock
                .Setup(r => r.Query())
                .Returns(propertyTypes.AsQueryable());

            _improvementRepositoryMock
                .Setup(r => r.Query())
                .Returns(improvements.AsQueryable());

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            var properties = new List<Property>
            {
                new Property
                {
                    Id = 1,
                    Code = "P1",
                    Price = 100m,
                    Description = "Propiedad 1",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent-1",
                    Status = PropertyStatus.Vendida
                },
                new Property
                {
                    Id = 2,
                    Code = "P2",
                    Price = 200m,
                    Description = "Propiedad 2",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent-2",
                    Status = PropertyStatus.Disponible // cualquier estado != Vendida
                },
                new Property
                {
                    Id = 3,
                    Code = "P3",
                    Price = 300m,
                    Description = "Propiedad 3",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent-1",
                    Status = PropertyStatus.Vendida
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            _propertyRepositoryMock
                .Setup(r => r.Query())
                .Returns(context.Properties);

            var expectedTotalProperties = properties.Count;
            var expectedSoldProperties = properties.Count(p => p.Status == PropertyStatus.Vendida);
            var expectedAvailableProperties = expectedTotalProperties - expectedSoldProperties;

            // Agentes
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Agente, null))
                .ReturnsAsync(10);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Agente, true))
                .ReturnsAsync(7);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Agente, false))
                .ReturnsAsync(3);

            // Clientes
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Cliente, null))
                .ReturnsAsync(20);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Cliente, true))
                .ReturnsAsync(15);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Cliente, false))
                .ReturnsAsync(5);

            // Desarrolladores
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Desarrollador, null))
                .ReturnsAsync(4);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Desarrollador, true))
                .ReturnsAsync(3);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Desarrollador, false))
                .ReturnsAsync(1);

            // Administradores
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Administrador, null))
                .ReturnsAsync(2);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Administrador, true))
                .ReturnsAsync(2);
            _accountServiceMock
                .Setup(s => s.CountUsers(Roles.Administrador, false))
                .ReturnsAsync(0);

            // Act
            var result = await _service.GetDashboardAsync();

            // Assert

            Assert.NotNull(result);

            // SaleTypes
            Assert.Equal(saleTypes.Count, result.TotalSaleTypes);
            Assert.Equal(saleTypes.Count(x => x.IsActive), result.ActiveSaleTypes);
            Assert.Equal(saleTypes.Count(x => !x.IsActive), result.InactiveSaleTypes);

            // PropertyTypes
            Assert.Equal(propertyTypes.Count, result.TotalPropertyTypes);
            Assert.Equal(propertyTypes.Count(x => x.IsActive), result.ActivePropertyTypes);
            Assert.Equal(propertyTypes.Count(x => !x.IsActive), result.InactivePropertyTypes);

            // Improvements
            Assert.Equal(improvements.Count, result.TotalImprovements);
            Assert.Equal(improvements.Count(x => x.IsActive), result.ActiveImprovements);
            Assert.Equal(improvements.Count(x => !x.IsActive), result.InactiveImprovements);

            // Agentes
            Assert.Equal(10, result.TotalAgents);
            Assert.Equal(7, result.ActiveAgents);
            Assert.Equal(3, result.InactiveAgents);

            // Clientes
            Assert.Equal(20, result.TotalClients);
            Assert.Equal(15, result.ActiveClients);
            Assert.Equal(5, result.InactiveClients);

            // Desarrolladores
            Assert.Equal(4, result.TotalDevelopers);
            Assert.Equal(3, result.ActiveDevelopers);
            Assert.Equal(1, result.InactiveDevelopers);

            // Administradores
            Assert.Equal(2, result.TotalAdmins);
            Assert.Equal(2, result.ActiveAdmins);
            Assert.Equal(0, result.InactiveAdmins);

            // Propiedades
            Assert.Equal(expectedTotalProperties, result.TotalProperties);
            Assert.Equal(expectedAvailableProperties, result.AvailableProperties);
            Assert.Equal(expectedSoldProperties, result.SoldProperties);

            _accountServiceMock.Verify(s => s.CountUsers(Roles.Agente, null), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Agente, true), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Agente, false), Times.Once);

            _accountServiceMock.Verify(s => s.CountUsers(Roles.Cliente, null), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Cliente, true), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Cliente, false), Times.Once);

            _accountServiceMock.Verify(s => s.CountUsers(Roles.Desarrollador, null), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Desarrollador, true), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Desarrollador, false), Times.Once);

            _accountServiceMock.Verify(s => s.CountUsers(Roles.Administrador, null), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Administrador, true), Times.Once);
            _accountServiceMock.Verify(s => s.CountUsers(Roles.Administrador, false), Times.Once);
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            public DbSet<Property> Properties { get; set; } = null!;
        }
    }
}
