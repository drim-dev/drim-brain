FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BankingService.csproj", "."]
RUN dotnet restore "BankingService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "BankingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:80
ENTRYPOINT ["dotnet", "BankingService.dll"]
