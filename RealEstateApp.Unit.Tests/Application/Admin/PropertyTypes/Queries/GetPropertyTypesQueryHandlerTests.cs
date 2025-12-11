using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypes;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Unit.Tests.Application.Admin.PropertyTypes.Queries
{
    public class GetPropertyTypesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Ordered_List_Of_Dtos()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetPropertyTypesQueryHandlerTestsDb")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.PropertyTypes.AddRange(
                new PropertyType { Id = 2, Name = "Zeta", Description = "Desc Z", IsActive = true },
                new PropertyType { Id = 1, Name = "Alfa", Description = "Desc A", IsActive = false }
            );
            await context.SaveChangesAsync();

            var repository = new PropertyTypeRepository(context);

            var handler = new GetPropertyTypesQueryHandler(repository);
            var query = new GetPropertyTypesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal("Alfa", result[0].Name);
            Assert.Equal("Zeta", result[1].Name);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }
    }
}
