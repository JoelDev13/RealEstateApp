FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RealEstateApp.sln ./
COPY RealEstateApp.Domain/RealEstateApp.Domain.csproj RealEstateApp.Domain/
COPY RealEstateApp.Application/RealEstateApp.Application.csproj RealEstateApp.Application/
COPY RealEstateApp.Infraestructure.Persistence/RealEstateApp.Infraestructure.Persistence.csproj RealEstateApp.Infraestructure.Persistence/
COPY RealEstateApp.Infraestructure.Identity/RealEstateApp.Infraestructure.Identity.csproj RealEstateApp.Infraestructure.Identity/
COPY RealEstateApp.Infraestructure.Shared/RealEstateApp.Infraestructure.Shared.csproj RealEstateApp.Infraestructure.Shared/
COPY RealEstateApp.WebApp/RealEstateApp.WebApp.csproj RealEstateApp.WebApp/

RUN dotnet restore RealEstateApp.sln

COPY . .

RUN dotnet publish RealEstateApp.WebApp/RealEstateApp.WebApp.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RealEstateApp.WebApp.dll"]
