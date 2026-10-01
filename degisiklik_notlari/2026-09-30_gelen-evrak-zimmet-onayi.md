# Gelen Evrak: Zimmet Devir / Teslim Onayı ve Bildirimler

**Tarih:** 2026-09-30
**Branch:** feature/incoming-document-stats (commit edilmedi)
**Migration:** `20260930193547_AddDocumentAllocationRequestsAndNotifications` (henüz veritabanına uygulanmadı)

## Amaç

Gelen evrakta kurum içi bir kullanıcıya Devir veya Teslim yapıldığında zimmet hemen o kişiye geçmiyor. Alıcı onay verene kadar evrak devreden kişinin üzerinde kalıyor. Alıcı onay verirse zimmeti kabul etmiş oluyor:

| Yapılan işlem | Onayda oluşan zimmet statüsü |
|---|---|
| Teslim Et (`status: 3`) | Teslim Alındı (`5`) |
| Devir Et (`status: 2`) | Devir Alındı (`7`, yeni) |

Onay bekleyen kişiye uygulama içinde bildirim gidiyor ve onay verene kadar belirli aralıklarla hatırlatılıyor. E-posta gönderilmiyor.

## Hangi işlemler onaya tabi?

| İşlem | Onay |
|---|---|
| Kurum içi başka kullanıcıya Devir / Teslim | **Gerekir** |
| Kişinin evrakı kendine zimmetlemesi | Gerekmez |
| Dış kurum kullanıcısına zimmet (`userType: 2`) | Gerekmez, eskisi gibi doğrudan |
| Ön Kayıt, Arşiv, Kargoya Verildi | Gerekmez |
| PDF yükleme ve taramadaki otomatik Ön Kayıt zimmeti | Gerekmez |
| Giden evrak zimmeti | Bu değişiklik kapsamında değil |

Onay mekanizması `ZimmetApprovalRequired` ayarıyla kapatılabilir. Kapatılırsa eski davranışa, yani doğrudan zimmete dönülür.

## Talep durumları (`AllocationRequestStatusEnum`)

| Değer | Etiket | Anlam |
|---|---|---|
| 1 | Onay Bekliyor | Zimmet devredende; alıcının onayı bekleniyor |
| 2 | Onaylandı | Zimmet alıcıya geçti |
| 3 | Reddedildi | Alıcı kabul etmedi; zimmet devredende kaldı |
| 4 | İptal Edildi | Devreden veya işlemi yapan kişi talebi geri çekti |
| 5 | Geçersiz | Talep beklerken evrağın zimmeti başka bir işlemle değişti; talep kendiliğinden kapandı |

Bir evrakta aynı anda yalnızca **bir** bekleyen talep olabilir (veritabanında filtreli unique index var).

## Önyüzü etkileyen değişiklikler

### `POST api/DocumentAllocations/Create` (davranış değişti)

İstek gövdesi aynı. Onaya tabi bir işlemde zimmet oluşmuyor; onun yerine talep açılıyor ve şu yanıt dönüyor:

```json
{
  "message": "Zimmet talebi alıcının onayına gönderildi. Onay verilene kadar evrak mevcut zimmet sahibinde kalır.",
  "data": { "requestId": "…", "isPendingApproval": true }
}
```

Önyüzde yapılması gerekenler:
- Yanıtta `data.isPendingApproval == true` ise "Onaya gönderildi" gösterilmeli. Evrak devredenin listesinde kalmaya devam eder.
- Evrakın bekleyen talebi varken **her türlü** yeni zimmet engellenir. Yanıt mesajında alıcının adı yazar ve `data` alanında bekleyen talep bulunur. Yeni zimmet için önce talep iptal edilmelidir.
- Aynı anda iki kişi aynı evrak için talep açmaya çalışırsa ikincisine "az önce başka bir zimmet talebi oluşturuldu" mesajı döner.
- `status: 5` (Teslim Alındı) ve `status: 7` (Devir Alındı) **başka bir iç kullanıcı adına** gönderilemez; hata mesajı döner. Bu statüler yalnızca alıcının onayıyla ya da Teslim Al ile oluşur. Kişinin kendi adına (`userId == createdUserId`) ve dış kurum kullanıcısına gönderimi eskisi gibi serbesttir.
- İstekte geçerli bir token varsa `createdUserId` token'daki kullanıcıdan alınır ve gövdedeki değer yok sayılır. Böylece `createdUserId` alanına alıcının Id'si yazılarak onay atlanamaz. Token gönderilmezse gövdedeki değer kullanılmaya devam eder (bkz. Açık konular).

