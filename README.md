# .NET Clean Architecture API

**Built by Pratik Sheth**

A sample .NET 8 Web API that demonstrates **Clean Architecture** over a small
product catalog and ordering domain. The goal is a codebase where business
rules live in the center, frameworks and infrastructure sit at the edges, and
every layer can be tested in isolation.

## Architecture

```
        ┌──────────────────────────────────────────────┐
        │                      Api                      │  HTTP, controllers,
        │        (CleanArchitecture.Api)                │  JWT auth, Swagger
        └───────────────┬──────────────────────────────┘
                        │ depends on
        ┌───────────────▼──────────────────────────────┐
        │                 Application                   │  Use cases, DTOs,
        │     (CleanArchitecture.Application)           │  service + repository
        │                                               │  abstractions
        └───────┬───────────────────────▲──────────────┘
                │ depends on              │ implements
        ┌───────▼──────────────┐  ┌───────┴──────────────┐
        │       Domain         │  │    Infrastructure     │  EF Core (SQLite),
        │ (CleanArchitecture.  │  │ (CleanArchitecture.   │  repositories,
        │      .Domain)        │  │   .Infrastructure)    │  JWT token service
        └──────────────────────┘  └───────────────────────┘
```

- **Domain** — entities (`Product`, `Order`, `OrderItem`) and enums. No
  dependencies on any framework or other layer.
- **Application** — use-case services (`ProductService`, `OrderService`),
  DTOs, and the abstractions they depend on (`IProductRepository`,
  `IOrderRepository`, `IJwtTokenService`). Business rules such as input
  validation, stock checks, and order totals live here.
- **Infrastructure** — EF Core `AppDbContext` (SQLite provider), repository
  implementations, and the JWT token service. Everything replaceable: swap
  SQLite for SQL Server/PostgreSQL by changing one registration.
- **Api** — thin controllers, JWT bearer authentication, Swagger/OpenAPI,
  and a global exception-handling middleware that converts application
  exceptions into consistent JSON error responses (404 for
  `NotFoundException`, 400 for `ValidationException`, 500 otherwise).

## Features

- Product CRUD and order placement with stock validation and price snapshots
- EF Core with SQLite — runs anywhere, no database server required; seed data
  is created automatically on first run
- JWT authentication stub (see below) protecting all write endpoints
- Swagger UI with Bearer token support, served at the app root
- Global error-handling middleware
- xUnit unit tests for the Application layer using in-memory repository fakes

## Getting started

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
dotnet restore
dotnet run --project src/Api
```

Then open the Swagger UI at the printed URL (root path), e.g.
`https://localhost:5001/`.

Run the tests:

```bash
dotnet test
```

## Authentication (demo stub)

Write endpoints require a JWT. Get one from the demo login endpoint:

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "demo", "password": "demo123"}'
```

> The credentials and signing key in `appsettings.json` are **demo-only
> placeholders** so the sample runs out of the box. A production system would
> validate users against a real identity provider (e.g. Microsoft Entra ID)
> and keep secrets in a vault — never in source control.

Send the returned token on subsequent calls:

```bash
curl https://localhost:5001/api/products \
  -X POST \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"name": "Webcam", "description": "1080p USB webcam", "price": 79.99, "stockQuantity": 15}'
```

## Endpoints

| Method | Route                | Auth | Description                    |
| ------ | -------------------- | ---- | ------------------------------ |
| POST   | `/api/auth/login`    | No   | Exchange demo credentials for a JWT |
| GET    | `/api/products`      | No   | List products                  |
| GET    | `/api/products/{id}` | No   | Get a product                  |
| POST   | `/api/products`      | Yes  | Create a product               |
| PUT    | `/api/products/{id}` | Yes  | Update a product               |
| DELETE | `/api/products/{id}` | Yes  | Delete a product               |
| GET    | `/api/orders`        | No   | List orders                    |
| GET    | `/api/orders/{id}`   | No   | Get an order with line items   |
| POST   | `/api/orders`        | Yes  | Place an order                 |

Example order:

```json
{
  "customerEmail": "customer@example.com",
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 1 }
  ]
}
```

## Project structure

```
src/
  Domain/          Entities and enums (no dependencies)
  Application/     Services, DTOs, interfaces, exceptions
  Infrastructure/  EF Core context, repositories, JWT service
  Api/             Controllers, middleware, Program.cs
tests/
  Application.Tests/  xUnit tests with in-memory repository fakes
```

## Author

**Pratik Sheth** — Principal Software Engineer
LinkedIn: https://www.linkedin.com/in/pratik-s-a85a3317
Location: Buena Park, CA

## License

MIT — see [LICENSE](LICENSE).
