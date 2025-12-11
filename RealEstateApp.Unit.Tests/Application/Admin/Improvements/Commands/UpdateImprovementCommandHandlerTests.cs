using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;

public class UpdateImprovementCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_And_Return_Dto()
    {
        var repoMock = new Mock<IImprovementRepository>();

        var entity = new Improvement
        {
            Id = 1,
            Name = "Piscina",
            Description = "Vieja desc",
            IsActive = true
        };

        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var handler = new UpdateImprovementCommandHandler(repoMock.Object);

        var command = new UpdateImprovementCommand
        {
            Id = 1,
            Name = "Jacuzzi",
            Description = "Nueva desc",
            IsActive = false
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Jacuzzi", result.Name);
        Assert.Equal("Nueva desc", result.Description);
        Assert.False(result.IsActive);
        Assert.NotNull(entity.UpdatedAt);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        var repoMock = new Mock<IImprovementRepository>();

        repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Improvement?)null);

        var handler = new UpdateImprovementCommandHandler(repoMock.Object);

        var command = new UpdateImprovementCommand { Id = 99, Name = "Test" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );
    }
}