### Yeni: `api/DocumentAllocationRequests`

Diğer endpoint'lerde olduğu gibi hata durumlarında da HTTP 200 döner. Sonuç için **`data.result` koduna bakılmalı**; `message` yalnızca kullanıcıya gösterilecek metindir ve değişebilir.

| Endpoint | Gövde / parametre | Kim çağırır |
|---|---|---|
| `POST Approve` | `{ "requestId", "userId" }` | Alıcı |
| `POST ApproveBulk` | `{ "requestIds": [...], "userId" }` | Alıcı |
| `POST Reject` | `{ "requestId", "userId", "note" }` (`note` isteğe bağlı, en fazla 1000 karakter) | Alıcı |
| `POST RejectBulk` | `{ "requestIds": [...], "userId", "note" }` (ortak gerekçe) | Alıcı |
| `POST Cancel` | `{ "requestId", "userId", "note" }` | Devreden veya işlemi yapan kullanıcı |
| `POST CancelBulk` | `{ "requestIds": [...], "userId", "note" }` (ortak gerekçe) | Devreden veya işlemi yapan kullanıcı |

Token gönderilirse `userId` token'dan alınır.

**Tekli işlem yanıtı:**

```json
{ "message": "Zimmet kabul edildi; evrak teslim alındı",
  "data": { "requestId": "…", "result": 1, "message": "Zimmet kabul edildi; evrak teslim alındı" } }
```

**Toplu işlem yanıtı:** `data` içinde her talep için aynı `{ requestId, result, message }` nesnesi bulunur. `message` alanı özettir ("18 zimmet talebi onaylandı, 2 talep işlenemedi"). Bir istekte en fazla 500 talep gönderilebilir. Her talep ayrı commit'le işlenir; biri başarısız olursa diğerleri etkilenmez.

| `result` | Anlam |
|---|---|
| 1 | Başarılı |
| 2 | Geçersiz: talep beklerken evrağın zimmeti değişti, talep kapandı |
| 3 | Çakışma: aynı anda başka bir işlemle sonuçlandı; liste yenilenmeli |
| 4 | Talep bulunamadı |
| 5 | Yetkisiz: onay/red için alıcı, iptal için devreden veya işlemi yapan kullanıcı değil |
| 6 | Talep zaten sonuçlanmış |
| 7 | `userId` geçersiz |
| `GET GetPendingByUserId?userId=` | | "Onayımı bekleyenler" ekranı (eskiden yeniye) |
| `GET GetSentByUserId?userId=&onlyPending=true` | | "Gönderdiğim talepler" ekranı (yeniden eskiye) |
| `GET GetByDocumentId?incomingDocumentId=` | | Evrak detayında talep geçmişi; bekleyen talep `status == 1` olan kayıttır |

Talep DTO'sunda evrak bilgileri (`orginalNo`, `qrCode`, `subject`, `documentDate`), `fromUserFullName`, `toUserFullName`, `requestedByFullName`, `requestedAllocationStatus` (2/3), `status`, `responseNote`, `reminderCount` alanları bulunur.


### Yeni: `api/Notifications`

| Endpoint | Açıklama |
|---|---|
| `GET GetByUserId?userId=&onlyUnread=false&take=20` | Bildirimler, en yeni başta; `take` boşsa tümü |
| `GET GetUnreadCount?userId=` | `{ userId, unreadCount }`: zil ikonundaki sayı için |
| `POST MarkAsRead` | `{ "id", "userId" }` |
| `POST MarkAllAsRead` | `{ "userId" }` |

