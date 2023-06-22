# ALL LAYERS FROM IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

# NEW LAYER
COPY bin/Release/net7.0/publish/ .

ENV ASPNETCORE_URLS http://*:80

ENTRYPOINT ["dotnet", "/app/WebApi.dll"]