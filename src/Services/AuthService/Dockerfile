FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/AuthService/AuthService.API/AuthService.API.csproj", "src/Services/AuthService/AuthService.API/"]
COPY ["src/Services/AuthService/AuthService.Application/AuthService.Application.csproj", "src/Services/AuthService/AuthService.Application/"]
COPY ["src/Services/AuthService/AuthService.Domain/AuthService.Domain.csproj", "src/Services/AuthService/AuthService.Domain/"]
COPY ["src/Services/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "src/Services/AuthService/AuthService.Infrastructure/"]
COPY ["src/Shared/BuyLess.Shared/BuyLess.Shared.csproj", "src/Shared/BuyLess.Shared/"]

RUN dotnet restore "src/Services/AuthService/AuthService.API/AuthService.API.csproj"

COPY . .

RUN dotnet publish "src/Services/AuthService/AuthService.API/AuthService.API.csproj" \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
