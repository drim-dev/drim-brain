FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ResourcesLab.csproj", "./"]
RUN dotnet restore "ResourcesLab.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ResourcesLab.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResourcesLab.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5555
ENTRYPOINT ["dotnet", "ResourcesLab.dll"]
