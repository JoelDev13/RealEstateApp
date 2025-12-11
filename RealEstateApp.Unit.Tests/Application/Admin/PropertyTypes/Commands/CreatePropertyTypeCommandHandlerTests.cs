using Moq;
using RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Application.Admin.PropertyTypes.Commands
{
    public class CreatePropertyTypeCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Map_And_Save_PropertyType_And_Return_Id()
        {
            // Arrange
            var repositoryMock = new Mock<IPropertyTypeRepository>();
            PropertyType? savedEntity = null;

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<PropertyType>()))
                .Callback<PropertyType>(entity =>
                {
                    entity.Id = 1;
                    savedEntity = entity;
                })
                .Returns(Task.CompletedTask);

            var handler = new CreatePropertyTypeCommandHandler(repositoryMock.Object);

            var command = new CreatePropertyTypeCommand
            {
                Name = "Apartamento",
                Description = "Ideal para inversión",
                IsActive = true
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<PropertyType>()), Times.Once);

            Assert.Equal(1, result);
            Assert.NotNull(savedEntity);
            Assert.Equal(command.Name, savedEntity!.Name);
            Assert.Equal(command.Description, savedEntity.Description);
            Assert.Equal(command.IsActive, savedEntity.IsActive);

            var now = DateTime.UtcNow;
            Assert.InRange(savedEntity.CreatedAt, now.AddMinutes(-1), now.AddSeconds(1));
        }
    }
}
