# Gelen Evrak: Akış Durumu ile Yayın Durumunun Ayrılması

**Tarih:** 2026-09-28
**Branch:** feature/incoming-document-stats (commit edilmedi)
**Migration:** Yok. Yeni kolon eklenmedi, mevcut `SubmissionStatus` kolonu kullanılıyor.

## Amaç

`IncomingDocument.Status` hem evrakın akıştaki yerini hem de "Yayınla" bilgisini tutuyordu. Yayınla'ya basılınca evrakın önceki durumu kayboluyordu ve "Teslim Edildi" durumu hiç atanmıyordu. Bu iki bilgi artık ayrı alanlarda tutuluyor:

| Alan | Enum | Ne tutar |
|---|---|---|
| `status` | `DocumentStatusEnum` | Evrakın akıştaki yeri |
| `submissionStatus` | `PublishStatusEnum` (yeni) | Atlas'a yayın (aktarım) süreci |
| `release` / `releaseDate` | — | Yalnızca Atlas aktarımı başarılı olunca `true` ve tarih |

## Değerler

### `status` (DocumentStatusEnum)

| Değer | Anlam |
|---|---|
| 1 | Ön Kayıt |
| 2 | Evrak Kayıtta (Güncelleme) |
| 3 | Teslim Edildi (birimde) |
| 4 | Eşleştirme |
| 5 | Ocr |
| 6 | Yayınla: **Artık Status'a yazılmıyor.** Aşağıya bakın. |

### `submissionStatus` (PublishStatusEnum)

| Değer | Etiket | Anlam |
|---|---|---|
| 1 | Yayınlanmadı | Varsayılan. Ön kayıtta atanır. |
| 2 | Aktarım Sırasında | Yayınla'ya basıldı, aktarım bekliyor |
| 3 | Aktarılıyor | Aktarım servisi gönderiyor |
| 4 | Yayınlandı | Atlas'tan başarılı yanıt geldi |
| 5 | Aktarım Hatalı | Aktarım hata aldı, yeniden denenecek |

3, 4 ve 5 numaralı değerleri Atlas aktarım servisi atayacak. Bu servis henüz yazılmadı.

## Önyüzü etkileyen değişiklikler

### Yayınla: `PUT api/IncomingDocument/Update`

İstek gövdesi değişmedi. Yayınla için eskisi gibi `status: 6` gönderilebilir. Backend'in davranışı değişti:

- `status` alanına **dokunulmuyor**. Evrak hangi durumdaysa orada kalıyor.
- `submissionStatus` alanı `2` (Aktarım Sırasında) yapılıyor.
- `Yayinla` türünde transaction eskisi gibi oluşuyor.
- `status: 6` gönderildiğinde istekteki `submissionStatus` değeri yok sayılıyor.

**Önyüzde yapılması gerekenler:**

- Evrakın yayınlanıp yayınlanmadığı artık `status == 6` ile anlaşılamaz. Yerine şu alanlar kullanılmalı:
  - Yayına gönderildi mi: `submissionStatus >= 2`
  - Yayınlandı mı: `release == true` (veya `submissionStatus == 4`)
- Listelerde ve detayda "Yayın Durumu" `submissionStatus` alanından gösterilmeli. Etiketler yukarıdaki tabloda.
- Yayınla butonu, `submissionStatus` 2 veya 3 iken (sırada ya da aktarılıyorken) pasif yapılabilir. Böylece aynı evrak tekrar sıraya alınmaz.
- `GetDocumentsByStatus?status=6` sorgusu yeni kayıtlarda boş döner. Yayınlanan evrak listesi gerekiyorsa backend'e ayrı bir sorgu eklenmeli.

### Teslim Edildi: zimmet teslim edilince veya teslim alınınca

Aktif zimmet **Teslim** (`AllocationStatusEnum` = 3) veya **Teslim Alındı** (`AllocationStatusEnum` = 5) durumunda olunca evrakın `status` alanı otomatik olarak `3` (Teslim Edildi) oluyor. Önyüzün evrak durumunu ayrıca güncellemesi gerekmiyor.

- Zimmet ekranındaki "Teslim Et" butonu `POST api/DocumentAllocations/Create` ile `status: 3` gönderir; evrak durumu hemen Teslim Edildi olur. `status: 5` ile oluşturulan zimmet için de aynısı geçerlidir.
- Pasife çekilen eski zimmetler evrak durumunu değiştirmez.
- Yeni zimmet açılırken eski aktif zimmetin pasife çekilmesi, yeni zimmet, transaction ve evrak durumu tek commit'te kaydedilir. Biri hata verirse hiçbiri yazılmaz; evrak aktif zimmetsiz kalmaz.
- Teslim edilen evrak başka birime devredilirse durumu Teslim Edildi olarak kalır.

