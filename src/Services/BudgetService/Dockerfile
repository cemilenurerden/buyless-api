FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/BudgetService/BudgetService.API/BudgetService.API.csproj", "src/Services/BudgetService/BudgetService.API/"]
COPY ["src/Services/BudgetService/BudgetService.Application/BudgetService.Application.csproj", "src/Services/BudgetService/BudgetService.Application/"]
COPY ["src/Services/BudgetService/BudgetService.Domain/BudgetService.Domain.csproj", "src/Services/BudgetService/BudgetService.Domain/"]
COPY ["src/Services/BudgetService/BudgetService.Infrastructure/BudgetService.Infrastructure.csproj", "src/Services/BudgetService/BudgetService.Infrastructure/"]
COPY ["src/Shared/BuyLess.Shared/BuyLess.Shared.csproj", "src/Shared/BuyLess.Shared/"]

RUN dotnet restore "src/Services/BudgetService/BudgetService.API/BudgetService.API.csproj"

COPY . .

RUN dotnet publish "src/Services/BudgetService/BudgetService.API/BudgetService.API.csproj" \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BudgetService.API.dll"]
