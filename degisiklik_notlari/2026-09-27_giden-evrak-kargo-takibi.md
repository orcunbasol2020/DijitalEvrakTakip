# Giden Evrak Kargo Takibi

**Tarih:** 2026-09-27
**Commit:** `1e4d344` (feature/incoming-document-stats)
**Migration:** `20260927093850_AddOutgoingDocumentShipments` (uygulandı)

## Amaç

Giden evrak, teslim alan kişiye zimmetlenir. Bu kişi evrakı bir dış kurum personeline elden zimmetleyebilir ya da kargo ile gönderebilir. Kargo ile gönderimde takip numarası ve kargo durumunun tutulması gerekiyordu.

Kargo bilgisi evrağın değil gönderimin özelliği olduğu için ayrı bir tabloda tutulur. Aynı evrak birden fazla kuruma farklı paketlerle gidebilir. Aynı kuruma giden birden fazla evrak da tek pakette gidebilir.

## Veri modeli

### Yeni tablo: `OutgoingDocumentShipments`

| Alan | Açıklama |
|---|---|
| `CargoCompany` | Kargo firması (`CargoCompanyEnum`) |
| `TrackingNumber` | Takip numarası, zorunlu, indeksli |
| `SentDate` | Gönderim tarihi |
| `SentUserId` | Kargoya veren kullanıcı |
| `ExternalInstitutionId` | Alıcı kurum |
| `RecipientName` | Paket üzerindeki alıcı adı |
| `Status` | Kargo durumu (`ShipmentStatusEnum`) |
| `DeliveredDate` | Teslim tarihi |
| `Cost` | Kargo ücreti |
| `Notes` | Not |

### Değişen tablolar

- **`OutgoingDocumentDistributions`:** `CargoPostNumber` kaldırıldı. `DeliveryMethod` ve `ShipmentId` eklendi. Kargo silinirse `ShipmentId` boşaltılır.
- **`OutgoingDocumentTransactions`:** `CargoPostNumber` kaldırıldı, yerine `ShipmentId` eklendi.
- **`OutgoingDocumentDelivery` entity'si:** Hiçbir yerde kullanılmadığı için silindi.

## Enum değerleri

**CargoCompanyEnum**

| Değer | Ad |
|---|---|
| 1 | PTT Kargo |
| 2 | Aras Kargo |
| 3 | Yurtiçi Kargo |
| 4 | MNG Kargo |
| 5 | Sürat Kargo |
| 6 | Kurye |
| 99 | Diğer |

**ShipmentStatusEnum**

| Değer | Ad |
|---|---|
| 1 | Kargoya Verildi |
| 2 | Yolda |
| 3 | Teslim Edildi |
| 4 | İade |

**DeliveryMethodEnum**

| Değer | Ad |
|---|---|
| 1 | Elden |
| 2 | Kargo / Posta |
| 3 | EBYS |

**Mevcut enum'lara eklenenler**

- `AllocationStatusEnum`: 6 = Kargoya Verildi
- `EnvelopeStatusEnum`: 5 = Kargoya Verildi

## İş akışı

### Kargoya verme (`POST api/OutgoingDocumentShipments/Create`)

1. İstekteki dağıtım satırları bulunur. Bulunamayan ya da daha önce kargolanmış satır varsa istek reddedilir.
2. Her evrağın aktif zimmetinin kargoya veren kullanıcıda olduğu kontrol edilir. Değilse hiçbir kayıt oluşturulmadan hata döner.
3. `envelopeId` gönderildiyse zarf kontrol edilir. Zarf yoksa ya da zaten kargoya verilmişse istek reddedilir.
4. Kargo kaydı oluşturulur. Alıcı kurum istekte yoksa önce dağıtım satırlarından, sonra zarftan alınır.
5. Dağıtım satırları kargoya bağlanır: `ShipmentId` yazılır, `DeliveryMethod` 2 olur, `SentDate` doldurulur.
6. Evrakların aktif zimmeti "Kargoya Verildi" durumuyla kapatılır. Fiziksel evrak artık kimsede olmadığı için zimmet zinciri burada biter.
7. Her evrağa "Gönderildi" tipinde işlem kaydı yazılır.
8. Zarf verildiyse durumu "Kargoya Verildi" yapılır.

### Kargo güncelleme (`PUT api/OutgoingDocumentShipments/Update`)

- Boş gönderilen alanlar değiştirilmez.
- Durum "Teslim Edildi" olursa, boşsa teslim tarihi doldurulur. Paketteki dağıtım satırlarının teslim tarihi de yazılır.
- Durum "Teslim Edildi" veya "İade" olursa evraklara ilgili işlem kaydı yazılır.

## Endpoint'ler

| Metot | Yol |
|---|---|
| POST | `api/OutgoingDocumentShipments/Create` |
| PUT | `api/OutgoingDocumentShipments/Update` |
| GET | `api/OutgoingDocumentShipments/GetById?id=` |
| GET | `api/OutgoingDocumentShipments/GetByOutgoingDocumentId?outgoingDocumentId=` |
| GET | `api/OutgoingDocumentShipments/GetByTrackingNumber?trackingNumber=` |

## Client'ı etkileyen değişiklikler

- Dağıtım satırı ekleme ve güncelleme isteklerinden `cargoPostNumber` kaldırıldı, yerine isteğe bağlı `deliveryMethod` geldi. Bu alan eklerken bilinmeyebilir. Kargolamada backend kendisi doldurur.
- Dağıtım yanıtına `deliveryMethod`, `shipmentId`, `cargoCompany`, `trackingNumber` ve `shipmentStatus` eklendi.
- Giden evrak güncelleme isteğinden `cargoPostNumber` kaldırıldı.
- Evrak işlem geçmişi yanıtında `cargoPostNumber` yerine `shipmentId` var.
- Zarf durumu artık client tarafından 5 yapılmamalı. Kargo isteğinde `envelopeId` gönderilmesi yeterli.

## Aynı işte yapılan düzeltmeler

Repository'nin `GetAll()` sorgusu takip etmeden (AsNoTracking) çalışıyor. Bu yüzden `Update()` çağrısı, sorguda birlikte yüklenen ilişkili kayıtları da değişmiş sayıp yeniden yazıyordu. Güncelleme amaçlı okumalar artık yalnızca ilgili tabloyu yüklüyor:

- Kargo oluşturma ve güncelleme
- Zarf durum güncelleme endpoint'i
- Genel zarf güncelleme endpoint'i

## Bilinen eksikler

- `envelopeId` gönderildiğinde zarftaki evrakların dağıtım satırları otomatik bulunmuyor. Client bu satırları `distributionIds` içinde göndermeli. Eksik gönderilirse bazı evrakların zimmeti açık kalır. Önerilen çözüm: `envelopeId` geldiğinde backend, zarftaki her evrak için zarfın kurumuna ait dağıtım satırını kendisi bulsun.
- Zarf durum güncelleme endpoint'i hâlâ 5 değerini kabul ediyor. Bu yüzden kargo kaydı olmadan da zarf "Kargoya Verildi" yapılabiliyor.
