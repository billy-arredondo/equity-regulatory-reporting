# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Equity regulatory reporting system that calculates company ownership participation chains and generates reports for a regulatory entity. The repository contains a .NET 10 backend (`/api`) and a React frontend (`/app`), developed on separate branches.

## Package Manager Rule

**Never use `npm`. Always use `pnpm`** for all frontend operations — install, add, run, etc. This is a hard project rule due to a security vulnerability in npm.

## Backend Commands (`/api/equity-regulatory-reporting/`)

```bash
# Build entire solution
dotnet build

# Run the API (auto-migrates + seeds in Development)
dotnet run --project equity-regulatory-reporting.Api

# Add a migration (run from the solution root)
dotnet ef migrations add <MigrationName> \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api \
  --output-dir Migrations

# Remove last migration
dotnet ef migrations remove \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api

# Apply migrations manually
dotnet ef database update \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api

# Start local PostgreSQL
cd ../   # go to /api/
docker compose up -d
```

The `ConnectionStrings__DefaultConnection` environment variable overrides the appsettings value for EF CLI tools.

## Frontend Commands (`/app/equity-regulatory-reporting-app/`)

```bash
pnpm install          # install dependencies
pnpm dev              # start dev server (localhost:5173)
pnpm build            # tsc -b && vite build
pnpm lint             # eslint
pnpm preview          # preview production build
```

## Backend Architecture

Five projects under `/api/equity-regulatory-reporting/`, organized as Clean Architecture:

```
Domain          ← no dependencies
Application     ← Domain
Persistence     ← Application, Domain
Infrastructure  ← Application, Domain, Persistence
Api             ← Application, Infrastructure, Persistence
```

**Domain** — Entities (`Person`, `Country`, `DocumentType`, `DocumentTypePersonType`, `Participation`, `Position`, `BoardMember`), enums (`PersonType`, `Permission`), and `AuditableEntity` base class. No external dependencies.

**Application** — CQRS via MediatR. Each feature lives in `Auth/Commands/<Name>/` with three files: `<Name>Command.cs` (record + result record), `<Name>CommandHandler.cs`, `<Name>CommandValidator.cs`. Interfaces (`IRepository<T>`, `ICurrentUser`, `IJwtTokenService`, etc.) are defined here and implemented elsewhere. The `ValidationBehavior<,>` MediatR pipeline behavior runs all `IValidator<TRequest>` automatically before every handler — **handlers never call validation explicitly**.

**Persistence** — `ApplicationDbContext` extends `IdentityDbContext<ApplicationUser, ApplicationRole, Guid>`. One `IEntityTypeConfiguration<T>` per entity in `Configurations/`. Identity classes (`ApplicationUser`, `ApplicationRole`, `RefreshToken`) live in `Persistence/Identity/`. Generic `Repository<T>` is registered as an open generic (`AddScoped(typeof(IRepository<>), typeof(Repository<>))`). `AuditableEntityInterceptor` fills audit fields from `ICurrentUser` on every `SaveChanges`.

**Infrastructure** — JWT token generation/validation, refresh token rotation (SHA-256 hashed, stored in DB), `IdentityService` (wraps `UserManager`/`RoleManager`), `CurrentUser` (reads from `IHttpContextAccessor`), and the permission authorization system.

**Api** — `Program.cs` wires all three `AddX()` extension methods, registers Swashbuckle with JWT Bearer, and runs `MigrateAsync()` + seeding on Development startup. `ValidationExceptionHandler` maps `FluentValidation.ValidationException` → HTTP 400 problem details.

### Package Versioning

All NuGet versions are centralized in `Directory.Packages.props` (solution root). Add new packages with `dotnet add package <Name>` — it auto-updates both the `.csproj` (version-less `<PackageReference>`) and `Directory.Packages.props`. Never put `Version=` attributes on `<PackageReference>` elements.

### Permission System

`Permission` is a `[Flags]` enum stored as `int`. Roles have a `Permissions` property (bitwise OR of allowed values). The JWT access token carries a single `perm` claim (the int). `HasPermissionAttribute(Permission.X)` on a controller action triggers `PermissionPolicyProvider` → `PermissionAuthorizationHandler`, which checks `(claimValue & required) != 0`.

### UUID v7 Primary Keys

All PKs are `Guid` initialized via `Guid.CreateVersion7()` (time-ordered, index-friendly). Identity tables also use `Guid` keys. The EF configuration maps them as `uuid` in PostgreSQL.

### Database Conventions

`UseSnakeCaseNamingConvention()` (EFCore.NamingConventions) applies globally — all table and column names in PostgreSQL are snake_case. DateTimeOffset maps to `timestamptz`. Check constraints on `Participation`: percentage in `[0,100]` and `effective_to >= effective_from`.

## Frontend Architecture

React 19 + Vite 8 + TypeScript 6 in `/app/equity-regulatory-reporting-app/`. Planned stack (from spec): Zustand for sidebar-collapse and theme persistence (localStorage), TanStack Query for server state, Shadcn/Tweakcn + Tailwind for UI, Lucide React for icons.

Planned routes: `/persons`, `/person-types` (read-only enum), `/document-types`, `/countries`, `/participations`, `/board`, `/positions`, `/users`.

The UI must have a collapsible left sidebar (icons-only when collapsed, state persisted in localStorage via Zustand) and a light/dark theme (also persisted via Zustand).

## Code Language Convention

All code must be written in English — identifiers, enum values, comments, variable names, class names — regardless of the surrounding language of documentation or commit messages (which may be in Spanish).

## Spec

`docs/spec/spec-01.md` is the source of truth for entity definitions, business rules, and technical decisions. Calculation algorithm and report format are deferred to a future spec.
