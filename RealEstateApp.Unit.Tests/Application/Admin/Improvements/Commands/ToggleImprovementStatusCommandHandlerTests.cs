using Moq;
using RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;

public class ToggleImprovementStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Toggle_Status()
    {
        var repoMock = new Mock<IImprovementRepository>();

        var entity = new Improvement { Id = 1, Name = "Piscina", IsActive = true };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var handler = new ToggleImprovementStatusCommandHandler(repoMock.Object);

        var result = await handler.Handle(new ToggleImprovementStatusCommand { Id = 1 }, CancellationToken.None);

        Assert.False(entity.IsActive);
        Assert.NotNull(entity.UpdatedAt);
        Assert.Equal(MediatR.Unit.Value, result);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<IImprovementRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Improvement?)null);

        var handler = new ToggleImprovementStatusCommandHandler(repoMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ToggleImprovementStatusCommand { Id = 99 }, CancellationToken.None)
        );
    }
}
