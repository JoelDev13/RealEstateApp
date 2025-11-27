using Microsoft.AspNetCore.Identity;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Infraestructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            // Crea el rol Administrador
            if (!await roleManager.RoleExistsAsync(Roles.Administrador.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrador.ToString()));
            }

            // Crea el rol Agente
            if (!await roleManager.RoleExistsAsync(Roles.Agente.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Agente.ToString()));
            }

            // Crea el rol Cliente
            if (!await roleManager.RoleExistsAsync(Roles.Cliente.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Cliente.ToString()));
            }
        }
    }
}

