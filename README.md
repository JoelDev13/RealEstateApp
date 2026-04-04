# RealEstateApp
 
 Plataforma web para gestión y publicación de propiedades inmobiliarias, con **roles**, **autenticación**, **módulo de administración**, **catálogos**, **ofertas** y **chat** entre cliente y agente. Incluye una **WebApp MVC** y una **Web API** (versionada) construidas sobre .NET 8
 
 ## Qué construimos
 - **WebApp (ASP.NET Core MVC)**
   - Portal público para navegar propiedades
   - Registro/login por roles (cliente/agente) y paneles por rol
 - **Web API (ASP.NET Core Web API + Swagger + API Versioning)**
   - Endpoints para autenticación y catálogos (propiedades, tipos, etc.)
 - **Arquitectura por capas**
   - `Domain` (entidades + enums + settings)
   - `Application` (casos de uso/servicios/DTOs)
   - `Infrastructure` (Persistencia + Identity + Shared)
   - `WebApp` y `WebApi` (presentación)
 
 ## Features principales
 ### Roles
 - **Administrador**
   - Gestión de agentes y catálogos (por ejemplo: tipos de venta, tipos de propiedad, mejoras)
 - **Agente**
   - Crear/editar propiedades
   - Subida de imágenes
   - Gestión de ofertas
   - Chat con clientes interesados
 - **Cliente**
   - Navegar propiedades
   - Chatear con el agente responsable de la propiedad
 
 ### Entidades del dominio
 - `Property`, `PropertyImage`, `PropertyType`, `SaleType`, `Improvement`
 - `Offer`, `FavoriteProperty`, `Message`
 
 ## Stack / Tecnologías
 - **.NET 8**
 - **ASP.NET Core MVC** (`RealEstateApp.WebApp`)
 - **ASP.NET Core Web API** (`RealEstateApp.WebApi`) con:
   - **Swagger (Swashbuckle)**
   - **API Versioning**
 - **Entity Framework Core (SQL Server)**
 - **ASP.NET Core Identity** (roles + confirmación de email)
 - **JWT** para autenticación en WebApi
 - **AutoMapper**, **MediatR**, **FluentValidation**
 - **TailwindCSS** (compilación a `wwwroot`) para estilos
 
 ## Estructura del repositorio
 - `RealEstateApp.WebApp/` - UI MVC (Views, Controllers, wwwroot)
 - `RealEstateApp.WebApi/` - API REST (Controllers/v1, Swagger)
 - `RealEstateApp.Application/` - servicios, DTOs, features
 - `RealEstateApp.Domain/` - entidades, enums y settings
 - `RealEstateApp.Infraestructure.Persistence/` - EF Core DbContext + repos
 - `RealEstateApp.Infraestructure.Identity/` - IdentityContext + servicios + seeds
 - `RealEstateApp.Unit.Tests/` y `RealEstateApp.Integration.Tests/`
 
 ## Requisitos
 - **.NET SDK 8**
 - **SQL Server / SQLExpress**
 - **Node.js** (solo para compilar Tailwind)
 
 ## Configuración
 La configuración está en:
 - `RealEstateApp.WebApp/appsettings.json`
 - `RealEstateApp.WebApi/appsettings.json`
 
 ## Docker (docker-compose)
 En la raíz del repo hay un `docker-compose.yml` que levanta:
 - `sqlserver` (SQL Server 2022 Express)
 - `webapi` (RealEstateApp.WebApi)
 - `webapp` (RealEstateApp.WebApp)
 
 ### Requisitos
 - Docker Desktop
 
 ### Levantar el ambiente
 Desde la raíz del repositorio:
 - `docker compose up --build`
 
 Puertos (host):
 - WebApp: `http://localhost:5111`
 - WebApi (Swagger): `http://localhost:5185/swagger`
 - SQL Server: `localhost,1433`
 
 ### Variables / secretos
 En `docker-compose.yml` se definen (como ejemplo) estas variables:
 - `ConnectionStrings__DefaultConnection`
 - `ConnectionStrings__IdentityConnection`
 - `JwtSettings__Key` (IMPORTANTE: cambiar por un secret largo)
 - `MailSettings__*` (opcionales)
 

 
