using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class PropertyImageRepositoryTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;

        public PropertyImageRepositoryTests()
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

            _context.PropertyTypes.AddRange(propertyTypes);
            _context.SaleTypes.AddRange(saleTypes);
            _context.Properties.AddRange(properties);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task AddImages_DebeAgregarMultiplesImagenes()
        {
            // Arrange
            var images = new[]
            {
                new PropertyImage { Url = "image1.jpg", PropertyId = 1 },
                new PropertyImage { Url = "image2.jpg", PropertyId = 1 },
                new PropertyImage { Url = "image3.jpg", PropertyId = 1 }
            };

            // Act
            _context.PropertyImages.AddRange(images);
            await _context.SaveChangesAsync();

            // Assert
            var savedImages = await _context.PropertyImages.Where(pi => pi.PropertyId == 1).ToListAsync();
            Assert.Equal(3, savedImages.Count);
            Assert.Contains(savedImages, i => i.Url == "image1.jpg");
            Assert.Contains(savedImages, i => i.Url == "image2.jpg");
            Assert.Contains(savedImages, i => i.Url == "image3.jpg");
        }

        [Fact]
        public async Task GetImagesByPropertyId_DebeRetornarTodasLasImagenes()
        {
            // Arrange
            var images = new[]
            {
                new PropertyImage { Url = "frontal.jpg", PropertyId = 1 },
                new PropertyImage { Url = "interior.jpg", PropertyId = 1 },
                new PropertyImage { Url = "jardin.jpg", PropertyId = 2 }
            };
            _context.PropertyImages.AddRange(images);
            await _context.SaveChangesAsync();

            // Act
            var property1Images = await _context.PropertyImages.Where(pi => pi.PropertyId == 1).ToListAsync();

            // Assert
            Assert.Equal(2, property1Images.Count);
            Assert.All(property1Images, i => Assert.Equal(1, i.PropertyId));
            Assert.Contains(property1Images, i => i.Url == "frontal.jpg");
            Assert.Contains(property1Images, i => i.Url == "interior.jpg");
        }

        [Fact]
        public async Task DeleteImagesByPropertyId_DebeEliminarTodasLasImagenes()
        {
            // Arrange
            var images = new[]
            {
                new PropertyImage { Url = "image1.jpg", PropertyId = 1 },
                new PropertyImage { Url = "image2.jpg", PropertyId = 1 },
                new PropertyImage { Url = "image3.jpg", PropertyId = 2 }
            };
            _context.PropertyImages.AddRange(images);
            await _context.SaveChangesAsync();

            // Act
            var property1Images = await _context.PropertyImages.Where(pi => pi.PropertyId == 1).ToListAsync();
            _context.PropertyImages.RemoveRange(property1Images);
            await _context.SaveChangesAsync();

            // Assert
            var remainingImages = await _context.PropertyImages.Where(pi => pi.PropertyId == 1).ToListAsync();
            var property2Images = await _context.PropertyImages.Where(pi => pi.PropertyId == 2).ToListAsync();

            Assert.Empty(remainingImages);
            Assert.Single(property2Images);
        }

        [Fact]
        public async Task UpdateImage_DebeActualizarUrlCorrectamente()
        {
            // Arrange
            var image = new PropertyImage { Url = "original.jpg", PropertyId = 1 };
            _context.PropertyImages.Add(image);
            await _context.SaveChangesAsync();

            // Act
            image.Url = "updated.jpg";
            _context.PropertyImages.Update(image);
            await _context.SaveChangesAsync();

            // Assert
            var updatedImage = await _context.PropertyImages.FindAsync(image.Id);
            Assert.Equal("updated.jpg", updatedImage.Url);
        }

        [Fact]
        public async Task DeleteImage_DebeEliminarImagenCorrectamente()
        {
            // Arrange
            var image = new PropertyImage { Url = "to_delete.jpg", PropertyId = 1 };
            _context.PropertyImages.Add(image);
            await _context.SaveChangesAsync();

            // Act
            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();

            // Assert
            var deletedImage = await _context.PropertyImages.FindAsync(image.Id);
            Assert.Null(deletedImage);
        }
    }
}