Bildirimde `type`, `title`, `message`, `relatedEntityId` (talep Id'si) ve `incomingDocumentId` alanları bulunur. Birden fazla evrağı kapsayan toplu hatırlatmalarda son ikisi boş gelir.

| `type` | Kime | Ne zaman |
|---|---|---|
| 1 Zimmet Onay Talebi | Alıcı | Talep açılınca |
| 2 Zimmet Onay Hatırlatma | Alıcı | Hatırlatma zamanı gelince |
| 3 Zimmet Onaylandı | Devreden ve işlemi yapan | Onayda |
| 4 Zimmet Reddedildi | Devreden ve işlemi yapan | Redde; gerekçe varsa mesajda yazar |
| 5 Zimmet Talebi İptal | Alıcı | İptalde |
| 6 Zimmet Onayı Gecikiyor | Devreden ve işlemi yapan | Hatırlatma sayısı eşiğe ulaşınca (bir kez) |
| 7 Zimmet Talebi Geçersiz | Tüm taraflar | Talep beklerken zimmet değişince |

**Bildirimlerin toplanması:**
- **Alıcıda tek "onay bekliyor" bildirimi (tür 1 ve 2):** Alıcının okunmamış halde en fazla bir tane onay talebi veya hatırlatma bildirimi olur. Yeni talep ya da hatırlatma geldiğinde eskisi kaldırılır ve yerine güncel sayıyı içeren tek bildirim açılır: "200 evrak zimmet onayınızı bekliyor. Son gelen: …". Tek talep varken bildirim o evrağa bağlıdır (`relatedEntityId` dolu); birden fazla talep varken boştur.
- **Talep sonuçlanınca:** Bu bildirimdeki sayı kalan talep sayısına göre güncellenir. Bekleyen talep kalmayınca bildirim okundu sayılır.
- **Toplu işlemlerde (tür 3, 4, 5):** Karşı tarafa talep başına değil, kişi başına tek bildirim gider: "Ayşe Yılmaz, 20 evrakın zimmetini kabul etti." Tekli işlemde mesaj evrak bilgisini içerir.

Sunucu şu an bildirimleri anlık olarak iletmiyor (push yok). Önyüz `GetUnreadCount` endpoint'ini girişte ve belirli aralıklarla (ör. 1 dakikada bir) çağırmalıdır.

### Zimmet statüsü ve işlem geçmişi

- `AllocationStatusEnum`: `DevirAlindi = 7` ("Devir Alındı") eklendi. Zimmet listelerindeki statü etiketlerine eklenmesi gerekir.
- `TransactionTypeEnum`: `ZimmetTalebi = 13`, `DevirAlindi = 14`, `ZimmetTalebiReddedildi = 15`, `ZimmetTalebiIptal = 16` eklendi. Teslim onayında mevcut `TeslimAlindi = 12` yazılır.
- Teslim onayında evrakın `status` alanı eskisi gibi `3` (Teslim Edildi) olur. Ancak bu artık Teslim Et'e basıldığında değil, **alıcı onay verdiğinde** gerçekleşir.

## Hatırlatma servisi

`ZimmetApprovalReminderBackgroundService` 5 dakikada bir çalışır. Ayarlar AppSettings'ten her turda okunur, bu yüzden değişiklik için yeniden başlatma gerekmez.

| Anahtar | Varsayılan | Anlam |
|---|---|---|
| `ZimmetApprovalRequired` | `true` | Onay mekanizması açık mı |
| `ZimmetReminderEnabled` | `true` | Hatırlatma açık mı |
| `ZimmetReminderIntervalHours` | `4` | Kaç saatte bir hatırlatma yapılacağı (en az 1) |
| `ZimmetReminderEscalateAfter` | `3` | Kaçıncı hatırlatmada devredene gecikme bildirimi gideceği (`0` = gönderilmez) |
| `ZimmetReminderWorkStartHour` | `8` | Hatırlatma başlangıç saati |
| `ZimmetReminderWorkEndHour` | `18` | Hatırlatma bitiş saati |

- Hatırlatmalar yalnızca hafta içi ve mesai saatlerinde gönderilir (sunucu saati). Mesai dışında zamanı gelen hatırlatma, ertesi mesai başında gider.
- Onay hiçbir zaman kendiliğinden verilmez. Talep, alıcı onaylayana, reddedene veya devreden iptal edene kadar bekler.

## Tutarlılık

- Onayda şu işlemler tek commit'te yapılır: eski zimmetin pasife çekilmesi, yeni zimmet, transaction, evrak durumu, talebin kapanması ve bildirimler.
- Onay anında evrağın aktif zimmeti talep açıldığındakiyle aynı değilse onay yapılmaz ve talep Geçersiz olur. Örneğin bu arada PDF yükleme ile Ön Kayıt zimmeti açılmışsa bu durum oluşur. Hatırlatma servisi de bu tür talepleri kapatır.
- Talepte `RowVersion` var. Onay, red ve iptal aynı anda gelirse yalnızca biri kaydedilir; diğerine "başka bir işlemle güncellendi" mesajı döner.

## Açık konular

- **Migration uygulanmadı.** `dotnet ef database update -p DijitalEvrakTakip.Persistance -s DijitalEvrakTakip.WebApi` ile uygulanmalıdır.
- **Kimlik doğrulama:** Zimmet ve bildirim işlemlerinde token gönderilirse kullanıcı token'dan (`NameIdentifier`) alınıyor. Ancak controller'larda `[Authorize]` olmadığı için token göndermeyen bir istek gövdedeki `userId` / `createdUserId` değeriyle hâlâ başkası adına işlem yapabilir. Önyüz bu endpoint'lere her zaman token göndermeli. Açığın tamamen kapanması için endpoint'lere `[Authorize]` eklenmelidir.
- **`PUT DocumentAllocations/Update`** zimmetin `userId` alanını doğrudan değiştirebiliyor ve onay mekanizmasını atlıyor. Bu alanın değiştirilmesi kapatılmalı ya da yetkiyle sınırlanmalı.
- **Mevcut zimmetler:** Değişiklikten önce Teslim (3) statüsüyle açılmış zimmetler için `POST DocumentAllocations/Receive` (Teslim Al) eskisi gibi çalışır.
- **Giden evrak:** Aynı yapı sonraki aşamada ayrıca değerlendirilecek.

## Değişen dosyalar

**Yeni**
- `Domain/Entities/DocumentAllocationRequest.cs`, `Domain/Entities/Notification.cs`
- `Domain/Enums/AllocationRequestStatusEnum.cs`, `NotificationTypeEnum.cs`, `AllocationRequestActionResultEnum.cs`
- `Domain/Dtos/DocumentAllocationRequestDto.cs`, `NotificationDto.cs`
- `Domain/Repositories/IDocumentAllocationRequestRepository.cs`, `INotificationRepository.cs`
- `Persistance/Configurations/DocumentAllocationRequestConfiguration.cs`, `NotificationConfiguration.cs`
- `Persistance/Repositories/DocumentAllocationRequestRepository.cs`, `NotificationRepository.cs`
- `Persistance/Services/DocumentAllocationRequestService.cs`, `NotificationService.cs`
- `Persistance/Migrations/20260930193547_AddDocumentAllocationRequestsAndNotifications.cs` (+ Designer, snapshot); AppSettings seed satırları da bu migration'da
- `Application/Services/IDocumentAllocationRequestService.cs`, `INotificationService.cs`
- `Application/Features/DocumentAllocationRequestFeatures/`: Approve, ApproveBulk, Reject, RejectBulk, Cancel, CancelBulk; GetPendingByUserId, GetSentByUserId, GetByDocumentId; ortak doğrulama ve mesajlar için `AllocationRequestActionRunner`
- `Domain/Dtos/AllocationRequestActionResultDto.cs`: `{ requestId, result, message }`
- `Presentation/Abstractions/ApiController.cs`: `ResolveUserId` (token varsa kullanıcıyı token'dan alır)
- `Application/Features/NotificationFeatures/`: GetByUserId, GetUnreadCount, MarkAsRead, MarkAllAsRead
- `Presentation/Controllers/DocumentAllocationRequestsController.cs`, `NotificationsController.cs`
- `WebApi/BackgroundServices/ZimmetApprovalReminderBackgroundService.cs`

**Değişen**
- `Domain/Enums/AllocationStatus.cs`: `DevirAlindi = 7`
- `Domain/Enums/TransactionType.cs`: 13–16
- `Domain/Constants/AppSettingKeys.cs`: zimmet onayı ve hatırlatma anahtarları
- `Application/Services/IDocumentAllocationService.cs`, `Persistance/Services/DocumentAllocationService.cs`: `StageCreateAsync` (commit etmeden zimmet hazırlar); `CreateAsync` bunu kullanıyor, davranışı değişmedi
- `Application/Features/DocumentAllocationFeatures/Commands/CreateDocumentAllocation/CreateDocumentAllocationCommandHandler.cs`: onaya tabi işlemlerde talep açıyor; bekleyen talep varken zimmeti engelliyor
- `WebApi/Program.cs`: servis, repository ve background service kayıtları

## Test

Solution derleniyor ve mevcut birim testi geçiyor. Migration veritabanına uygulanmadığı için uçtan uca deneme yapılmadı.
