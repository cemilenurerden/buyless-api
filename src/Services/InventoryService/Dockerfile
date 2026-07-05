FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/InventoryService/InventoryService.API/InventoryService.API.csproj", "src/Services/InventoryService/InventoryService.API/"]
COPY ["src/Services/InventoryService/InventoryService.Application/InventoryService.Application.csproj", "src/Services/InventoryService/InventoryService.Application/"]
COPY ["src/Services/InventoryService/InventoryService.Domain/InventoryService.Domain.csproj", "src/Services/InventoryService/InventoryService.Domain/"]
COPY ["src/Services/InventoryService/InventoryService.Infrastructure/InventoryService.Infrastructure.csproj", "src/Services/InventoryService/InventoryService.Infrastructure/"]
COPY ["src/Shared/BuyLess.Shared/BuyLess.Shared.csproj", "src/Shared/BuyLess.Shared/"]

RUN dotnet restore "src/Services/InventoryService/InventoryService.API/InventoryService.API.csproj"

COPY . .

RUN dotnet publish "src/Services/InventoryService/InventoryService.API/InventoryService.API.csproj" \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.API.dll"]
