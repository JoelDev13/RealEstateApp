using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class PropertyRepositoryTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private IPropertyRepository _repository;

        public PropertyRepositoryTests()
        {
            Setup();
        }

        private void Setup()
        {
            var services = new ServiceCollection();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            _repository = _serviceProvider.GetRequiredService<IPropertyRepository>();

            // Seed data
            SeedPropertyTypes();
            SeedSaleTypes();
        }

        private void SeedPropertyTypes()
        {
            var propertyTypes = new[]
            {
                new PropertyType { Id = 1, Name = "Apartamento", Description = "Unidad residencial" },
                new PropertyType { Id = 2, Name = "Casa", Description = "Vivienda unifamiliar" },
                new PropertyType { Id = 3, Name = "Terreno", Description = "Lote sin construir" }
            };
            _context.PropertyTypes.AddRange(propertyTypes);
            _context.SaveChanges();
        }

        private void SeedSaleTypes()
        {
            var saleTypes = new[]
            {
                new SaleType { Id = 1, Name = "Venta", Description = "Venta directa" },
                new SaleType { Id = 2, Name = "Alquiler", Description = "Arrendamiento" },
                new SaleType { Id = 3, Name = "Venta-Alquiler", Description = "Ambas opciones" }
            };
            _context.SaleTypes.AddRange(saleTypes);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task CreateProperty_DebeCrearPropiedadCorrectamente()
        {
            // Arrange
            var property = new Property
            {
                Code = "PROP001",
                Price = 150000,
                Description = "Hermosa casa con 3 habitaciones",
                SizeInSquareMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent-123"
            };

            // Act
            var createdProperty = await _repository.AddAsync(property);

            // Assert
            Assert.NotNull(createdProperty);
            Assert.Equal("PROP001", createdProperty.Code);
            Assert.Equal(150000, createdProperty.Price);
            Assert.True(createdProperty.Id > 0);
        }

        [Fact]
        public async Task GetPropertiesByAgentId_DebeRetornarSoloPropiedadesDelAgente()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Code = "PROP001", Price = 100000, Description = "Propiedad 1", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" },
                new Property { Code = "PROP002", Price = 200000, Description = "Propiedad 2", SizeInSquareMeters = 150, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" },
                new Property { Code = "PROP003", Price = 300000, Description = "Propiedad 3", SizeInSquareMeters = 200, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-456" }
            };
            
            foreach (var prop in properties)
            {
                await _repository.AddAsync(prop);
            }

            // Act
            var agent1Properties = await _repository.GetByAgentAsync("agent-123");

            // Assert
            Assert.Equal(2, agent1Properties.Count);
            Assert.All(agent1Properties, p => Assert.Equal("agent-123", p.AgentId));
        }

        [Fact]
        public async Task GetAvailablePropertiesByAgentId_NoDebeRetornarPropiedadesVendidas()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Code = "PROP001", Price = 100000, Description = "Disponible", SizeInSquareMeters = 120, Status = PropertyStatus.Disponible, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" },
                new Property { Code = "PROP002", Price = 200000, Description = "Vendida", SizeInSquareMeters = 150, Status = PropertyStatus.Vendida, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" }
            };
            
            foreach (var prop in properties)
            {
                await _repository.AddAsync(prop);
            }

            // Act
            var availableProperties = await _repository.GetAvailableByAgentAsync("agent-123");

            // Assert
            Assert.Single(availableProperties);
            Assert.False(availableProperties[0].IsSold);
            Assert.Equal("Disponible", availableProperties[0].Description);
        }

        [Fact]
        public async Task UpdateProperty_DebeActualizarPropiedadCorrectamente()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Título Original", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            await _repository.AddAsync(property);

            // Act
            property.Description = "Título Actualizado";
            property.Price = 120000;
            var updatedProperty = await _repository.UpdateAsync(property);

            // Assert
            Assert.Equal("Título Actualizado", updatedProperty.Description);
            Assert.Equal(120000, updatedProperty.Price);
        }

        [Fact]
        public async Task DeleteProperty_DebeEliminarPropiedadCorrectamente()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Propiedad a eliminar", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            await _repository.AddAsync(property);

            // Act
            var deleteResult = await _repository.DeleteAsync(property.Id);

            // Assert
            Assert.True(deleteResult);
            
            var deletedProperty = await _repository.GetByIdAsync(property.Id);
            Assert.Null(deletedProperty);
        }

        [Fact]
        public async Task GetPropertyByCode_DebeRetornarPropiedadCorrecta()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Búsqueda por código", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            await _repository.AddAsync(property);

            // Act
            var foundProperty = await _repository.GetByCodeAsync("PROP001");

            // Assert
            Assert.NotNull(foundProperty);
            Assert.Equal("PROP001", foundProperty.Code);
            Assert.Equal("Búsqueda por código", foundProperty.Description);
        }

        [Fact]
        public async Task GetPropertyWithIncludes_DebeIncluirTodasLasRelaciones()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Propiedad con relaciones", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            
            var image = new PropertyImage { Url = "image1.jpg", PropertyId = property.Id };
            var improvement = new Improvement { Name = "Piscina", Description = "Piscina privada" };
            
            await _repository.AddAsync(property);
            _context.PropertyImages.Add(image);
            _context.Improvements.Add(improvement);
            await _context.SaveChangesAsync();

            // Act
            var propertyWithIncludes = await _repository.GetByIdAsync(property.Id);

            // Assert
            Assert.NotNull(propertyWithIncludes);
            Assert.NotNull(propertyWithIncludes.PropertyType);
            Assert.NotNull(propertyWithIncludes.SaleType);
            Assert.NotNull(propertyWithIncludes.Images);
            Assert.NotNull(propertyWithIncludes.Improvements);
        }

        [Fact]
        public async Task UpdatePropertyStatus_DebeCambiarEstadoAVendida()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Propiedad disponible", SizeInSquareMeters = 120, Status = PropertyStatus.Disponible, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            await _repository.AddAsync(property);

            // Act
            property.Status = PropertyStatus.Vendida;
            await _repository.UpdateAsync(property);

            // Assert
            var updatedProperty = await _repository.GetByIdAsync(property.Id);
            Assert.True(updatedProperty.IsSold);
        }

        [Fact]
        public async Task CodeExistsAsync_DebeVerificarExistenciaDeCodigo()
        {
            // Arrange
            var property = new Property { Code = "PROP001", Price = 100000, Description = "Propiedad con código", SizeInSquareMeters = 120, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" };
            await _repository.AddAsync(property);

            // Act
            var exists = await _repository.CodeExistsAsync("PROP001");
            var notExists = await _repository.CodeExistsAsync("PROP999");

            // Assert
            Assert.True(exists);
            Assert.False(notExists);
        }
    }
}
