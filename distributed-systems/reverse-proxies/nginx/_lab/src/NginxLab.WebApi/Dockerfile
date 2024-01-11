FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ./NginxLab.WebApi/NginxLab.WebApi.csproj ./NginxLab.WebApi/
RUN dotnet restore ./NginxLab.WebApi/NginxLab.WebApi.csproj

COPY . .
RUN dotnet build ./NginxLab.WebApi/NginxLab.WebApi.csproj -c Release -o /app/build

RUN dotnet publish ./NginxLab.WebApi/NginxLab.WebApi.csproj -c Release -o /app/publish

FROM base AS final

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NginxLab.WebApi.dll"]
