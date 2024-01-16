FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./Listener/Listener.csproj", "./Listener/"]
RUN dotnet restore "./Listener/Listener.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./Listener/Listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Listener/Listener.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Listener.dll"]
