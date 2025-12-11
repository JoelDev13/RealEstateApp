using Moq;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Unit.Tests.Common;

namespace RealEstateApp.Unit.Tests.Application.Admin.SaleTypes.Queries
{
    public class GetSaleTypesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Ordered_List_Of_Dtos()
        {
            var repoMock = new Mock<ISaleTypeRepository>();

            var data = new List<SaleType>
            {
                new SaleType { Id = 2, Name = "Zeta", Description = "Desc Z", IsActive = true },
                new SaleType { Id = 1, Name = "Alfa", Description = "Desc A", IsActive = false }
            }.AsQueryable();

            var asyncData = new TestAsyncEnumerable<SaleType>(data);

            repoMock.Setup(r => r.Query()).Returns(asyncData);

            var handler = new GetSaleTypesQueryHandler(repoMock.Object);
            var query = new GetSaleTypesQuery();

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Equal("Alfa", result[0].Name);
            Assert.Equal("Zeta", result[1].Name);

            repoMock.Verify(r => r.Query(), Times.Once);
        }
    }
}
