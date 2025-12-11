using Microsoft.AspNetCore.Identity;
using Moq;
using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Identity.Repositories;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class DeveloperUserRepositoryTests
    {
        private const string DeveloperRoleName = "Desarrollador";

        private Mock<UserManager<AppUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<AppUser>>();
            return new Mock<UserManager<AppUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                store.Object, null, null, null, null);
        }

        private DeveloperUserRepository CreateRepository(
            Mock<UserManager<AppUser>> userManagerMock,
            Mock<RoleManager<IdentityRole>> roleManagerMock)
        {
            return new DeveloperUserRepository(userManagerMock.Object, roleManagerMock.Object);
        }

        // GetAllDevelopersAsync
        [Fact]
        public async Task GetAllDevelopersAsync_Should_Return_Developer_List()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            var usersInRole = new List<AppUser>
            {
                new AppUser
                {
                    Id = "1",
                    FirstName = "Dev",
                    LastName = "Uno",
                    UserName = "dev1",
                    Cedula = "001",
                    Email = "dev1@test.com",
                    IsActive = true
                },
                new AppUser
                {
                    Id = "2",
                    FirstName = "Dev",
                    LastName = "Dos",
                    UserName = "dev2",
                    Cedula = "002",
                    Email = "dev2@test.com",
                    IsActive = false
                }
            };

            userManagerMock
                .Setup(um => um.GetUsersInRoleAsync(DeveloperRoleName))
                .ReturnsAsync(usersInRole);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            // Act
            var result = await repo.GetAllDevelopersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Id == "1" && d.IsActive);
            Assert.Contains(result, d => d.Id == "2" && !d.IsActive);
        }

        // GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_Should_Return_Developer_When_Exists()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            var user = new AppUser
            {
                Id = "dev-1",
                FirstName = "Dev",
                LastName = "Uno",
                UserName = "devuser",
                Cedula = "123",
                Email = "dev@test.com",
                IsActive = true
            };

            userManagerMock
                .Setup(um => um.FindByIdAsync("dev-1"))
                .ReturnsAsync(user);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            // Act
            var result = await repo.GetByIdAsync("dev-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("dev-1", result!.Id);
            Assert.Equal("Dev", result.FirstName);
            Assert.Equal("Uno", result.LastName);
            Assert.Equal("devuser", result.UserName);
            Assert.Equal("123", result.Cedula);
            Assert.Equal("dev@test.com", result.Email);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            userManagerMock
                .Setup(um => um.FindByIdAsync("no-existe"))
                .ReturnsAsync((AppUser?)null);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            // Act
            var result = await repo.GetByIdAsync("no-existe");

            // Assert
            Assert.Null(result);
        }

        // CreateDeveloperAsync
        [Fact]
        public async Task CreateDeveloperAsync_Should_Create_User_And_Assign_Role()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            // Setup CreateAsync
            userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<AppUser, string>((user, pwd) =>
                {
                    // Simular que identity asigna un Id
                    user.Id = "new-id";
                });

            roleManagerMock
                .Setup(rm => rm.RoleExistsAsync(DeveloperRoleName))
                .ReturnsAsync(false);

            roleManagerMock
                .Setup(rm => rm.CreateAsync(It.Is<IdentityRole>(r => r.Name == DeveloperRoleName)))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), DeveloperRoleName))
                .ReturnsAsync(IdentityResult.Success);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            var dto = new DeveloperUserCreateDto
            {
                FirstName = "Nuevo",
                LastName = "Dev",
                Cedula = "999",
                Email = "nuevo@dev.com",
                UserName = "nuevoDev",
                Password = "Password123*"
            };

            // Act
            var id = await repo.CreateDeveloperAsync(dto);

            // Assert
            Assert.Equal("new-id", id);

            userManagerMock.Verify(um =>
                um.CreateAsync(It.IsAny<AppUser>(), dto.Password),
                Times.Once);

            roleManagerMock.Verify(rm =>
                rm.RoleExistsAsync(DeveloperRoleName),
                Times.Once);

            roleManagerMock.Verify(rm =>
                rm.CreateAsync(It.Is<IdentityRole>(r => r.Name == DeveloperRoleName)),
                Times.Once);

            userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.IsAny<AppUser>(), DeveloperRoleName),
                Times.Once);
        }

        // UpdateDeveloperAsync
        [Fact]
        public async Task UpdateDeveloperAsync_Should_Update_Basic_Data_When_Password_Null()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            var existingUser = new AppUser
            {
                Id = "dev-1",
                FirstName = "Old",
                LastName = "Name",
                Cedula = "123",
                Email = "old@test.com",
                UserName = "oldUser"
            };

            userManagerMock
                .Setup(um => um.FindByIdAsync("dev-1"))
                .ReturnsAsync(existingUser);

            userManagerMock
                .Setup(um => um.UpdateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            var dto = new DeveloperUserUpdateDto
            {
                Id = "dev-1",
                FirstName = "New",
                LastName = "Name",
                Cedula = "456",
                Email = "new@test.com",
                UserName = "newUser",
                Password = null
            };

            // Act
            await repo.UpdateDeveloperAsync(dto);

            // Assert: valores en el objeto
            Assert.Equal("New", existingUser.FirstName);
            Assert.Equal("Name", existingUser.LastName);
            Assert.Equal("456", existingUser.Cedula);
            Assert.Equal("new@test.com", existingUser.Email);
            Assert.Equal("newUser", existingUser.UserName);

            userManagerMock.Verify(um =>
                um.UpdateAsync(existingUser),
                Times.Once);

            // No genera ni resetear password
            userManagerMock.Verify(um =>
                um.GeneratePasswordResetTokenAsync(It.IsAny<AppUser>()),
                Times.Never);

            userManagerMock.Verify(um =>
                um.ResetPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateDeveloperAsync_Should_Update_Password_When_Provided()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            var existingUser = new AppUser
            {
                Id = "dev-1",
                FirstName = "Dev",
                LastName = "User",
                Cedula = "123",
                Email = "dev@test.com",
                UserName = "devUser"
            };

            userManagerMock
                .Setup(um => um.FindByIdAsync("dev-1"))
                .ReturnsAsync(existingUser);

            userManagerMock
                .Setup(um => um.UpdateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(um => um.GeneratePasswordResetTokenAsync(existingUser))
                .ReturnsAsync("reset-token");

            userManagerMock
                .Setup(um => um.ResetPasswordAsync(existingUser, "reset-token", It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            var dto = new DeveloperUserUpdateDto
            {
                Id = "dev-1",
                FirstName = "Dev",
                LastName = "User",
                Cedula = "123",
                Email = "dev@test.com",
                UserName = "devUser",
                Password = "NewPassword123*"
            };

            // Act
            await repo.UpdateDeveloperAsync(dto);

            // Assert
            userManagerMock.Verify(um =>
                um.UpdateAsync(existingUser),
                Times.Once);

            userManagerMock.Verify(um =>
                um.GeneratePasswordResetTokenAsync(existingUser),
                Times.Once);

            userManagerMock.Verify(um =>
                um.ResetPasswordAsync(existingUser, "reset-token", "NewPassword123*"),
                Times.Once);
        }

        // SetActiveStatusAsync
        [Fact]
        public async Task SetActiveStatusAsync_Should_Update_IsActive()
        {
            var userManagerMock = CreateUserManagerMock();
            var roleManagerMock = CreateRoleManagerMock();

            var existingUser = new AppUser
            {
                Id = "dev-1",
                IsActive = true
            };

            userManagerMock
                .Setup(um => um.FindByIdAsync("dev-1"))
                .ReturnsAsync(existingUser);

            userManagerMock
                .Setup(um => um.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);

            var repo = CreateRepository(userManagerMock, roleManagerMock);

            // Act
            await repo.SetActiveStatusAsync("dev-1", false);

            // Assert
            Assert.False(existingUser.IsActive);

            userManagerMock.Verify(um =>
                um.UpdateAsync(existingUser),
                Times.Once);
        }
    }
}
