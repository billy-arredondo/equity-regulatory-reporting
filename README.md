# Equity Regulatory Reporting

System for calculating company ownership participation chains and generating reports for a regulatory entity. It tracks shareholding structures, board member records, and entity hierarchies across legal persons and legal entities.

## Stack

| Layer            | Technology                                           |
| ---------------- | ---------------------------------------------------- |
| Backend          | .NET 10, ASP.NET Core, Entity Framework Core 10      |
| Database         | PostgreSQL 17                                        |
| Auth             | JWT (access token) + HttpOnly cookie (refresh token) |
| Frontend         | React 19, Vite 8, TypeScript 6                       |
| UI               | Tailwind CSS v4, Shadcn/Tweakcn, Lucide React        |
| State            | TanStack Query (server), Zustand (UI)                |
| Containerization | Docker Compose (database only)                       |

## Repository layout

```
/
├── api/
│   ├── docker-compose.yml                     ← PostgreSQL container (run from here)
│   └── equity-regulatory-reporting/           ← .NET 10 solution root
│       ├── equity-regulatory-reporting.Domain
│       ├── equity-regulatory-reporting.Application
│       ├── equity-regulatory-reporting.Persistence
│       ├── equity-regulatory-reporting.Infrastructure
│       └── equity-regulatory-reporting.Api
├── app/
│   └── equity-regulatory-reporting-app/       ← React frontend
└── docs/
    └── spec/
        └── spec-01.md                         ← Entity definitions and business rules
```

### Backend architecture

Five projects organized as Clean Architecture, with a strict one-way dependency rule:

```
Domain          ← no external dependencies
Application     ← Domain
Persistence     ← Application, Domain
Infrastructure  ← Application, Domain, Persistence
Api             ← Application, Infrastructure, Persistence
```

All features in the Application layer follow the CQRS pattern via MediatR, grouped under `Features/<FeatureName>/Commands/` and `Features/<FeatureName>/Queries/`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)
- [Node.js 20+](https://nodejs.org/) and [pnpm](https://pnpm.io/)

> **Note:** The frontend uses `pnpm` exclusively. Do not use `npm`.

## Getting started

### 1. Start the database

```bash
cd api
docker compose up -d
```

This starts a PostgreSQL 17 container on port `5432` with the default credentials below.

### 2. Run the API

```bash
cd api/equity-regulatory-reporting
dotnet run --project equity-regulatory-reporting.Api
```

In `Development`, the API automatically runs pending EF Core migrations and seeds initial data on startup. The server listens on `http://localhost:5074`.

### 3. Run the frontend

```bash
cd app/equity-regulatory-reporting-app
pnpm install
pnpm dev
```

The app is available at `http://localhost:5173`.

## Default credentials

| Setting            | Value                   |
| ------------------ | ----------------------- |
| Admin email        | `admin@example.com`     |
| Admin password     | `Admin1234!`            |
| DB host            | `localhost:5432`        |
| DB name            | `equity_regulatory`     |
| DB user / password | `postgres` / `postgres` |

## Configuration

Key settings in `appsettings.json` / `appsettings.Development.json`:

| Key                                      | Description                                      |
| ---------------------------------------- | ------------------------------------------------ |
| `ConnectionStrings:DefaultConnection`    | PostgreSQL connection string                     |
| `Jwt:SigningKey`                         | HMAC-SHA256 secret (minimum 32 characters)       |
| `Jwt:AccessTokenMinutes`                 | Access token lifetime (default: 5 min)           |
| `Jwt:RefreshTokenDays`                   | Refresh token lifetime (default: 7 days)         |
| `Seed:AdminEmail` / `Seed:AdminPassword` | Credentials for the bootstrapped admin user      |
| `Seed:Enabled`                           | Set to `false` to skip seeding on startup        |
| `Cors:AllowedOrigins`                    | Comma-separated list of allowed frontend origins |

The `ConnectionStrings__DefaultConnection` environment variable overrides the appsettings value for EF CLI commands.

## Backend commands

All commands run from `api/equity-regulatory-reporting/`:

```bash
# Build the entire solution
dotnet build

# Add a migration
dotnet ef migrations add <MigrationName> \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api \
  --output-dir Migrations

# Remove the last migration
dotnet ef migrations remove \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api

# Apply migrations manually
dotnet ef database update \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api
```

## Frontend commands

All commands run from `app/equity-regulatory-reporting-app/`:

```bash
pnpm install       # install dependencies
pnpm dev           # start dev server → http://localhost:5173
pnpm build         # type-check + production build
pnpm lint          # run ESLint
pnpm test          # run unit tests (Vitest)
pnpm preview       # preview the production build
```

## Modules

| Route             | Description                                             |
| ----------------- | ------------------------------------------------------- |
| `/people`         | Natural persons                                         |
| `/companies`      | Legal persons — includes Participations and Board tabs  |
| `/entities`       | Legal entities — includes Participations and Board tabs |
| `/person-types`   | Person type reference (read-only enum)                  |
| `/document-types` | Document type catalog                                   |
| `/countries`      | Country catalog                                         |
| `/positions`      | Corporate position catalog                              |
| `/users`          | User management (requires `UserRead` permission)        |

Shareholding participations and board member records are managed from within each company or entity's detail page, not as standalone top-level modules.

## Permission system

Permissions are stored as a `[Flags]` integer on each role and embedded in the JWT access token as a single `perm` claim. Controllers use `[HasPermission(Permission.X)]`, which triggers a policy handler that checks `(claimValue & required) != 0`. The frontend mirrors this with a `PermissionGuard` component and a `hasPermission(perm)` helper from the auth store.

## Design decisions

- **UUID v7 primary keys** — time-ordered, index-friendly, generated via `Guid.CreateVersion7()`.
- **Snake_case in PostgreSQL** — applied globally via `UseSnakeCaseNamingConvention()`.
- **Centralized NuGet versions** — all package versions live in `Directory.Packages.props`. Never add `Version=` to a `<PackageReference>` element.
- **MediatR validation pipeline** — `ValidationBehavior<,>` runs all FluentValidation validators before every handler. Handlers never call validation explicitly.
- **Refresh token rotation** — tokens are SHA-256 hashed before storage and rotated on every use.
- **pnpm enforced** — `npm` is forbidden in this project due to a security policy. All frontend package operations must use `pnpm`.
