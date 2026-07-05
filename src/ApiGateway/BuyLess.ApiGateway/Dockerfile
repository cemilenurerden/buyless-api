FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ApiGateway/BuyLess.ApiGateway/BuyLess.ApiGateway.csproj", "src/ApiGateway/BuyLess.ApiGateway/"]

RUN dotnet restore "src/ApiGateway/BuyLess.ApiGateway/BuyLess.ApiGateway.csproj"

COPY . .

RUN dotnet publish "src/ApiGateway/BuyLess.ApiGateway/BuyLess.ApiGateway.csproj" \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BuyLess.ApiGateway.dll"]
