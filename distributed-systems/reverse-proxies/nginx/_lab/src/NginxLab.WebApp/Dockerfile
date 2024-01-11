FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ./NginxLab.WebApp/NginxLab.WebApp.csproj ./NginxLab.WebApp/
RUN dotnet restore ./NginxLab.WebApp/NginxLab.WebApp.csproj

COPY . .
RUN dotnet build ./NginxLab.WebApp/NginxLab.WebApp.csproj -c Release -o /app/build

RUN dotnet publish ./NginxLab.WebApp/NginxLab.WebApp.csproj -c Release -o /app/publish

FROM base AS final

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NginxLab.WebApp.dll"]
