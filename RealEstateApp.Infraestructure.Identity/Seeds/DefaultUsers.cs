using Microsoft.AspNetCore.Identity;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            // Usuario Administrador
            var adminUser = new AppUser
            {
                UserName = "admin",
                Email = "admin@realestateapp.com",
                EmailConfirmed = true,
                Cedula = "00000000000",
                FirstName = "Admin",
                LastName = "Sistema",
                UserType = "Administrador",
                PhoneNumber = "809-000-0000",
                ProfilePicture = "",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                var user = await userManager.FindByNameAsync(adminUser.UserName);
                if (user == null)
                {
                    await userManager.CreateAsync(adminUser, "Admin123!");
                    await userManager.AddToRoleAsync(adminUser, Roles.Administrador.ToString());
                }
            }

            // Usuario Agente
            var agenteUser = new AppUser
            {
                UserName = "agente",
                Email = "agente@realestateapp.com",
                EmailConfirmed = true,
                FirstName = "Juan",
                LastName = "Pérez",
                Cedula = "12345678901",
                UserType = "Agente",
                PhoneNumber = "809-123-4567",
                ProfilePicture = "",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (userManager.Users.All(u => u.UserName != agenteUser.UserName))
            {
                var user = await userManager.FindByNameAsync(agenteUser.UserName);
                if (user == null)
                {
                    await userManager.CreateAsync(agenteUser, "Agente123!");
                    await userManager.AddToRoleAsync(agenteUser, Roles.Agente.ToString());
                }
            }

            // Usuario Cliente
            var clienteUser = new AppUser
            {
                UserName = "cliente1",
                Email = "cliente1@realestateapp.com",
                EmailConfirmed = true,
                FirstName = "Cliente",
                LastName = "Ejemplo",
                Cedula = "98765432109",
                PhoneNumber = "809-987-6543",
                ProfilePicture = "",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (userManager.Users.All(u => u.UserName != clienteUser.UserName))
            {
                var user = await userManager.FindByNameAsync(clienteUser.UserName);
                if (user == null)
                {
                    await userManager.CreateAsync(clienteUser, "Cliente123!");
                    await userManager.AddToRoleAsync(clienteUser, Roles.Cliente.ToString());
                }
            }
        }
    }
}

