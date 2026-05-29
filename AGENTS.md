# AGENTS.md

Agent quick-reference for this repo. Every item here is something an agent would likely get wrong without it.

---

## Layout

```
/api/                                  ← docker-compose.yml lives HERE (not inside the solution)
/api/equity-regulatory-reporting/      ← .NET 10 solution root (5 projects)
/app/equity-regulatory-reporting-app/  ← React 19 frontend
/docs/spec/spec-01.md                  ← business-rule source of truth
```

The two stacks were developed on separate branches. There is currently no test suite.

---

## Hard Rules

- **Never use `npm`.** Always use `pnpm` for all frontend operations — security policy.
- **Never put `Version=` on `<PackageReference>`.** All NuGet versions are centralized in `Directory.Packages.props`. Use `dotnet add package <Name>` and it updates both files automatically.
- **All code identifiers in English** — commit messages/docs may be Spanish, but identifiers, comments, enum values, and class names must be English.

---

## Starting the Dev Environment

### Backend (order matters)

```bash
# 1. Start PostgreSQL — run from /api/, NOT from inside the solution folder
cd api
docker compose up -d

# 2. Run the API — auto-migrates and seeds in Development
cd equity-regulatory-reporting
dotnet run --project equity-regulatory-reporting.Api
```

The API listens on `http://localhost:5074`. In `Development`, `MigrateAsync()` and `SeedAsync()` run at startup automatically — no manual `dotnet ef database update` needed during development.

Default seeded admin: `admin@example.com` / `Admin1234!`

### Frontend

```bash
cd app/equity-regulatory-reporting-app
pnpm install
pnpm dev   # http://localhost:5173
```

Vite proxies all `/api` requests to `http://localhost:5074`. Frontend code should use `/api/...` paths only — never hardcode the backend port.

---

## EF Migrations

Always run from the solution root (`/api/equity-regulatory-reporting/`) with both flags:

```bash
dotnet ef migrations add <Name> \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api \
  --output-dir Migrations

dotnet ef migrations remove \
  --project equity-regulatory-reporting.Persistence \
  --startup-project equity-regulatory-reporting.Api
```

Override the connection string for CLI tools via env var:
```
ConnectionStrings__DefaultConnection=<connection string>
```

---

## Backend Architecture Quirks

**Clean Architecture layer order** (deps flow one way only):
```
Domain ← Application ← Persistence ← Infrastructure ← Api
```

**Validation is automatic — never call it in handlers.** `ValidationBehavior<,>` is a MediatR pipeline behavior that runs all `IValidator<TRequest>` before every handler. Just create the validator class.

**Command result types** are defined in the same file as the command (`<Name>Command.cs`), not separately.

**Permissions** use a `[Flags]` enum stored as `int`. JWT carries a single `perm` claim; authorization checks `(claimValue & required) != 0`. No role-string checks.

**API docs** served at `/scalar` (Scalar, not Swagger UI). Uses native .NET 10 `Microsoft.AspNetCore.OpenApi` — not Swashbuckle.

---

## Database Conventions

- All table/column names are **`snake_case`** (`UseSnakeCaseNamingConvention()` applied globally).
- All PKs are `Guid` initialized with **`Guid.CreateVersion7()`** (time-ordered) — never `Guid.NewGuid()`.
- `DateTimeOffset` maps to `timestamptz`.
- PostgreSQL 17 Alpine, default creds: `postgres`/`postgres`, DB: `equity_regulatory`.

---

## Frontend Stack (actual installed packages)

- React 19, React Router DOM 7, TanStack Query 5, Zustand 5
- Tailwind CSS 4 via `@tailwindcss/vite` (not PostCSS-based)
- Shadcn CLI (`shadcn ^4.8.2`) + `@base-ui/react` — components are generated into the project by the CLI, not imported from a package
- `next-themes`, `sonner`, `vaul`, `cmdk`
- `@fontsource-variable/geist` (local font, not Google Fonts)
- TypeScript 6 (`~6.0.2` pinned), Vite 8, `@vitejs/plugin-react` v6 (Oxc transformer, not SWC)
- Path alias `@` → `./src` (configured in both `vite.config.ts` and `tsconfig.json`)

`pnpm build` runs `tsc -b && vite build` — TypeScript is checked before bundling.

---

## CORS

Allowed origins are config-driven. In Development, `appsettings.Development.json` sets `Cors:AllowedOrigins` to `["http://localhost:5173"]`. The Vite proxy also rewrites cookie domains via `cookieDomainRewrite: "localhost"` — required for refresh-token cookies to work across the proxy.
