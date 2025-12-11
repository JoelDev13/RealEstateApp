using Moq;
using RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Application.Admin.PropertyTypes.Commands
{
    public class DeletePropertyTypeCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Delete_When_Entity_Exists()
        {
            // Arrange
            var repositoryMock = new Mock<IPropertyTypeRepository>();
            var existingEntity = new PropertyType { Id = 1, Name = "Casa" };

            repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingEntity);

            repositoryMock
                .Setup(r => r.DeleteAsync(existingEntity))
                .Returns(Task.CompletedTask);

            var handler = new DeletePropertyTypeCommandHandler(repositoryMock.Object);
            var command = new DeletePropertyTypeCommand { Id = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
            repositoryMock.Verify(r => r.DeleteAsync(existingEntity), Times.Once);

            Assert.Equal(MediatR.Unit.Value, result);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Entity_Does_Not_Exist()
        {
            // Arrange
            var repositoryMock = new Mock<IPropertyTypeRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((PropertyType?)null);

            var handler = new DeletePropertyTypeCommandHandler(repositoryMock.Object);
            var command = new DeletePropertyTypeCommand { Id = 99 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await handler.Handle(command, CancellationToken.None));

            repositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<PropertyType>()), Times.Never);
        }
    }
}
