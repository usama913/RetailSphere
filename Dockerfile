FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./RetailSphere.sln"
RUN dotnet build "./src/RetailSphere.API/RetailSphere.API.csproj" --configuration $BUILD_CONFIGURATION --no-restore

FROM build AS test
CMD ["dotnet", "test", "./RetailSphere.sln", "--configuration", "Release"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/RetailSphere.API/RetailSphere.API.csproj" --configuration $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetailSphere.API.dll"]
