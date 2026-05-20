# Prompt Şablonları — Angular & .NET

---

## Şablon 1: Role Prompting — Mimari Karar

**Kullanım Durumu:** Yeni bir özellik veya katman eklemeden önce mimari yaklaşım belirlemek istediğinde.

**Şablon:**
```
Sen, 10+ yıl deneyimli bir .NET backend mimarısın. Clean Architecture, SOLID prensipleri ve DDD konularında uzmansın.

Aşağıdaki bağlamı incele:
- Proje: [PROJE_ADI]
- Mevcut katmanlar: [KATMAN_LİSTESİ]
- Eklenmek istenen özellik: [ÖZELLİK_TANIMI]

Şu soruyu yanıtla: Bu özelliği mevcut mimariye nasıl entegre edersin?

Kısıtlamalar:
- "Yapabilirsiniz" gibi belirsiz ifadeler kullanma; doğrudan hangi dosyanın değişeceğini söyle
- Teorik açıklama yapma; somut sınıf ve interface adları ver
- Birden fazla yaklaşım listeleme; sadece en uygun olanını öner

Yanıtını şu yapıda ver:
1. Etkilenecek dosyalar (tam yol ile)
2. Yeni oluşturulacak sınıflar/interface'ler
3. DI kayıt değişiklikleri
```

**Örnek Kullanım:**
```
Sen, 10+ yıl deneyimli bir .NET backend mimarısın...

- Proje: Türkkep API (.NET 10, Controller → Service → Repository)
- Mevcut katmanlar: Controllers, Services, Repositories, Models
- Eklenmek istenen özellik: Müşteri arama (ad ve email'e göre filtreleme)

Yanıtını şu yapıda ver:
1. Etkilenecek dosyalar: api/Repositories/IRepository.cs, api/Repositories/CustomerRepository.cs, api/Services/ICustomerService.cs, api/Services/CustomerService.cs, api/Controllers/CustomersController.cs
2. Yeni oluşturulacak: CustomerSearchQuery record (Models/)
3. DI kayıt değişiklikleri: Yok — mevcut Scoped kayıtlar yeterli
```

---

## Şablon 2: Chain of Thought — Hata Analizi

**Kullanım Durumu:** Bir runtime veya derleme hatası aldığında, kök nedeni bulmak ve doğru çözüme ulaşmak istediğinde.

**Şablon:**
```
Adım adım düşünerek aşağıdaki hatayı analiz et.

**Ortam:**
- Framework: [FRAMEWORK_VE_VERSİYON]
- Dosya: [DOSYA_YOLU]
- Hata mesajı:
[HATA_METNİ]

**İlgili kod:**
```[DİL]
[KOD_BLOĞU]
```

Şu adımları sırayla izle:
1. Hatanın tam olarak ne anlama geldiğini açıkla
2. Hatanın neden oluştuğunu tespit et (kök neden)
3. Yanlış olan varsayımı veya type uyuşmazlığını belirt
4. Çözümü uygula — sadece değişmesi gereken satırları göster, tüm dosyayı yeniden yazma
5. Aynı hatanın tekrarlamaması için ne yapılmalı?
```

**Örnek Kullanım:**
```
Adım adım düşünerek aşağıdaki hatayı analiz et.

**Ortam:**
- Framework: Angular 20, rxResource
- Dosya: src/app/features/customers/customer-list.ts
- Hata mesajı: TypeError: newCollection[Symbol.iterator] is not a function

**İlgili kod:**
```typescript
readonly customersResource = rxResource<Customer[], unknown>({
  stream: () => this._customerService.getCustomers(),
});
// template: @for (customer of customersResource.value(); ...)
```

1. Hata: @for, Symbol.iterator'ı olmayan bir nesne üzerinde iterasyon yapmaya çalışıyor
2. Kök neden: API PagedResult<Customer> döndürüyor; servis Customer[] bekliyor
3. Yanlış varsayım: rxResource generic tipi Customer[] ama gerçek response { items, page, ... }
4. Çözüm: Generic'i PagedResult<Customer> yap, @for'u value()!.items üzerinde çalıştır
5. Önlem: Servis dönüş tipini API response şemasıyla her zaman eşleştir
```

