# Atlas Aktarım takip sayfası (frontend)

**Tarih:** 2026-10-14
**Etkilenen:** Yeni sayfa: **Atlas Aktarım** (öneri: `/atlas-transfer`, yönetici menüsü); gelen evrak detayına Aktarım sekmesi
**Backend dalı:** feature/atlas-transfer-queue
**Sorumlu taraf:** Backend hazır, sıra frontend'de

Sistemin nasıl çalıştığı ve kullanıcıya gösterilecek açıklama metni: [atlas_eyp_aktarimi.md](atlas_eyp_aktarimi.md) (1. bölüm).
Bu not yalnızca takip sayfasının tasarımını ve API kullanımını anlatır.

---

## 👉 Frontend için (2026-10-14)

Yayınlanan gelen evraklar EYP paketine çevrilip arka planda Atlas'a aktarılıyor. Bu sayfa aktarımı tek
yerden izlemek ve müdahale etmek için:

- Servis çalışıyor mu, en son ne zaman çalıştı, sıradaki tur ne zaman?
- Aktarımın çalışması için gereken ayarlar tamam mı?
- Kaç evrak sırada, kaçı aktarıldı, kaçı hatalı?
- Hatalı evrakları yeniden sıraya alma, turu beklemeden çalıştırma, evrakın paketini indirme.

Backend'de bunun için gereken her şey hazır; ön yüzde yeni sayfa, bir servis ve bir detay penceresi gerekiyor.
Backend'deki son değişiklik: CORS artık `Content-Disposition` başlığını açıyor, indirilen dosyanın adı
okunabilir (2. bölüm, indirme).

---

