using Moq;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

public class GetSaleTypeByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Dto_When_Entity_Exists()
    {
        // Arrange
        var repoMock = new Mock<ISaleTypeRepository>();

        var entity = new SaleType
        {
            Id = 1,
            Name = "Contado",
            Description = "Pago inmediato",
            IsActive = true
        };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var handler = new GetSaleTypeByIdQueryHandler(repoMock.Object);
        var query = new GetSaleTypeByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Contado", result.Name);
        Assert.Equal("Pago inmediato", result.Description);
        Assert.True(result.IsActive);

        repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Entity_Not_Found()
    {
        // Arrange
        var repoMock = new Mock<ISaleTypeRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((SaleType?)null);

        var handler = new GetSaleTypeByIdQueryHandler(repoMock.Object);
        var query = new GetSaleTypeByIdQuery(99);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        repoMock.Verify(r => r.GetByIdAsync(99), Times.Once);
    }
}
