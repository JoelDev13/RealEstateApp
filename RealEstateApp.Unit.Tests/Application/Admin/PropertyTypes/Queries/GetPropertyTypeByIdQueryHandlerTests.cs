using Moq;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

public class GetPropertyTypeByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Dto_When_Entity_Exists()
    {
        // Arrange
        var repositoryMock = new Mock<IPropertyTypeRepository>();

        var entity = new PropertyType
        {
            Id = 1,
            Name = "Casa",
            Description = "Amplia",
            IsActive = true
        };

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(entity);

        var handler = new GetPropertyTypeByIdQueryHandler(repositoryMock.Object);
        var query = new GetPropertyTypeByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Casa", result.Name);
        Assert.Equal("Amplia", result.Description);
        Assert.True(result.IsActive);

        repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Entity_Not_Found()
    {
        // Arrange
        var repositoryMock = new Mock<IPropertyTypeRepository>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((PropertyType?)null);

        var handler = new GetPropertyTypeByIdQueryHandler(repositoryMock.Object);
        var query = new GetPropertyTypeByIdQuery(99);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(query, CancellationToken.None));

        repositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
    }
}
