# Atlas'a EYP aktarımı: nasıl çalışır, nasıl izlenir

**Tarih:** 2026-10-14
**Etkilenen:** Gelen evrak yayınlama; yeni Atlas Aktarım (izleme) sayfası; gelen evrak detayı
**Sorumlu taraf:** Backend hazır, sıra frontend'de

---

## 👉 Frontend için (2026-10-14)

Backend'de EYP paketi artık gerçekten üretiliyor (e-Yazışma 2.0) ve aktarım servisini izlemek için gereken
uç noktalar hazır. Frontend'den beklenenler:

1. **Bilgi metni:** 1. bölümdeki "Sistem nasıl çalışır" metni olduğu gibi bir yardım/bilgi ekranında gösterilebilir.
2. **Atlas Aktarım sayfası:** servisin durumu, kuyruk, hatalı evraklar ve son denemeler. Uç noktalar 2. bölümde,
   önerilen yerleşim 3. bölümde.
3. **Evrak detayında Aktarım sekmesi:** evrakın paketleri, denemeleri ve paket indirme.

**Takip sayfasının tasarımı, TypeScript tipleri ve servis örneği:** [atlas_aktarim_takip_sayfasi.md](atlas_aktarim_takip_sayfasi.md).

**Yeni uç noktalar** (hepsi `api/AtlasTransfer/...`): `GetStatus`, `GetQueue`, `GetAttempts`,
`GetDocument/{documentId}`, `DownloadPackage/{documentId}`.
**Var olanlar:** `GetStats`, `GetFailed`, `Retry`, `Run`.

---

## 1. Sistem nasıl çalışır

*Bu bölüm kullanıcıya gösterilmek üzere yazıldı.*

### Yayınlama

Gelen evrak kaydedilirken veya güncellenirken **Yayınla** seçilirse evrak Atlas'a aktarılmak üzere sıraya
alınır. Yayınlamadan önce evrakın bilgileri kontrol edilir. Şunlardan biri eksikse evrak yayınlanmaz ve
eksikler ekranda listelenir:

