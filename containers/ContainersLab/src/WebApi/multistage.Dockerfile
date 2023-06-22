# ALL LAYERS FROM IMAGE
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /src

# NEW LAYER
COPY ./WebApi/WebApi.csproj ./WebApi/

# NEW LAYER
RUN dotnet restore --no-cache --no-dependencies WebApi/WebApi.csproj

# NEW LAYER
COPY . .

RUN dotnet publish -c Release --no-restore WebApi/WebApi.csproj

# ALL LAYERS FROM IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# NEW LAYER
COPY --from=build /src/WebApi/bin/Release/net7.0/publish /app

ENV ASPNETCORE_URLS http://*:80

ENTRYPOINT ["dotnet", "WebApi.dll"]
