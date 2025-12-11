using Moq;
using RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;


public class CreateImprovementCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Improvement_And_Return_Id()
    {
        // Arrange
        var repoMock = new Mock<IImprovementRepository>();
        Improvement? savedEntity = null;

        repoMock.Setup(r => r.AddAsync(It.IsAny<Improvement>()))
                .Callback<Improvement>(entity =>
                {
                    entity.Id = 1;
                    savedEntity = entity;
                })
                .Returns(Task.CompletedTask);

        var handler = new CreateImprovementCommandHandler(repoMock.Object);

        var command = new CreateImprovementCommand
        {
            Name = "Piscina",
            Description = "Incluye piscina privada",
            IsActive = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        Assert.NotNull(savedEntity);
        Assert.Equal("Piscina", savedEntity!.Name);
        Assert.Equal("Incluye piscina privada", savedEntity.Description);
        Assert.True(savedEntity.IsActive);
        Assert.NotNull(savedEntity.CreatedAt);

        repoMock.Verify(r => r.AddAsync(It.IsAny<Improvement>()), Times.Once);
    }
}