## 1. Sayfa yerleşimi

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ Atlas Aktarım                         [ⓘ Nasıl çalışır?]  [▶ Şimdi Çalıştır] │
├──────────────────────────────────────────────────────────────────────────────┤
│ ● Otomatik aktarım: AÇIK · 60 sn'de bir, en fazla 10 evrak                     │
│ ⟳ Çalışıyor (09:14:02'den beri)   |  Son tur: 09:13:02 · otomatik            │
│   "3 evrak işlendi: 2 aktarıldı, 1 tekrar denenecek, 0 hatalı."              │
│ Sonraki tur: 09:14:02  ·  Son başarılı aktarım: 09:13:04                     │
│                                                                              │
│ Kontroller:  ✔ Evrakı alan kurum   ✔ Fiziki gönderim PDF'i                   │
│              ⚠ Atlas'a gönderim (sahte istemci)                              │
├──────────────┬──────────────┬──────────────┬──────────────┬─────────────────┤
│ Yayınlanmadı │ Sırada       │ Aktarılıyor  │ Yayınlandı   │ Hatalı          │
│      6       │      2       │      0       │     148      │      3  →       │
├──────────────┴──────────────┴──────────────┴──────────────┴─────────────────┤
│ [ Sırada (2) ] [ Hatalı (3) ] [ Son Denemeler ]                              │
│ ┌──────────────────────────────────────────────────────────────────────────┐ │
│ │ Evrak No       Konu              Durum     Durum Zamanı   Deneme  …      │ │
│ │ 2026/65645345  Öğrenci kabul …   Sırada    11.10 22:59    0              │ │
│ └──────────────────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────────────────┘
```

Angular Material karşılıkları: üst kısım `mat-card`, kontroller `mat-chip`, sayılar beş küçük `mat-card`,
tablolar `mat-tab-group` + `mat-table`, çalışıyor göstergesi `mat-progress-spinner` (küçük, diameter 16),
bildirimler `MatSnackBar`, evrak detayı `MatDialog`.

### 1.1 Durum kartı (`GetStatus`)

| Gösterilecek | Alan | Not |
|---|---|---|
| Otomatik aktarım açık/kapalı | `autoTransferEnabled`, `intervalSeconds`, `batchSize` | Kapalıysa gri rozet: "KAPALI · yalnızca elle çalıştırılır" |
| Çalışıyor göstergesi | `isRunning`, `runningSince` | `isRunning` true iken spinner |
| Son tur | `lastRun.finishedAt`, `lastRun.manual`, `lastRun.result.message` / `lastRun.error` | `manual` → "elle" / "otomatik". `error` doluysa kırmızı. `result.ran === false` ise mesaj gri (tur bir nedenle iş yapmadı, ör. "Otomatik Atlas aktarımı kapalı.") |
| Sonraki tur | `nextRunAt` | null ise gösterme (otomatik aktarım kapalı ya da uygulama yeni başladı) |
| Son başarılı aktarım | `lastSuccessAt` | null ise "Henüz yok" |
| Kontroller | `checks[]` | Her biri bir rozet; açıklama `message` (tooltip) |

`lastRun` uygulama belleğindedir; sunucu yeniden başlayınca ilk tura kadar null gelir. Bu durumda
"Son tur: sunucu yeniden başladıktan sonra henüz çalışmadı" yazılabilir. `lastAttemptAt` veritabanındaki son
denemedir ve yeniden başlatmadan etkilenmez.

**Kontrol rozetleri** (`code` sabittir, ona göre renk seçilir):

| `code` | `ok: true` | `ok: false` |
|---|---|---|
| `Recipient` | yeşil | **kırmızı**: aktarım hiç çalışmaz, evraklar sırada bekler |
| `PhysicalDeliveryPdf` | yeşil | **kırmızı**: dosyası olmayan evrakların paketi hazırlanamaz, bu evraklar hatalıya düşer |
| `AutoTransfer` | yeşil | gri: bilgi |
| `AtlasDelivery` | yeşil | turuncu: paketler hazırlanıyor ama Atlas'a gitmiyor (geçici durum) |

Kırmızı bir kontrol varsa kartın üstünde uyarı şeridi gösterilmesi önerilir:
"Aktarım şu anda çalışamıyor: {name} — {message}".

### 1.2 Sayı kartları (`GetStatus.counts`)

| Kart | Alan | Tıklayınca |
|---|---|---|
| Yayınlanmadı | `notPublished` | (gelen evrak listesine süzgeçle gidilebilir) |
| Sırada | `queued` | Sırada sekmesi |
| Aktarılıyor | `transferring` | Sırada sekmesi |
| Yayınlandı | `succeeded` | Son Denemeler sekmesi |
| Hatalı | `failed` | Hatalı sekmesi; 0'dan büyükse kırmızı |

Sekme başlıklarındaki sayılar da buradan alınabilir (Sırada = `queued + transferring`, Hatalı = `failed`).

### 1.3 Sekmeler

**Sırada** (`GET api/AtlasTransfer/GetQueue`)

| Sütun | Alan |
|---|---|
| Evrak No | `qrCode` (tıklanınca evrak aktarım detayı açılır, bkz. 1.4) |
| Konu | `subject` |
| Durum | `status`: 2 → "Sırada", 3 → "Aktarılıyor" (spinner) |
| Durum Zamanı | `statusUpdatedAt`: Sırada ise sıraya (yeniden) alınma, Aktarılıyor ise aktarımın başlama zamanı |
| Deneme | `tryCount` |
| Sonraki Deneme | `nextAttemptAt` (doluysa "geçici hata sonrası bekliyor") |
| Son Hata | `lastError` (kısaltılmış, tooltip'te tamamı) |

Boşken: "Sırada bekleyen evrak yok."

**Hatalı** (`GET api/AtlasTransfer/GetFailed`)

| Sütun | Alan |
|---|---|
| Evrak No | `qrCode` |
| Konu | `subject` |
| Deneme | `tryCount` |
| Son Deneme | `lastAttemptAt` |
| Hata | `lastError` |
| İşlem | **Yeniden Dene** butonu (1.5) |

Hata mesajları backend'de Türkçe ve kullanıcıya gösterilebilir. Örnekler:
"EYP oluşturulamadı: Evrak dosyası bulunamadı: …", "Atlas paketi reddetti: …",
"Atlas'a gönderilemedi: … (5 deneme yapıldı.)".

**Son Denemeler** (`GET api/AtlasTransfer/GetAttempts?limit=50`)

| Sütun | Alan |
|---|---|
| Zaman | `attemptedAt` |
| Evrak No / Konu | `qrCode`, `subject` |
| Deneme | `attemptNo` |
| Sonuç | rozet: `isSuccess` → yeşil "Aktarıldı"; değilse `isPermanentFailure` → kırmızı "Hatalı", aksi halde turuncu "Tekrar denenecek" |
| Atlas Referansı / Hata | `atlasReferenceId` ya da `error` |

"Daha fazla" ile `limit` 100, 200 … 500'e kadar artırılabilir.

### 1.4 Evrak aktarım detayı (`GET api/AtlasTransfer/GetDocument/{documentId}`)

Tablolardaki evrak numarasına tıklanınca açılan pencere. Aynı içerik gelen evrak detayında "Aktarım"
sekmesi olarak da kullanılabilir.

- Üstte: evrak no, konu, durum (`status`, bkz. 2. bölümdeki tablo), `statusUpdatedAt`.
- **Paketler** (`packages`, en yenisi önce):
  - Oluşturulma, dosya adı, boyut (`fileSize`, KB/MB), deneme sayısı, Atlas referansı, aktarılma zamanı.
  - `usedPlaceholderContent: true` → "Üst yazı: fiziki gönderim sayfası (evrakın dosyası yok)".
  - `isActive: false` → "Eski paket (evrak yeniden sıraya alındı)", soluk gösterilir.
  - **İndir** butonu yalnızca `fileExists: true` iken etkin.
- **Denemeler** (`attempts`): Son Denemeler sekmesiyle aynı sütunlar.
- Durum Hatalı (5) ise **Yeniden Dene**.

### 1.5 İşlemler

**Şimdi Çalıştır**: `POST api/AtlasTransfer/Run` (gövde boş)

- Otomatik aktarım kapalı olsa da bir tur çalıştırır. Onay penceresi gerekmez.
- Cevap `AtlasTransferRunResult`. `message` snackbar'da gösterilir, ardından `GetStatus` ve açık sekme yenilenir.
- `ran: false` hata değildir. Örneğin "Atlas aktarımı şu anda zaten çalışıyor." ya da
  "Evrakı alan kurumun KKK kodu veya adı ayarlarda tanımlı değil; aktarım yapılmadı." gelebilir; bunlar
  bilgi olarak gösterilir.
- Tur bitene kadar cevap gelmez (en fazla 100 evrak). Buton istek süresince kapalı ve spinner'lı olmalı.

**Yeniden Dene**: `POST api/AtlasTransfer/Retry`, gövde `{ "documentId": "…", "userId": "…" }`

- `userId`: giriş yapan kullanıcı. Token gönderiliyorsa backend token'dakini kullanır.
- Evrak güncel bilgileriyle yeniden kontrol edilir ve yeni paketle sıraya alınır.
  Cevap: `{ "message": "Evrak yeniden Atlas aktarım sırasına alındı.", "data": null }`.
- Evrakta hâlâ eksik bilgi varsa `500` ve
  `{"Message":"Evrak Atlas'a aktarılamaz: Konu girilmemiş. …","StatusCode":500}` döner. Mesaj olduğu gibi
  gösterilir; kullanıcı evrakı düzenleyip tekrar denemelidir. Pencerede evrak düzenleme sayfasına bağlantı
  verilebilir.
- Başarılıysa Hatalı ve Sırada sekmeleri ile `GetStatus` yenilenir.

**Paketi İndir**: `GET api/AtlasTransfer/DownloadPackage/{documentId}?packageId={id}`

- `packageId` verilmezse güncel paket iner. Paketler listesinden indirirken o paketin `id`'si verilir.
- `responseType: 'blob'` ile alınır. Dosya adı `Content-Disposition` başlığındadır, ör.
  `202655778689_a7aa…eyp`. Okunamazsa `{qrCode}.eyp` kullanılabilir.
- Hata cevabı da blob olarak gelir; mesaj için blob metne çevrilip JSON okunmalı (aşağıdaki servis örneği).

### 1.6 Yenileme

- Sayfa açıkken `GetStatus` 15 saniyede bir. `isRunning: true` iken 5 saniyede bir yenilenir; tur bitince
  açık sekme de yenilenir.
- Sekme içerikleri sekme açılınca, işlemlerden sonra ve sağ üstteki "Yenile" ile yüklenir; sürekli yoklama
  gerekmez.
- Tarayıcı sekmesi arka plandayken (`document.visibilityState === 'hidden'`) yoklama durdurulur.
- Sayfadan çıkınca abonelikler kapatılır (`takeUntilDestroyed`).

### 1.7 Yetki

Uç noktalar projedeki diğerleri gibi henüz `[Authorize]` değil. Sayfa ve menü öğesi yalnızca yönetici
rollerine gösterilmeli. Paket indirme evrakın içeriğini (üst yazı, ekler) verdiği için özellikle önemli.

---

## 2. API tipleri

Başarılı cevaplar camelCase. Hatalar: `500 {"Message":"...","StatusCode":500}` (diğer uç noktalarla aynı).

**Zamanlar UTC.** Bir kısmı sonunda `Z` ile (`"2026-10-14T09:06:49.02Z"`, bellekten gelenler), bir kısmı
`Z`'siz (`"2026-10-11T19:59:36.18"`, veritabanından gelenler) döner. İkisi de UTC'dir. `Z`'siz değer
`new Date()`'e verilirse yerel saat sanılır; aşağıdaki `utc()` yardımcısı kullanılmalı.

`status` (PublishStatusEnum):

| Değer | Etiket | Renk önerisi |
|---|---|---|
| 1 veya null | Yayınlanmadı | gri |
| 2 | Sırada | mavi |
| 3 | Aktarılıyor | mavi + spinner |
| 4 | Yayınlandı | yeşil |
| 5 | Aktarım Hatalı | kırmızı |

```ts
// atlas-transfer.models.ts

export interface AtlasTransferRunResult {
  ran: boolean;            // false: tur iş yapmadı, nedeni message'da
  picked: number;
  succeeded: number;
  requeued: number;        // geçici hata, tekrar denenecek
  failed: number;          // Aktarım Hatalı'ya düşen
  recoveredStuck: number;  // takılı kalıp sıraya geri alınan
  message: string;
}

export interface AtlasTransferStats {
  notPublished: number;
  queued: number;
  transferring: number;
  succeeded: number;
  failed: number;
}

export interface AtlasTransferLastRun {
  startedAt: string;
  finishedAt: string;
  manual: boolean;
  result: AtlasTransferRunResult | null;
  error: string | null;
}

export type AtlasTransferCheckCode = 'AutoTransfer' | 'Recipient' | 'PhysicalDeliveryPdf' | 'AtlasDelivery';

export interface AtlasTransferCheck {
  code: AtlasTransferCheckCode;
  name: string;
  ok: boolean;
  message: string;
}

export interface AtlasTransferStatus {
  autoTransferEnabled: boolean;
  intervalSeconds: number;
  batchSize: number;
  maxTryCount: number;
  isRunning: boolean;
  runningSince: string | null;
  lastRun: AtlasTransferLastRun | null;
  nextRunAt: string | null;
  lastAttemptAt: string | null;
  lastSuccessAt: string | null;
  counts: AtlasTransferStats;
  checks: AtlasTransferCheck[];
}

export interface AtlasTransferQueueItem {
  documentId: string;
  qrCode: string;
  subject: string | null;
  status: 2 | 3;
  statusUpdatedAt: string | null;
  tryCount: number;
  nextAttemptAt: string | null;
  lastError: string | null;
}

export interface AtlasTransferFailedDocument {
  documentId: string;
  qrCode: string;
  subject: string | null;
  tryCount: number;
  lastError: string | null;
  lastAttemptAt: string | null;
}

export interface AtlasTransferAttempt {
  documentId: string;
  qrCode: string;
  subject: string | null;
  packageId: string;
  attemptNo: number;
  attemptedAt: string;
  isSuccess: boolean;
  isPermanentFailure: boolean;
  error: string | null;
  atlasReferenceId: string | null;
}

export interface AtlasTransferPackage {
  id: string;
  isActive: boolean;
  createdDate: string;
  fileName: string | null;
  fileSize: number | null;
  sha256: string | null;
  fileExists: boolean;
  usedPlaceholderContent: boolean;
  tryCount: number;
  lastAttemptAt: string | null;
  nextAttemptAt: string | null;
  lastError: string | null;
  atlasReferenceId: string | null;
  transferredAt: string | null;
}

export interface AtlasTransferDocument {
  documentId: string;
  qrCode: string;
  subject: string | null;
  status: number | null;
  statusUpdatedAt: string | null;
  packages: AtlasTransferPackage[];
  attempts: AtlasTransferAttempt[];
}

/** Backend'in UTC zamanını Date'e çevirir; sonunda Z/offset yoksa UTC kabul eder. */
export function utc(value: string | null | undefined): Date | null {
  if (!value) return null;
  return new Date(/[zZ]|[+-]\d\d:\d\d$/.test(value) ? value : value + 'Z');
}
```

```ts
// atlas-transfer.service.ts

@Injectable({ providedIn: 'root' })
export class AtlasTransferService {
  private readonly http = inject(HttpClient);
  private readonly base = 'api/AtlasTransfer';

  getStatus()   { return this.http.get<AtlasTransferStatus>(`${this.base}/GetStatus`); }
  getQueue()    { return this.http.get<AtlasTransferQueueItem[]>(`${this.base}/GetQueue`); }
  getFailed()   { return this.http.get<AtlasTransferFailedDocument[]>(`${this.base}/GetFailed`); }
  getAttempts(limit = 50) {
    return this.http.get<AtlasTransferAttempt[]>(`${this.base}/GetAttempts`, { params: { limit } });
  }
  getDocument(documentId: string) {
    return this.http.get<AtlasTransferDocument>(`${this.base}/GetDocument/${documentId}`);
  }
  run() { return this.http.post<AtlasTransferRunResult>(`${this.base}/Run`, {}); }
  retry(documentId: string, userId: string) {
    return this.http.post<{ message: string }>(`${this.base}/Retry`, { documentId, userId });
  }

  /** Paketi indirir; hata cevabı blob geldiği için mesajı okuyup fırlatır. */
  async downloadPackage(documentId: string, packageId?: string, fallbackName = 'paket.eyp') {
    const params: Record<string, string> = packageId ? { packageId } : {};
    try {
      const response = await firstValueFrom(this.http.get(`${this.base}/DownloadPackage/${documentId}`,
        { params, responseType: 'blob', observe: 'response' }));
      const disposition = response.headers.get('Content-Disposition') ?? '';
      const match = /filename\*=UTF-8''([^;]+)|filename="?([^";]+)"?/i.exec(disposition);
      const name = match ? decodeURIComponent(match[1] ?? match[2]) : fallbackName;
      const url = URL.createObjectURL(response.body!);
      const a = document.createElement('a');
      a.href = url; a.download = name; a.click();
      URL.revokeObjectURL(url);
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.error instanceof Blob) {
        const body = JSON.parse(await e.error.text());
        throw new Error(body.Message ?? 'Paket indirilemedi.');
      }
      throw e;
    }
  }
}
```

---

## 3. Bilinmesi gerekenler

- **Atlas'a gönderim şu an sahte.** Paketler hazırlanıp arşivleniyor ama Atlas'a gitmiyor, "Yayınlandı"
  sayılıyor; `atlasReferenceId` `FAKE-…` ile başlar. `AtlasDelivery` kontrolü bunu gösterir. Gerçek teslim
  gelince sayfada değişiklik gerekmez.
- **Geliştirme veritabanında aktarım şu an çalışmıyor:** `AtlasEypRecipientKkk` / `AtlasEypRecipientName`
  boş (`Recipient` kontrolü kırmızı). Sayfayı denemek için Ayarlar'dan doldurulmalı (Bakanlık: `24305112` /
  `DIŞİŞLERİ BAKANLIĞI`). Otomatik aktarım kapalıyken "Şimdi Çalıştır" ile tur elle çalıştırılabilir.
- Geliştirme veritabanında henüz paket ve deneme kaydı yok. Son Denemeler ve paket listeleri ancak ilk tur
  çalışınca dolar.
- Listeler en fazla 500 kayıt döner.

**Yapılacaklar:**

- [ ] `atlas-transfer.models.ts` ve `atlas-transfer.service.ts` (2. bölüm)
- [ ] Atlas Aktarım sayfası ve yönetici menüsüne bağlantı: durum kartı, kontroller, sayı kartları, Sırada / Hatalı / Son Denemeler sekmeleri
- [ ] Şimdi Çalıştır, Yeniden Dene, Paketi İndir
- [ ] Evrak aktarım detayı penceresi; gelen evrak detayına Aktarım sekmesi olarak da eklenmesi
- [ ] "Nasıl çalışır?" penceresi: [atlas_eyp_aktarimi.md](atlas_eyp_aktarimi.md) 1. bölümdeki metin
- [ ] Yoklama (15 sn, çalışırken 5 sn, sekme gizliyken durur)
- [ ] Geliştirme ortamında Ayarlar'dan alıcı kurumu doldurup "Şimdi Çalıştır" ile uçtan uca dene
