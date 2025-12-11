using Moq;
using RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;

public class DeleteImprovementCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_When_Entity_Exists()
    {
        var repoMock = new Mock<IImprovementRepository>();
        var entity = new Improvement { Id = 1, Name = "Piscina" };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        repoMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var handler = new DeleteImprovementCommandHandler(repoMock.Object);

        var result = await handler.Handle(new DeleteImprovementCommand { Id = 1 }, CancellationToken.None);

        Assert.Equal(MediatR.Unit.Value, result);
        repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<IImprovementRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Improvement?)null);

        var handler = new DeleteImprovementCommandHandler(repoMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteImprovementCommand { Id = 99 }, CancellationToken.None)
        );
    }
}
