using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class PropertyImprovementRepositoryTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;

        public PropertyImprovementRepositoryTests()
        {
            Setup();
        }

        private void Setup()
        {
            var services = new ServiceCollection();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed data
            SeedData();
        }

        private void SeedData()
        {
            var propertyTypes = new[]
            {
                new PropertyType { Id = 1, Name = "Apartamento", Description = "Unidad residencial" }
            };

            var saleTypes = new[]
            {
                new SaleType { Id = 1, Name = "Venta", Description = "Venta directa" }
            };

            var properties = new[]
            {
                new Property { Id = 1, Code = "PROP001", Price = 150000, Description = "Casa en Santo Domingo", SizeInSquareMeters = 150, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" },
                new Property { Id = 2, Code = "PROP002", Price = 80000, Description = "Apartamento en Santiago", SizeInSquareMeters = 80, PropertyTypeId = 1, SaleTypeId = 1, AgentId = "agent-123" }
            };

            var improvements = new[]
            {
                new Improvement { Id = 1, Name = "Aire Acondicionado", Description = "AC Central" },
                new Improvement { Id = 2, Name = "Piscina", Description = "Piscina privada" },
                new Improvement { Id = 3, Name = "Parqueo", Description = "Parqueo cubierto" },
                new Improvement { Id = 4, Name = "Seguridad", Description = "Seguridad 24/7" }
            };

            _context.PropertyTypes.AddRange(propertyTypes);
            _context.SaleTypes.AddRange(saleTypes);
            _context.Properties.AddRange(properties);
            _context.Improvements.AddRange(improvements);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task AddImprovementsToProperty_DebeAsociarMejoras()
        {
            // Arrange
            var property = await _context.Properties.FindAsync(1);
            var improvements = await _context.Improvements.Take(3).ToListAsync();

            // Act
            foreach (var improvement in improvements)
            {
                property.Improvements.Add(improvement);
            }
            await _context.SaveChangesAsync();

            // Assert
            var propertyWithImprovements = await _context.Properties
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Id == 1);
            
            Assert.Equal(3, propertyWithImprovements.Improvements.Count);
            Assert.Contains(propertyWithImprovements.Improvements, i => i.Name == "Aire Acondicionado");
            Assert.Contains(propertyWithImprovements.Improvements, i => i.Name == "Piscina");
            Assert.Contains(propertyWithImprovements.Improvements, i => i.Name == "Parqueo");
        }

        [Fact]
        public async Task GetImprovementsByPropertyId_DebeRetornarMejoras()
        {
            // Arrange
            var property1 = await _context.Properties.FindAsync(1);
            var property2 = await _context.Properties.FindAsync(2);
            var improvements = await _context.Improvements.ToListAsync();

            property1.Improvements.Add(improvements[0]); // Aire Acondicionado
            property1.Improvements.Add(improvements[1]); // Piscina
            property2.Improvements.Add(improvements[2]); // Parqueo
            property2.Improvements.Add(improvements[3]); // Seguridad
            
            await _context.SaveChangesAsync();

            // Act
            var property1Improvements = await _context.Properties
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Id == 1);

            // Assert
            Assert.Equal(2, property1Improvements.Improvements.Count);
            Assert.Contains(property1Improvements.Improvements, i => i.Name == "Aire Acondicionado");
            Assert.Contains(property1Improvements.Improvements, i => i.Name == "Piscina");
        }

        [Fact]
        public async Task RemoveImprovementsFromProperty_DebeEliminarAsociacion()
        {
            // Arrange
            var property = await _context.Properties.FindAsync(1);
            var improvements = await _context.Improvements.Take(3).ToListAsync();
            
            foreach (var improvement in improvements)
            {
                property.Improvements.Add(improvement);
            }
            await _context.SaveChangesAsync();

            // Act
            var improvementToRemove = property.Improvements.First(i => i.Name == "Piscina");
            property.Improvements.Remove(improvementToRemove);
            await _context.SaveChangesAsync();

            // Assert
            var updatedProperty = await _context.Properties
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Id == 1);
            
            Assert.Equal(2, updatedProperty.Improvements.Count);
            Assert.DoesNotContain(updatedProperty.Improvements, i => i.Name == "Piscina");
            Assert.Contains(updatedProperty.Improvements, i => i.Name == "Aire Acondicionado");
            Assert.Contains(updatedProperty.Improvements, i => i.Name == "Parqueo");
        }

        [Fact]
        public async Task UpdateImprovements_DebeActualizarListaDeMejoras()
        {
            // Arrange
            var property = await _context.Properties.FindAsync(1);
            var improvements = await _context.Improvements.Take(2).ToListAsync();
            
            property.Improvements.Add(improvements[0]); // Aire Acondicionado
            property.Improvements.Add(improvements[1]); // Piscina
            await _context.SaveChangesAsync();

            // Act - Eliminar Piscina y agregar Parqueo y Seguridad
            var toRemove = property.Improvements.First(i => i.Name == "Piscina");
            property.Improvements.Remove(toRemove);
            
            var newImprovements = await _context.Improvements
                .Where(i => i.Name == "Parqueo" || i.Name == "Seguridad")
                .ToListAsync();
            
            foreach (var improvement in newImprovements)
            {
                property.Improvements.Add(improvement);
            }
            await _context.SaveChangesAsync();

            // Assert
            var updatedProperty = await _context.Properties
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Id == 1);
            
            Assert.Equal(3, updatedProperty.Improvements.Count);
            Assert.Contains(updatedProperty.Improvements, i => i.Name == "Aire Acondicionado");
            Assert.Contains(updatedProperty.Improvements, i => i.Name == "Parqueo");
            Assert.Contains(updatedProperty.Improvements, i => i.Name == "Seguridad");
            Assert.DoesNotContain(updatedProperty.Improvements, i => i.Name == "Piscina");
        }

        [Fact]
        public async Task CreateImprovement_DebeCrearMejoraCorrectamente()
        {
            // Arrange
            var newImprovement = new Improvement
            {
                Name = "Jardín",
                Description = "Jardín paisajístico",
                IsActive = true
            };

            // Act
            _context.Improvements.Add(newImprovement);
            await _context.SaveChangesAsync();

            // Assert
            var savedImprovement = await _context.Improvements.FindAsync(newImprovement.Id);
            Assert.NotNull(savedImprovement);
            Assert.Equal("Jardín", savedImprovement.Name);
            Assert.Equal("Jardín paisajístico", savedImprovement.Description);
            Assert.True(savedImprovement.IsActive);
        }

        [Fact]
        public async Task UpdateImprovement_DebeActualizarDatosCorrectamente()
        {
            // Arrange
            var improvement = await _context.Improvements.FirstAsync();

            // Act
            improvement.Name = "Aire Acondicionado Actualizado";
            improvement.Description = "AC Central con tecnología inverter";
            improvement.UpdatedAt = DateTime.UtcNow;
            
            _context.Improvements.Update(improvement);
            await _context.SaveChangesAsync();

            // Assert
            var updatedImprovement = await _context.Improvements.FindAsync(improvement.Id);
            Assert.Equal("Aire Acondicionado Actualizado", updatedImprovement.Name);
            Assert.Equal("AC Central con tecnología inverter", updatedImprovement.Description);
            Assert.NotNull(updatedImprovement.UpdatedAt);
        }

        [Fact]
        public async Task DeleteImprovement_DebeEliminarCorrectamente()
        {
            // Arrange
            var improvement = await _context.Improvements.FirstAsync();

            // Act
            _context.Improvements.Remove(improvement);
            await _context.SaveChangesAsync();

            // Assert
            var deletedImprovement = await _context.Improvements.FindAsync(improvement.Id);
            Assert.Null(deletedImprovement);
        }
    }
}
