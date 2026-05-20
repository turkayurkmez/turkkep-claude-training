# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Workspace Structure

```
project/
├── api/        # .NET 10 Web API (backend)
└── client/     # Angular 20 SPA (frontend)
```

Sub-project guidance:
- Backend rules: see below
- Frontend rules: [client/CLAUDE.md](client/CLAUDE.md)

## Common Commands

```bash
# Build
dotnet build api/

# Run (HTTP on http://localhost:5015)
dotnet run --project api/ --launch-profile http

# Run (HTTPS on https://localhost:7052)
dotnet run --project api/ --launch-profile https
```

OpenAPI schema is available at `http://localhost:5015/openapi/v1.json` in Development.

## Backend Architecture (`api/`)

Three mandatory layers — **Controller → Service → Repository**:

- **Controller**: Routing, input validation, response mapping only. Cannot access repositories directly.
- **Service**: All business logic lives here. The only layer allowed to call repositories.
- **Repository**: Data access. Must implement `IRepository<T>`.

### Naming Conventions

- Types, methods, properties: `PascalCase`
- Local variables, parameters: `camelCase`
- Private fields: `_camelCase`
- Class and interface names must reflect a single responsibility (SRP)

### Async Rules

- `async`/`await` is mandatory for all I/O operations
- `ConfigureAwait(false)` is for library code only — never in application code
- All `public async` methods must accept a `CancellationToken` parameter

```csharp
// Correct
public async Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken)
```

## Protected Files

Never modify:
- `api/appsettings.Production.json`
