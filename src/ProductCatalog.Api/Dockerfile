# Build context is the repository root (see docker-compose.yml).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so the package layer is cached until a project file changes.
COPY src/ProductCatalog.Domain/ProductCatalog.Domain.csproj src/ProductCatalog.Domain/
COPY src/ProductCatalog.Application/ProductCatalog.Application.csproj src/ProductCatalog.Application/
COPY src/ProductCatalog.Infrastructure/ProductCatalog.Infrastructure.csproj src/ProductCatalog.Infrastructure/
COPY src/ProductCatalog.Api/ProductCatalog.Api.csproj src/ProductCatalog.Api/
RUN dotnet restore src/ProductCatalog.Api/ProductCatalog.Api.csproj

COPY src/ src/
RUN dotnet publish src/ProductCatalog.Api/ProductCatalog.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
RUN mkdir logs && chown $APP_UID logs
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]
