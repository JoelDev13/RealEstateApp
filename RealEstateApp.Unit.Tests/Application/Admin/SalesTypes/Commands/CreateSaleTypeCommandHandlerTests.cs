using Moq;
using RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.SaleTypes.Commands;

public class CreateSaleTypeCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_SaleType_And_Return_Id()
    {
        // Arrange
        var repoMock = new Mock<ISaleTypeRepository>();
        SaleType? savedEntity = null;

        repoMock.Setup(r => r.AddAsync(It.IsAny<SaleType>()))
                .Callback<SaleType>(entity =>
                {
                    entity.Id = 1;
                    savedEntity = entity;
                })
                .Returns(Task.CompletedTask);

        var handler = new CreateSaleTypeCommandHandler(repoMock.Object);

        var command = new CreateSaleTypeCommand
        {
            Name = "Directa",
            Description = "Venta directa",
            IsActive = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        Assert.NotNull(savedEntity);
        Assert.Equal("Directa", savedEntity!.Name);
        Assert.Equal("Venta directa", savedEntity.Description);
        Assert.True(savedEntity.IsActive);
        Assert.NotNull(savedEntity.CreatedAt);

        repoMock.Verify(r => r.AddAsync(It.IsAny<SaleType>()), Times.Once);
    }
}
