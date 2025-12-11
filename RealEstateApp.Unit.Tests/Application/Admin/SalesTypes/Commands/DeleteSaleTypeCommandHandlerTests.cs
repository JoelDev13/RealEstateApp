using MediatR;
using Moq;
using RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

public class DeleteSaleTypeCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_When_Entity_Exists()
    {
        var repoMock = new Mock<ISaleTypeRepository>();
        var entity = new SaleType { Id = 1, Name = "Test" };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        repoMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var handler = new DeleteSaleTypeCommandHandler(repoMock.Object);

        var result = await handler.Handle(new DeleteSaleTypeCommand { Id = 1 }, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<ISaleTypeRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((SaleType?)null);

        var handler = new DeleteSaleTypeCommandHandler(repoMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteSaleTypeCommand { Id = 99 }, CancellationToken.None)
        );
    }
}

