# Product Catalog API

ASP.NET Core 10 Web API for product management with JWT authentication.

Frontend: [product-catalog-web](https://github.com/sumertayoga/product-catalog-web)

## Tech stack

- .NET 10, ASP.NET Core (controllers)
- PostgreSQL 18 + EF Core
- Valkey (Redis-compatible) cache
- Serilog, Swagger UI
- Docker Compose

## Run with Docker (recommended)

Requires Docker.

1. `git clone https://github.com/sumertayoga/product-catalog-api.git && cd product-catalog-api`
2. Copy `.env.example` to `.env` and set `POSTGRES_PASSWORD` and `JWT_KEY` (any random string, at least 32 characters)
3. `docker compose up --build`
4. Open http://localhost:8080/swagger

The database schema is created automatically on startup.

## Run with dotnet

Requires the .NET 10 SDK and a PostgreSQL server.

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=productcatalog;Username=postgres;Password=<password>" --project src/ProductCatalog.Api
dotnet user-secrets set "Jwt:Key" "<random string, at least 32 characters>" --project src/ProductCatalog.Api
dotnet run --project src/ProductCatalog.Api --launch-profile http
```

Swagger: http://localhost:5183/swagger. Without Redis configured, an in-memory cache is used.

## Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | - | Register, returns a JWT |
| POST | `/api/auth/login` | - | Login, returns a JWT |
| GET | `/api/products?name=&minPrice=&maxPrice=&page=&pageSize=` | JWT | List / search products (paged) |
| GET | `/api/products/{id}` | JWT | Get a product |
| POST | `/api/products` | JWT | Create a product |
| PUT | `/api/products/{id}` | JWT | Update a product |
| DELETE | `/api/products/{id}` | JWT | Delete a product |
| GET | `/health` | - | Health check (database) |

Errors are returned as ProblemDetails (`400` validation, `401`, `404`, `409`, `500`).
Sample requests are in [`ProductCatalog.Api.http`](src/ProductCatalog.Api/ProductCatalog.Api.http).

## Architecture

Clean Architecture with four projects. Dependencies point inward:

```
Api  ->  Infrastructure  ->  Application  ->  Domain
```

- **Domain**: entities (`Product`, `User`) and their rules.
- **Application**: use cases as CQRS commands and queries with handlers, plus interfaces for persistence, cache, password hashing and tokens. No EF Core or ASP.NET dependencies.
- **Infrastructure**: EF Core/PostgreSQL repositories, Redis cache, JWT and password hashing.
- **Api**: controllers, authentication, error handling, logging.

## Caching

- Product by id is cached for 5 minutes, search results for 1 minute.
- Create/update/delete clear the product entry and invalidate all cached searches (via a version key).
- If the cache is unavailable, the API logs a warning and reads from the database.

## Assumptions

- All product endpoints require a logged-in user; there are no roles.
- Migrations run automatically on startup for easy setup.
- HTTPS is expected to be handled by the hosting platform or a reverse proxy.
