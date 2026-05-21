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

## Güvenlik Kuralları

### Kimlik Doğrulama ve Yetkilendirme
- Her yeni controller ve endpoint `[Authorize]` ile koru; açık erişim gerektiren endpoint'leri `[AllowAnonymous]` ile **açıkça** işaretle.
- `Program.cs`'de `app.UseAuthorization()` çağrısından önce `app.UseAuthentication()` olduğunu doğrula.

### Veri İfşası
- API yanıtlarında her zaman ayrı bir response DTO tipi kullan; domain/entity sınıfını doğrudan serialize etmek yerine servis katmanında açık bir mapping yap.
- Sayfalama olmadan tüm kayıtları döndüren metotlar yerine her zaman `GetPagedAsync` kullan; `GetAll()` gibi sınırsız veri döndüren metotları interface'den çıkar.
- LINQ sorgularını repository sınırını geçmeden önce `.ToList()` ile materyalize et; `IEnumerable<T>` döndüren lazy sorgular servis veya controller katmanına taşınmamalı.

### Girdi Doğrulama
- Tüm model property'lerine uygun data annotation ekle: string alanlar için `[StringLength]`, e-posta için `[EmailAddress]`, sayısal aralıklar için `[Range]`.
- Uygun olan tüm validasyonlar için FluentValidation kullan; özellikle karmaşık doğrulama kuralları veya cross-field validasyon gerektiren durumlarda.
- Sayfalama parametrelerinde her zaman hem alt hem üst sınır uygula; `Math.Clamp(value, min, max)` kullan.

### Hata Yönetimi ve Bilgi Sızıntısı
- Global exception handler middleware ekle; hata yanıtlarında stack trace, dosya yolu veya sınıf adı yerine genel bir mesaj döndür.
- `AllowedHosts` ayarını spesifik domain adlarıyla sınırla; `"*"` değerini yalnızca geliştirme ortamında kabul et.
- OpenAPI/Swagger endpoint'ini yalnızca `IsDevelopment()` koşuluyla etkinleştir.

### Güvenlik Header'ları ve Taşıma Güvenliği
- Production ortamında `app.UseHsts()` ekle; `app.UseHttpsRedirection()` her ortamda aktif olsun.
- Middleware pipeline'ına `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: strict-origin-when-cross-origin` header'larını ekle.

### Rate Limiting
- Dışarıya açık her endpoint için `AddRateLimiter` ile bir politika tanımla ve `[EnableRateLimiting]` ile uygula.

### Loglama
- Servis katmanındaki tüm veri erişim operasyonlarını `ILogger` ile kaydet; log mesajlarında kimin, ne zaman, hangi parametrelerle sorgu yaptığı izlenebilir olsun.
- Güvenlik olaylarını (başarısız yetkilendirme, geçersiz girdi) ayrı bir log kategorisiyle kaydet.

### Mimari
- Repository interface'leri `Models` namespace'inden bağımsız olsun; `PagedQuery` gibi domain tipler yerine ham parametreler (`int skip, int take`) al, mapping servis katmanında yap.
- DI yaşam döngüsüyle tutarlı ol: `AddScoped` ile kayıtlı sınıflarda `static` field kullanma; paylaşılan state varsa `AddSingleton` tercih et.
- `async` metot imzası taşıyan her metot gerçek bir I/O `await` içersin; `await Task.CompletedTask` veya `Task.FromResult` kullanan sahte async metodlar gerçek implementasyon gelene kadar senkron imzayla yaz.
