FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ./CorsLab.WebApi/CorsLab.WebApi.csproj ./CorsLab.WebApi/
RUN dotnet restore ./CorsLab.WebApi/CorsLab.WebApi.csproj

COPY . .
RUN dotnet build ./CorsLab.WebApi/CorsLab.WebApi.csproj -c Release -o /app/build

RUN dotnet publish ./CorsLab.WebApi/CorsLab.WebApi.csproj -c Release -o /app/publish

FROM base AS final

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CorsLab.WebApi.dll"]
