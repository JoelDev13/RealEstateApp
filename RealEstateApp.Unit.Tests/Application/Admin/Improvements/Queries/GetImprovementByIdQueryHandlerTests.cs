using Moq;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

public class GetImprovementByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Dto_When_Entity_Exists()
    {
        // Arrange
        var repoMock = new Mock<IImprovementRepository>();

        var entity = new Improvement
        {
            Id = 1,
            Name = "Piscina",
            Description = "Piscina climatizada",
            IsActive = true
        };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var handler = new GetImprovementByIdQueryHandler(repoMock.Object);
        var query = new GetImprovementByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Piscina", result.Name);
        Assert.Equal("Piscina climatizada", result.Description);
        Assert.True(result.IsActive);

        repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Entity_Not_Found()
    {
        var repoMock = new Mock<IImprovementRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Improvement?)null);

        var handler = new GetImprovementByIdQueryHandler(repoMock.Object);
        var query = new GetImprovementByIdQuery(99);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        repoMock.Verify(r => r.GetByIdAsync(99), Times.Once);
    }
}
