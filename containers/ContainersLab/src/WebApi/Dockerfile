FROM mcr.microsoft.com/dotnet/sdk:7.0.304-bullseye-slim-amd64 AS build
WORKDIR /src
COPY . .
RUN dotnet publish WebApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0.7-bullseye-slim-amd64 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS="http://+"
EXPOSE 80
ENTRYPOINT ["dotnet", "/app/WebApi.dll"]