using Moq;
using RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.PropertyTypes.Commands;

public class TogglePropertyTypeStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Toggle_Status_And_Update_Entity()
    {
        // Arrange
        var repositoryMock = new Mock<IPropertyTypeRepository>();

        var entity = new PropertyType
        {
            Id = 1,
            Name = "Casa",
            IsActive = true
        };

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(entity);

        repositoryMock
            .Setup(r => r.UpdateAsync(entity))
            .Returns(Task.CompletedTask);

        var handler = new TogglePropertyTypeStatusCommandHandler(repositoryMock.Object);
        var command = new TogglePropertyTypeStatusCommand { Id = 1 };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(entity.IsActive);
        Assert.NotNull(entity.UpdatedAt);

        repositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        Assert.Equal(MediatR.Unit.Value, result);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Entity_Not_Found()
    {
        // Arrange
        var repositoryMock = new Mock<IPropertyTypeRepository>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((PropertyType?)null);

        var handler = new TogglePropertyTypeStatusCommandHandler(repositoryMock.Object);
        var command = new TogglePropertyTypeStatusCommand { Id = 99 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PropertyType>()), Times.Never);
    }
}
