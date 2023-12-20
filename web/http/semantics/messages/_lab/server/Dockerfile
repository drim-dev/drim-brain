FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MessagesLab.csproj", "./"]
RUN dotnet restore "MessagesLab.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MessagesLab.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagesLab.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5555
ENTRYPOINT ["dotnet", "MessagesLab.dll"]
