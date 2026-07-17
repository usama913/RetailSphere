FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./RetailSphere.sln"
RUN dotnet publish "./ui/RetailSphere.UI/RetailSphere.UI.csproj" --configuration $BUILD_CONFIGURATION -o /app/publish

# Blazor WASM builds to static files — served by nginx, no .NET runtime needed at runtime.
FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY ui/RetailSphere.UI/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
