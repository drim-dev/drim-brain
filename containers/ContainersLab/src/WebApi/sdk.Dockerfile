# ALL LAYERS FROM IMAGE
FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /src

# NEW LAYER
COPY . .

# NEW LAYER
RUN dotnet publish -c Release WebApi.csproj

# NEW LAYER
RUN cp -r bin/Release/net7.0/publish /app

RUN rm -rf /src

ENV ASPNETCORE_URLS http://*:80

ENTRYPOINT ["dotnet", "/app/WebApi.dll"]