- Evrak numarası (QR kod)
- Konu
- Evrak tarihi
- Gizlilik derecesi (Kişiye Özel'in EYP karşılığı henüz belirlenmediği için bu derecedeki evrak yayınlanamaz)
- Gönderen kurum

### Aktarım

Sistem, sıradaki evrakları belirli aralıklarla (varsayılan dakikada bir, her seferinde en fazla 10 evrak) alır.
Her evrak için:

1. **EYP paketi hazırlanır.** EYP, kamu kurumları arasında resmî yazışma için kullanılan standart elektronik
   pakettir (e-Yazışma 2.0). Pakete şunlar konur:
   - **Üst yazı:** evrakın taranmış/yüklenmiş dosyası. Dosyası olmayan evrakta (genelde gizlilik dereceli
     evraklar) yerine "Bu Belge Fiziki Olarak Gönderilecektir." yazan standart sayfa konur.
   - **Ekler:** yüklenen elektronik ekler (PDF) dosyalarıyla; fiziki ek varsa açıklamasıyla.
   - **Evrak bilgileri:** konu, evrak tarihi, gönderen kurumun evrak sayısı (girilmemişse Atlas evrak numarası),
     gizlilik derecesi, ivedilik.
   - **Gönderen:** gönderen kurum, DETSİS kodu ve iletişim bilgileri. DETSİS kodu olmayan gönderenlere
     (yabancı misyonlar, dilekçeler) `99999999` yazılır; bilinmeyen iletişim bilgisi `-` olarak geçer.
   - **Dağıtım:** evrakın **Nereye** listesindeki birimler, her biri kendi Bilgi/Gereği bilgisiyle.
     Nereye boşsa evrak Bakanlığa yazılır.
2. **Paket Atlas'a gönderilir.** Paket ayrıca arşiv için sistemde saklanır.

### Durumlar

| Durum | Anlamı |
|---|---|
| Yayınlanmadı | Evrak henüz yayınlanmadı. |
| Aktarım Sırasında | Evrak sırada, ilk uygun turda aktarılacak. Geçici bir hatadan sonra da bu duruma döner. |
| Aktarılıyor | Evrakın paketi şu anda hazırlanıyor veya gönderiliyor. |
| Yayınlandı | Atlas paketi kabul etti. |
| Aktarım Hatalı | Evrak aktarılamadı; hata mesajı izleme sayfasında görünür. Düzeltilip yeniden sıraya alınmalı. |

### Hatalar ve tekrar deneme

- **Geçici hata** (ör. Atlas'a ulaşılamadı): evrak sıraya geri döner ve giderek uzayan aralıklarla
  (1, 5, 15, 60 dakika) yeniden denenir. Varsayılan 5 denemeden sonra **Aktarım Hatalı** olur.
- **Kalıcı hata** (paket hazırlanamadı, örneğin evrak dosyası bulunamadı; ya da Atlas paketi reddetti):
  evrak hemen **Aktarım Hatalı** olur, tekrar denenmez.
- **Takılı kalan evrak:** bir evrak 30 dakikadan uzun **Aktarılıyor** durumunda kalırsa (ör. sunucu işlem
  sırasında yeniden başladı) kendiliğinden sıraya geri alınır.

**Aktarım Hatalı evrak için:** hatanın nedeni giderilir (ör. evrak bilgisi düzeltilir), ardından
**Yeniden Dene** ile evrak, güncel bilgileriyle hazırlanacak yeni bir paketle tekrar sıraya alınır.

### Ayarlar

Ayarlar Ayarlar sayfasından değiştirilir. Değişiklik bir sonraki turda geçerli olur, yeniden başlatma gerekmez.

| Ayar | Varsayılan | Açıklama |
|---|---|---|
| `AtlasTransferEnabled` | kapalı | Otomatik aktarım. Kapalıyken sıradaki evraklar yalnızca "Şimdi Çalıştır" ile aktarılır. |
| `AtlasTransferIntervalSeconds` | 60 | İki tur arası süre (en az 10 saniye). |
| `AtlasTransferBatchSize` | 10 | Bir turda en fazla kaç evrak aktarılır (en fazla 100). |
| `AtlasTransferMaxTryCount` | 5 | Geçici hatada kaç kez denendikten sonra Aktarım Hatalı olur. |
| `AtlasEypRecipientKkk` / `AtlasEypRecipientName` | boş | Evrakı alan kurum (Bakanlık): DETSİS kodu ve adı. **Boşken aktarım hiç çalışmaz.** |

### Şu anki sınırlamalar

- **Atlas'a gönderim henüz sahte.** Atlas'ın paket teslim yöntemi netleşene kadar paketler hazırlanıp
  arşivlenir ama Atlas'a gönderilmez, gönderilmiş sayılır. İzleme sayfasındaki "Atlas'a gönderim"
  kontrolü bu durumu gösterir.
- **Paket imzası temsilidir.** Evrak kayıt birimi dışarıdan gelen evrakı paketlediği için paket
  "Sistem Yöneticisi" adına, temsili bir imzayla hazırlanır; Atlas imzayı doğrulamaz.

---

## 2. İzleme uç noktaları

Başarılı cevaplar camelCase JSON. Hata cevapları diğer uç noktalarla aynı biçimde:
`500 {"Message":"...","StatusCode":500}`.

| Uç nokta | Ne döner | Ne zaman kullanılır |
|---|---|---|
| `GET api/AtlasTransfer/GetStatus` | Ayarlar, çalışan / son / sonraki tur, ön koşul kontrolleri, sayılar | Sayfanın üst kısmı; açıkken 10–15 sn'de bir |
| `GET api/AtlasTransfer/GetQueue` | Sıradaki ve aktarılan evraklar (en fazla 500), sıraya alınma sırasıyla | Kuyruk tablosu |
| `GET api/AtlasTransfer/GetFailed` *(vardı)* | Aktarım Hatalı evraklar ve son hata (en fazla 500) | Hatalılar tablosu |
| `GET api/AtlasTransfer/GetAttempts?limit=50` | Tüm evraklarda son gönderim denemeleri, en yenisi önce (limit 1–500) | Etkinlik akışı |
| `GET api/AtlasTransfer/GetDocument/{documentId}` | Evrakın durumu, paketleri ve tüm denemeleri | Evrak detayı, Aktarım sekmesi |
| `GET api/AtlasTransfer/DownloadPackage/{documentId}?packageId=` | `.eyp` dosyası (`application/octet-stream`, dosya adıyla) | Paketi indir |
| `GET api/AtlasTransfer/GetStats` *(vardı)* | Durumlara göre evrak sayıları | `GetStatus.counts` aynı bilgiyi içerir |
| `POST api/AtlasTransfer/Retry` *(vardı)* | `{ "documentId": "...", "userId": "..." }` → mesaj | Hatalı evrakta Yeniden Dene |
| `POST api/AtlasTransfer/Run` *(vardı)* | Turun sonucu | "Şimdi Çalıştır" (otomatik aktarım kapalı olsa da çalışır) |

### `GetStatus`

Geliştirme veritabanındaki gerçek cevap (alan sırası korunmuştur):

```json
{
  "autoTransferEnabled": false,
  "intervalSeconds": 60,
  "batchSize": 10,
  "maxTryCount": 5,
  "isRunning": false,
  "runningSince": null,
  "lastRun": {
    "startedAt": "2026-10-14T09:06:49.0221248Z",
    "finishedAt": "2026-10-14T09:06:49.0223129Z",
    "manual": true,
    "result": {
      "ran": true, "picked": 0, "succeeded": 0, "requeued": 0, "failed": 0, "recoveredStuck": 0,
      "message": "Kuyrukta aktarılacak evrak yok."
    },
    "error": null
  },
  "nextRunAt": null,
  "lastAttemptAt": null,
  "lastSuccessAt": null,
  "counts": { "notPublished": 6, "queued": 2, "transferring": 0, "succeeded": 0, "failed": 0 },
  "checks": [
    { "code": "AutoTransfer", "name": "Otomatik aktarım", "ok": false,
      "message": "Kapalı (AtlasTransferEnabled). Kuyruktaki evraklar yalnızca elle çalıştırınca aktarılır." },
    { "code": "Recipient", "name": "Evrakı alan kurum", "ok": false,
      "message": "AtlasEypRecipientKkk / AtlasEypRecipientName ayarları boş; aktarım turları çalışmaz, evraklar kuyrukta bekler." },
    { "code": "PhysicalDeliveryPdf", "name": "Fiziki gönderim PDF'i", "ok": true,
      "message": "C:\\EvrakTakip\\belgeler\\Sablonlar\\fiziki.pdf" },
    { "code": "AtlasDelivery", "name": "Atlas'a gönderim", "ok": false,
      "message": "Sahte istemci: paketler üretilip arşivlenir ama Atlas'a gönderilmez, başarılı sayılır." }
  ]
}
```

- `isRunning` / `runningSince`: bir tur şu anda çalışıyor.
- `lastRun`: tamamlanan son tur (arka plan veya `manual: true` ise "Şimdi Çalıştır"). `result.ran: false`
  turun bir nedenle hiç iş yapmadığını gösterir, nedeni `result.message`'dadır (ör. "Otomatik Atlas aktarımı kapalı.").
  Tur beklenmeyen bir hatayla kesildiyse `result: null`, `error` dolu.
- `nextRunAt`: arka plan servisinin sonraki turu; otomatik aktarım kapalıyken `null`.
- `lastAttemptAt` / `lastSuccessAt`: veritabanından son gönderim denemesi ve son başarılı aktarım.
- `checks`: aktarımın çalışması için gereken koşullar. `ok: false` olanlar uyarı olarak gösterilmeli.
  `code` sabittir: `AutoTransfer`, `Recipient`, `PhysicalDeliveryPdf`, `AtlasDelivery`.
  `Recipient` veya `PhysicalDeliveryPdf` kırmızıysa aktarım çalışmaz/paket üretilemez;
  `AutoTransfer` ve `AtlasDelivery` bilgi amaçlı uyarıdır.

### `GetQueue`

```json
[
  {
    "documentId": "6df1330d-83b7-4d7c-90d1-3aacac37b312",
    "qrCode": "2026/65645345",
    "subject": "Öğrenci kabul kontenjan",
    "status": 2,
    "statusUpdatedAt": "2026-10-11T19:59:36.1869661",
    "tryCount": 0,
    "nextAttemptAt": null,
    "lastError": null
  }
]
```

`status`: 2 Aktarım Sırasında, 3 Aktarılıyor. `nextAttemptAt` doluysa evrak geçici bir hatadan sonra o zamana
kadar beklemede; `lastError` o hatayı gösterir.

### `GetAttempts` ve `GetDocument`

Deneme kaydı (iki uç noktada aynı):

```json
{
  "documentId": "…", "qrCode": "2026/55778689", "subject": "Toplantı",
  "packageId": "…", "attemptNo": 1, "attemptedAt": "2026-10-14T09:10:02.123",
  "isSuccess": false, "isPermanentFailure": false,
  "error": "Atlas'a gönderilemedi: …", "atlasReferenceId": null
}
```

`GetDocument` cevabı:

```json
{
  "documentId": "a7aa7244-1081-49e0-b527-b170aae2ab20",
  "qrCode": "2026/55778689",
  "subject": "Toplantı",
  "status": 2,
  "statusUpdatedAt": "2026-10-12T18:33:07.0850301",
  "packages": [
    {
      "id": "…", "isActive": true, "createdDate": "…",
      "fileName": "202655778689_….eyp", "fileSize": 36232, "sha256": "…", "fileExists": true,
      "usedPlaceholderContent": true,
      "tryCount": 1, "lastAttemptAt": "…", "nextAttemptAt": null, "lastError": null,
      "atlasReferenceId": "…", "transferredAt": "…"
    }
  ],
  "attempts": [ ]
}
```

- `packages` en yenisi önce. Evrak yeniden sıraya alınınca eski paket `isActive: false` olarak geçmişte kalır,
  güncel bilgilerle yeni paket hazırlanır.
- `usedPlaceholderContent: true`: evrakın dosyası olmadığı için üst yazı olarak fiziki gönderim sayfası kullanıldı.
- `fileExists: false` ise paket indirilemez (paket hazırlanamadı ya da dosyası silindi).
- Yayınlanmamış evrakta `packages` ve `attempts` boş döner; evrak yoksa `500 "Evrak bulunamadı."`.

### `DownloadPackage`

`packageId` verilmezse güncel paket iner. Paket yoksa veya dosyası bulunamazsa `500` ve şu mesajlardan biri:
"Evrakın EYP paketi yok.", "EYP paketi henüz üretilmedi veya üretilemedi.", "EYP paket dosyası bulunamadı: …".
Paket evrakın içeriğini (üst yazı ve ekler) taşır; indirme yalnızca yetkili kullanıcılara açılmalı.

---

## 3. Önerilen ekranlar

**Atlas Aktarım sayfası** (yönetici menüsü):

1. **Durum kartı** (`GetStatus`):
   - Otomatik aktarım açık/kapalı, aralık
   - Çalışıyor göstergesi (`isRunning`)
   - Son tur: zaman, sonucun `message`'ı, elle mi otomatik mi
   - Sonraki tur
   - Son başarılı aktarım
   - `checks` listesi: yeşil/kırmızı rozetler
2. **Sayılar** (`GetStatus.counts`): Yayınlanmadı / Sırada / Aktarılıyor / Yayınlandı / Hatalı.
   Hatalı sayısı tıklanınca hatalılar sekmesi açılır.
3. **Sekmeler:**
   - **Sırada:** `GetQueue`. Sütunlar: evrak no, konu, durum, sıraya alınma, deneme sayısı, sonraki deneme, son hata.
   - **Hatalı:** `GetFailed`. Satırda **Yeniden Dene** (`Retry`).
   - **Son denemeler:** `GetAttempts`. Başarılı/geçici hata/kalıcı hata rozetiyle.
4. **Şimdi Çalıştır** butonu (`Run`). Cevaptaki `message` gösterilip `GetStatus` yenilenir.
   `ran: false` ve "zaten çalışıyor" mesajı normal bir durumdur.

Sayfa açıkken `GetStatus` 10–15 saniyede bir yenilenebilir. Tablolar sekme açılınca ve "Şimdi Çalıştır" /
"Yeniden Dene" sonrasında yenilenir.

**Gelen evrak detayında Aktarım sekmesi** (`GetDocument`): durum, güncel paket bilgisi ve **Paketi İndir**
(`DownloadPackage`), denemeler listesi. Durum Hatalı ise **Yeniden Dene**.

---

## 4. Bilinmesi gerekenler

- **Zamanlar UTC.** Veritabanından gelen zamanlar sonunda `Z` olmadan döner
  (`"2026-10-11T19:59:36.1869661"`); bunlar da UTC'dir. Ekranda yerel saate çevrilmeli.
- **Son tur ve sonraki tur bilgisi sunucunun belleğindedir.** Uygulama yeniden başlayınca ilk tura kadar
  `lastRun` boş döner. Birden fazla uygulama örneği çalışıyorsa her biri yalnızca kendi turlarını bilir.
  Kalıcı bilgi için `lastAttemptAt` / `lastSuccessAt` ve deneme listesi kullanılmalı.
- **Uç noktalar henüz `[Authorize]` değil** (projedeki diğer uç noktalar gibi). Paket indirme evrak içeriğini
  verdiği için menü ve buton yalnızca yetkili rollere gösterilmeli.
- **Listeler en fazla 500 kayıt** döner (`GetQueue`, `GetFailed`, `GetAttempts`).

**Yapılacaklar:**

- [ ] Bilgi metnini (1. bölüm) uygun bir yardım/bilgi ekranında göster
- [ ] Atlas Aktarım sayfası: durum kartı, sayılar, Sırada / Hatalı / Son denemeler sekmeleri, Şimdi Çalıştır
- [ ] Gelen evrak detayında Aktarım sekmesi ve Paketi İndir
- [ ] Ayarlar sayfasında `AtlasEypRecipientKkk` ve `AtlasEypRecipientName` doldurulmalı (aktarım bunlar olmadan çalışmaz)
