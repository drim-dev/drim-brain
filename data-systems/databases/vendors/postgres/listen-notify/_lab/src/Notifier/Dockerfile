FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./Notifier/Notifier.csproj", "./Notifier/"]
RUN dotnet restore "./Notifier/Notifier.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./Notifier/Notifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Notifier/Notifier.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifier.dll"]
