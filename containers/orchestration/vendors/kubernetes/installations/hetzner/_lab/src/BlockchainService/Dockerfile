FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BlockchainService/BlockchainService.csproj", "./BlockchainService/"]
RUN dotnet restore "./BlockchainService/BlockchainService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./BlockchainService/BlockchainService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./BlockchainService/BlockchainService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockchainService.dll"]
