FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ./CorsLab.WebApp/CorsLab.WebApp.csproj ./CorsLab.WebApp/
RUN dotnet restore ./CorsLab.WebApp/CorsLab.WebApp.csproj

COPY . .
RUN dotnet build ./CorsLab.WebApp/CorsLab.WebApp.csproj -c Release -o /app/build

RUN dotnet publish ./CorsLab.WebApp/CorsLab.WebApp.csproj -c Release -o /app/publish

FROM base AS final

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CorsLab.WebApp.dll"]
