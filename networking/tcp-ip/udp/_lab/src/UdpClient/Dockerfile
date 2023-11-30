FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./UdpClient/UdpClient.csproj", "./UdpClient/"]
RUN dotnet restore "./UdpClient/UdpClient.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./UdpClient/UdpClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./UdpClient/UdpClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UdpClient.dll"]