### Yeni: `POST api/DocumentAllocations/Receive`

"Teslim Al" butonu bu endpoint'i çağırır. Evrağın aktif zimmeti kendisinde olan kullanıcı evrağı teslim alır.

```json
{ "incomingDocumentId": "…", "userId": "…" }
```

- Aktif zimmet `Teslim Alındı (5)` olur ve evrak `status` alanı `3` (Teslim Edildi) olur.
- `TeslimAlindi (12)` türünde yeni bir transaction yazılır.
- Hata durumlarında da HTTP 200 döner, sonuç `message` alanında okunur:

| `message` | Durum |
|---|---|
| Evrak teslim alındı | Başarılı |
| Evrağın aktif zimmeti bulunamadı | Evrakın aktif zimmeti yok |
| Evrak bu kullanıcıya zimmetli değil | Aktif zimmet başka kullanıcıda |
| Evrak zaten teslim alınmış | Zimmet zaten Teslim Alındı durumunda |
| Geçersiz UserId | `userId` Guid değil |

## Açık konular

- **Eski kayıtlar:** Daha önce `status = 6` ile kaydedilmiş evraklar için veri düzeltmesi hazırlandı ama henüz çalıştırılmadı:
  ```sql
  UPDATE IncomingDocuments SET SubmissionStatus = 2, Status = 2 WHERE Status = 6;
  ```
  Bu sorgu çalıştırılana kadar eski kayıtlarda `status = 6` görülebilir.
- **Atlas aktarım servisi:** Sonraki aşamada yazılacak. Servis `submissionStatus = 2` olan kayıtları alacak. Başarılı olursa `release = true`, `releaseDate` ve `submissionStatus = 4`, hata alırsa `submissionStatus = 5` yazacak.

## Değişen dosyalar

- `DijitalEvrakTakip.Domain/Enums/PublishStatusEnum.cs` (yeni)
- `DijitalEvrakTakip.Domain/Enums/DocumentStatusEnum.cs`: Açıklamalar; `Teslim` etiketi "Teslim Edildi" oldu
- `DijitalEvrakTakip.Domain/Entities/IncomingDocument.cs`: Alan açıklamaları
- `DijitalEvrakTakip.Persistance/Services/IncomingDocumentService.cs`: Yayınla işlemi Status yerine SubmissionStatus'u güncelliyor
- `DijitalEvrakTakip.Persistance/Services/IncomingDocumentApplicationService.cs`: Ön kayıtta `PublishStatusEnum.Yayinlanmadi`
- `DijitalEvrakTakip.Persistance/Services/DocumentAllocationService.cs`: Zimmet Teslim (3) veya Teslim Alındı (5) olunca evrak Teslim Edildi oluyor; `ReceiveAsync`
- `DijitalEvrakTakip.Application/Features/DocumentAllocationFeatures/Commands/ReceiveDocumentAllocation/` (yeni)
- `DijitalEvrakTakip.Application/Services/IDocumentAllocationService.cs`: `ReceiveAsync`; `CreateAsync` eski aktif zimmeti de alıyor
- `DijitalEvrakTakip.Application/Features/DocumentAllocationFeatures/Commands/CreateDocumentAllocation/CreateDocumentAllocationCommandHandler.cs`: Eski zimmeti ayrı commit'le pasife çekmiyor, `CreateAsync`'e veriyor
- Giden evrak zimmetinde de aynı düzeltme yapıldı: `IOutgoingDocumentAllocationService.cs`, `OutgoingDocumentAllocationService.cs`, `CreateOutgoingDocumentAllocationCommandHandler.cs`
- `DijitalEvrakTakip.Domain/Enums/TransactionType.cs`: `TeslimAlindi = 12`
- `DijitalEvrakTakip.Presentation/Controllers/DocumentAllocationsController.cs`: `Receive` endpoint'i
- `DijitalEvrakTakip.Application/Features/IncomingDocumentFeatures/Queries/GetDocumentsByStatus/GetDocumentsByStatusQuery.cs`: Açıklama

## Test

Presentation projesi (ve bağımlı projeler) derleniyor. Uçtan uca deneme yapılmadı.
