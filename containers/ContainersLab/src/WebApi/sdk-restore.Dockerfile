# ALL LAYERS FROM IMAGE
FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /src

# NEW LAYER
COPY ./WebApi/WebApi.csproj ./WebApi/

# NEW LAYER
RUN dotnet restore --no-cache --no-dependencies WebApi/WebApi.csproj

# NEW LAYER
COPY . .

# NEW LAYER
RUN dotnet publish -c Release --no-restore WebApi/WebApi.csproj

# NEW LAYER
RUN cp -r WebApi/bin/Release/net7.0/publish /app

ENV ASPNETCORE_URLS http://*:80

ENTRYPOINT ["dotnet", "/app/WebApi.dll"]