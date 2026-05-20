# Project Overview

This workspace contains two sub-projects:
- `api/` — .NET 10 Web API (backend)
- `client/` — Angular application (frontend, not yet created)

---

## Backend (`api/`)

### Architecture

Three-layer structure: **Controller → Service → Repository**

| Katman | Sorumluluk |
|---|---|
| **Controller** | Routing, input validation, response mapping |
| **Service** | Business logic — the only layer that can call repositories |
| **Repository** | Data access — must implement `IRepository<T>` |

> Controllers **cannot** access repositories directly. All data access goes through a service.

### Naming Conventions

- Types, methods, properties: `PascalCase`
- Local variables, parameters: `camelCase`
- Private fields: `_camelCase` (underscore prefix)
- Every class and interface name must reflect a single responsibility (SRP)

### Async Rules

- `async`/`await` is mandatory for all I/O operations
- `ConfigureAwait(false)` is **only** for library code — never use it in application code
- All `public async` methods must accept a `CancellationToken` parameter

```csharp
// Correct
public async Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken)

// Wrong — missing CancellationToken
public async Task<Customer> GetByIdAsync(int id)
```

### Protected Files

The following files must **never** be modified:

- `api/appsettings.Production.json`
