using Moq;
using RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.SaleTypes.Commands;

public class ToggleSaleTypeStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Toggle_Status()
    {
        var repoMock = new Mock<ISaleTypeRepository>();

        var entity = new SaleType { Id = 1, Name = "Contado", IsActive = true };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var handler = new ToggleSaleTypeStatusCommandHandler(repoMock.Object);

        var result = await handler.Handle(new ToggleSaleTypeStatusCommand { Id = 1 }, CancellationToken.None);

        Assert.False(entity.IsActive);
        Assert.NotNull(entity.UpdatedAt);

        Assert.Equal(MediatR.Unit.Value, result);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<ISaleTypeRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((SaleType?)null);

        var handler = new ToggleSaleTypeStatusCommandHandler(repoMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ToggleSaleTypeStatusCommand { Id = 99 }, CancellationToken.None)
        );
    }
}