---

## Şablon 3: Structured Output — Kod İncelemesi

**Kullanım Durumu:** Yazdığın veya review etmen gereken bir kodu belirli standartlara göre değerlendirmek istediğinde.

**Şablon:**
```
Aşağıdaki [DİL] kodunu incele ve bulgularını belirtilen JSON formatında döndür. Başka hiçbir şey ekleme.

**İncelenecek kod:**
```[DİL]
[KOD_BLOĞU]
```

**Standartlar:**
[STANDART_LİSTESİ]

Yanıtı kesinlikle şu JSON formatında ver:
{
  "overall": "pass|warn|fail",
  "violations": [
    {
      "rule": "ihlal edilen kural",
      "line": 0,
      "severity": "error|warning|info",
      "current": "mevcut kod",
      "suggested": "önerilen kod"
    }
  ],
  "approved_patterns": ["doğru kullanılan pattern'lar"],
  "estimated_refactor_minutes": 0
}
```

**Örnek Kullanım:**
```
Aşağıdaki C# kodunu incele...

**Standartlar:**
- Controller Repository'ye doğrudan erişemez
- Tüm public async metodlar CancellationToken almalı
- Private field'lar _ prefix ile başlamalı
- Sınıf isimleri SRP'ye uymalı

Yanıt:
{
  "overall": "warn",
  "violations": [
    {
      "rule": "Tüm public async metodlar CancellationToken almalı",
      "line": 12,
      "severity": "warning",
      "current": "public async Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query)",
      "suggested": "public async Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken)"
    }
  ],
  "approved_patterns": ["Primary constructor DI", "IRepository<T> implementasyonu"],
  "estimated_refactor_minutes": 5
}
```

---

## Şablon 4: Negatif Kısıtlamalar — Component Geliştirme

**Kullanım Durumu:** Angular component yazarken proje kurallarına aykırı kalıpların (NgModule, eski decorator'lar, Zone.js bağımlı pattern'lar) üretilmesini engellemek istediğinde.

**Şablon:**
```
Angular 20 ile bir [COMPONENT_ADI] component'i yaz.

**Gereksinimler:**
- Selector: [SELECTOR]
- Girdi: [INPUT_TANIMI]
- Davranış: [DAVRANIS_TANIMI]
- Stil: [STİL_GEREKSİNİMİ]

**Yapma — bunların hiçbirini kullanma:**
- `standalone: true` yazma (Angular 20'de default)
- `@Input()`, `@Output()` decorator'larını kullanma → `input()`, `output()` fonksiyonlarını kullan
- `*ngIf`, `*ngFor`, `*ngSwitch` kullanma → `@if`, `@for`, `@switch` kullan
- `ngClass` veya `ngStyle` kullanma → `[class]` ve `[style]` binding kullan
- Constructor injection kullanma → `inject()` fonksiyonu kullan
- `ChangeDetectionStrategy.Default` kullanma → her zaman `OnPush` kullan
- `@HostBinding` / `@HostListener` kullanma → `host: {}` objesi kullan

**Yap:**
- `input.required<T>()` ile zorunlu input'ları tanımla
- `computed()` ile türetilmiş state hesapla
- Erişilebilirlik için ARIA attribute'larını ekle
- Inline template kullan (küçük component'ler için)
```

**Örnek Kullanım:**
```
Angular 20 ile bir CustomerBadge component'i yaz.

- Selector: app-customer-badge
- Girdi: customer: Customer (zorunlu), isActive: boolean (varsayılan true)
- Davranış: Müşteri adını göster; isActive false ise üstü çizili görünsün
- Stil: Bootstrap badge, aktifse bg-primary, değilse bg-secondary

Yapma: standalone: true, @Input(), *ngIf, ngClass, constructor injection, Default CD, @HostBinding
Yap: input(), computed(), [class] binding, inject(), OnPush, ARIA attribute'ları
```
