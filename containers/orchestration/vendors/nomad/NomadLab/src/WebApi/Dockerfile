FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /src

COPY ./WebApi/WebApi.csproj ./WebApi/

RUN dotnet restore --no-cache --no-dependencies WebApi/WebApi.csproj

COPY . .

RUN dotnet publish -c Release --no-restore WebApi/WebApi.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /src/WebApi/bin/Release/net7.0/publish /app

ENV ASPNETCORE_URLS http://*:80

ENTRYPOINT ["dotnet", "WebApi.dll"]
