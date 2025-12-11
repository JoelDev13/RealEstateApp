using Moq;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovements;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Unit.Tests.Common;

namespace RealEstateApp.Unit.Tests.Application.Admin.Improvements.Queries
{
    public class GetImprovementsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Ordered_List_Of_Dtos()
        {
            var repoMock = new Mock<IImprovementRepository>();

            var data = new List<Improvement>
            {
                new Improvement { Id = 3, Name = "Zeta", Description = "Ultimo", IsActive = true },
                new Improvement { Id = 1, Name = "Alfa", Description = "Primero", IsActive = true },
                new Improvement { Id = 2, Name = "Beta", Description = "Segundo", IsActive = false }
            }.AsQueryable();

            var asyncData = new TestAsyncEnumerable<Improvement>(data);

            repoMock.Setup(r => r.Query()).Returns(asyncData);

            var handler = new GetImprovementsQueryHandler(repoMock.Object);
            var query = new GetImprovementsQuery();

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(3, result.Count);
            Assert.Equal("Alfa", result[0].Name);
            Assert.Equal("Beta", result[1].Name);
            Assert.Equal("Zeta", result[2].Name);

            repoMock.Verify(r => r.Query(), Times.Once);
        }
    }
}
