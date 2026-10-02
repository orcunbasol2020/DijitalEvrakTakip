# Atlas Evrak Numarası (QR Kod) Havuzu

**Tarih:** 2026-10-02
**Migration:** `20261002073232_AddAtlasDocumentNumberPool` (henüz uygulanmadı)

## Amaç

Evrak numaraları (QR kodlar) Atlas EBYS web servisinden alınacak. Numara biçimi `yıl/8 hane`, örneğin `2026/42679909`.

Kayıt sırasında Atlas'a gidilmiyor. Numaralar arka planda Atlas'tan paket halinde çekilip `AtlasDocumentNumbers` tablosunda bir havuzda tutuluyor. Böylece Atlas yavaşladığında ya da kapandığında evrak kabulü durmuyor.

Atlas web servisinin sözleşmesi henüz belli değil. Şimdilik `FakeAtlasEbysClient` kullanılıyor. Bu istemci `2026/9xxxxxxx` biçiminde numara üretiyor. 9 ile başlaması, gerçek numaralarla karışmasın diye.

## Yapı

| Katman | Dosya | Görev |
|---|---|---|
| Domain | `Entities/AtlasDocumentNumber.cs` | Havuzdaki numara. Kullanılmayan `QrCodeList` entity'si bunun yerine geçti |
| Domain | `Enums/AtlasDocumentNumberStatus.cs` | Boşta (1), Ayrıldı (2), Kullanıldı (3), İptal (4) |
| Application | `Services/IAtlasEbysClient.cs` | Atlas'la konuşan istemcinin arayüzü. Veritabanına dokunmaz |
| Application | `Services/IAtlasDocumentNumberPoolService.cs` | Havuzu doldurma, numara ayırma, iptal, kullanıldı işaretleme |
| Infrastructure | `Atlas/FakeAtlasEbysClient.cs` | Sahte istemci. Gerçek istemci de buraya yazılacak |
| Persistance | `Services/AtlasDocumentNumberPoolService.cs` | Havuz servisinin uygulaması |
| WebApi | `BackgroundServices/AtlasDocumentNumberPoolBackgroundService.cs` | Stok azalınca havuzu dolduran arka plan servisi |

Gerçek istemci yazıldığında yalnızca `Program.cs` içindeki `IAtlasEbysClient` kaydı değişecek.

## Ayarlar (AppSettings tablosu)

| Anahtar | Varsayılan | Açıklama |
|---|---|---|
| `AtlasNumberPoolEnabled` | `false` | Havuz arka planda otomatik doldurulsun mu |
| `AtlasNumberPoolIntervalSeconds` | `300` | Stok kontrol aralığı, en az 30 saniye |
| `AtlasNumberPoolMinStock` | `100` | Boştaki numara sayısı bunun altına düşünce Atlas'tan yeni paket istenir |
| `AtlasNumberPoolBatchSize` | `100` | Tek seferde istenen numara adedi, en fazla 1000 |
| `AtlasNumberPoolEnforced` | `false` | Gelen evrak yalnızca havuzdaki numarayla açılabilsin mi |

Ayarlar her turda yeniden okunur. Değiştirildiklerinde uygulamayı yeniden başlatmak gerekmez.

Otomatik doldurma varsayılan olarak kapalı. Kapalıyken de `Reserve` ve `Refill` uç noktaları Atlas'tan numara çeker.

## Çalışma şekli

1. Arka plan servisi her turda boştaki numaraları sayar. Sayı minimum stoğun altındaysa Atlas'tan bir paket ister.
2. Gelen numaralar boşlukları kırpılarak, havuzda zaten var olanlar ayıklanarak "Boşta" durumuyla eklenir. `QrCode` kolonunda unique index var.
3. Etiket basmak için `Reserve` çağrılır. En eski boştaki numaralar tek bir SQL komutuyla (`UPDLOCK, READPAST`) "Ayrıldı" yapılır. Aynı anda gelen iki istek aynı numarayı alamaz. Havuz yetmezse bir kez Atlas'tan doldurma denenir.
4. Numarayla gelen evrak açıldığında numara aynı `SaveChanges` içinde "Kullanıldı" yapılır ve `IncomingDocumentId` ile evrağa bağlanır. Bu üç akışta geçerlidir:
   - Ön kayıt (`PreRegister`)
   - Numarayla PDF yükleme (`UploadWithDocumentNumber`), evrak yeni oluşuyorsa
   - Taranmış belge eşleştirme, evrak yeni oluşuyorsa
5. Kullanılmış veya iptal edilmiş bir numarayla yeni evrak açılamaz.
6. `AtlasNumberPoolEnforced = false` iken havuzda olmayan numaralar, yani elle girilenler ve eski etiketler, eskisi gibi kabul edilir. Geçiş tamamlanınca `true` yapılmalı.

## Endpoint'ler

### `POST api/AtlasDocumentNumbers/Reserve`

```json
{ "userId": "…", "count": 10 }
```

`count` 1 ile 100 arasında olmalı. Token varsa kullanıcı token'dan alınır.

```json
{ "message": "10 numara ayrıldı.", "data": ["2026/90000001", "2026/90000002"] }
```

### `GET api/AtlasDocumentNumbers/GetStock`

```json
{ "available": 90, "reserved": 10, "used": 0, "cancelled": 0, "minStock": 100 }
```

### `POST api/AtlasDocumentNumbers/Refill`

Stok minimumun altındaysa havuzu hemen doldurur. Otomatik doldurma kapalı olsa da çalışır.

### `POST api/AtlasDocumentNumbers/Cancel`

```json
{ "qrCode": "2026/90000001", "userId": "…", "reason": "Etiket hatalı basıldı" }
```

Kullanılmış numara iptal edilemez.

## Frontend için not

Numaralarda `/` var. `GetByQrCode?qrCode=` gibi query parametrelerinde değer `encodeURIComponent` ile gönderilmeli. Dosya adlarında `/` zaten temizleniyor.

## Atlas'tan beklenenler

Gerçek istemci için Atlas ekibinden şunlar gerekiyor:
- Servisin türü: SOAP/WSDL ya da REST
- Kimlik doğrulama yöntemi
- Toplu numara alınabiliyor mu
- Numaranın kullanıldığı veya iptal edildiği Atlas'a bildirilecek mi
