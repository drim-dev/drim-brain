FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./TcpServer/TcpServer.csproj", "./TcpServer/"]
RUN dotnet restore "./TcpServer/TcpServer.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./TcpServer/TcpServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./TcpServer/TcpServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TcpServer.dll"]
