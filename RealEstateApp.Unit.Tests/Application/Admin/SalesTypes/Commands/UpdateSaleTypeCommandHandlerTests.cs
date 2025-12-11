using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.SaleTypes.Commands;

public class UpdateSaleTypeCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_And_Return_Dto()
    {
        var repoMock = new Mock<ISaleTypeRepository>();

        var entity = new SaleType
        {
            Id = 1,
            Name = "Contado",
            Description = "Vieja desc",
            IsActive = true
        };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var handler = new UpdateSaleTypeCommandHandler(repoMock.Object);

        var command = new UpdateSaleTypeCommand
        {
            Id = 1,
            Name = "Financiado",
            Description = "Nueva desc",
            IsActive = false
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Financiado", result.Name);
        Assert.Equal("Nueva desc", result.Description);
        Assert.False(result.IsActive);
        Assert.NotNull(entity.UpdatedAt);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<ISaleTypeRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((SaleType?)null);

        var handler = new UpdateSaleTypeCommandHandler(repoMock.Object);

        var command = new UpdateSaleTypeCommand { Id = 99, Name = "Test" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );
    }
}
