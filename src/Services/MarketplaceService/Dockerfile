FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/MarketplaceService/MarketplaceService.API/MarketplaceService.API.csproj", "src/Services/MarketplaceService/MarketplaceService.API/"]
COPY ["src/Services/MarketplaceService/MarketplaceService.Application/MarketplaceService.Application.csproj", "src/Services/MarketplaceService/MarketplaceService.Application/"]
COPY ["src/Services/MarketplaceService/MarketplaceService.Domain/MarketplaceService.Domain.csproj", "src/Services/MarketplaceService/MarketplaceService.Domain/"]
COPY ["src/Services/MarketplaceService/MarketplaceService.Infrastructure/MarketplaceService.Infrastructure.csproj", "src/Services/MarketplaceService/MarketplaceService.Infrastructure/"]
COPY ["src/Shared/BuyLess.Shared/BuyLess.Shared.csproj", "src/Shared/BuyLess.Shared/"]

RUN dotnet restore "src/Services/MarketplaceService/MarketplaceService.API/MarketplaceService.API.csproj"

COPY . .

RUN dotnet publish "src/Services/MarketplaceService/MarketplaceService.API/MarketplaceService.API.csproj" \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MarketplaceService.API.dll"]
