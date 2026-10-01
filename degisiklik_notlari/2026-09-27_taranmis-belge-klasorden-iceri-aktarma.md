# Taranmış Belgelerin Klasörden İçe Aktarılması

**Tarih:** 2026-09-27
**Commit:** "Import scanned PDFs from a watched folder and fix scanned document list" (feature/incoming-document-stats)
**Migration:** `20260927120000_AddScanImportSettings` (uygulandı)

## Amaç

Tarayıcı PDF'leri bir klasöre bırakıyor. Bu dosyaların otomatik olarak taranmış belge kaydına dönüşmesi ve eşleştirme listesine düşmesi gerekiyordu. Klasör yolu, arayüzden değiştirilebilmesi için AppSettings tablosunda tutuluyor.

## Ayarlar (AppSettings tablosu)

| Anahtar | Varsayılan | Açıklama |
|---|---|---|
| `ScanImportFolderPath` | `C:\EvrakTakip\belgeler\scanned` | Tarayıcının PDF bıraktığı klasör |
| `ScanImportEnabled` | `true` | Otomatik içe aktarma açık mı |
| `ScanImportIntervalSeconds` | `30` | Kontrol aralığı, en az 10 saniye |

Ayarlar her turda yeniden okunur. Değiştirildiklerinde uygulamayı yeniden başlatmak gerekmez.

Dosyaların taşındığı hedef klasör değişmedi. appsettings.json'daki `FileStorage:IncomingDocumentsPath` değeri kullanılıyor, yani Processed klasörü.

## Çalışma şekli

1. API açıldıktan on saniye sonra arka plan servisi başlar ve ayardaki aralıkla çalışır.
2. İçe aktarma kapalıysa, klasör ayarı boşsa ya da klasör yoksa tur atlanır.
3. Klasörün yalnızca üst seviyesindeki `.pdf` dosyaları alınır.
4. Son on saniyede değişmiş ya da başka bir program tarafından açık tutulan dosyalar bir sonraki tura bırakılır. Böylece tarayıcının yazmayı bitirmediği dosyalar alınmaz.
5. Dosya `<orijinal ad>_<benzersiz kod>.pdf` adıyla Processed klasörüne taşınır. Ad, 200 karakterlik yol sınırına sığacak şekilde kısaltılır.
6. Evrak numarası boş bir taranmış belge kaydı oluşturulur:
   - `FileName`: Processed klasöründeki yeni ad
   - `OriginalPath`: Dosyanın tarayıcıdan gelen orijinal adı
   - `NewPath`: Yeni tam yol
7. Kayıt veritabanına yazılamazsa dosya scanned klasörünün altındaki `Error` klasörüne taşınır. Hata, `ErrorLogs` tablosuna `ScannedDocumentImport` yoluyla yazılır.

Arka plan servisi ile elle tetikleme aynı anda çalışmaz. Biri çalışırken diğeri "zaten çalışıyor" yanıtı döner.

Dosya değişikliği bildirimleri yerine periyodik tarama seçildi. Bildirimler ağ paylaşımlarında güvenilir değil ve uygulama kapalıyken gelen dosyaları görmüyor. Periyodik taramada uygulama açıldığında birikmiş dosyalar da alınıyor.

## Endpoint değişiklikleri

### Yeni: `POST api/ScannedDocuments/ImportFromFolder`

Klasörü hemen kontrol eder. Otomatik içe aktarma kapalı olsa da çalışır.

Örnek yanıt:

```json
{
  "ran": true,
  "imported": 3,
  "skipped": 1,
  "failed": 0,
  "message": "3 belge içe aktarıldı, 1 belge sonraki tura bırakıldı, 0 belge hatalı."
}
```

### Düzeltildi: `GET api/ScannedDocuments/GetAll`

- `fileName`, `startDate` ve `endDate` filtreleri artık çalışıyor. Önceden controller istekten parametre almıyordu ve tarih filtreleri serviste hiç kullanılmıyordu.
- Sonuçlar en yeniden eskiye sıralanıyor.
- Saat verilmeyen `endDate` için o günün tamamı dahil ediliyor.

### Değişti: `PUT api/ScannedDocuments/Update` (evrak numarası eşleştirme)

- **Tekrar kullanılan numara reddediliyor:** Numara silinmemiş başka bir taranmış belgeye verilmişse ya da bu numaralı gelen evrağa zaten bir dosya bağlıysa istek "numarası daha önce başka bir taranmış evrakla eşleştirilmiş" hatasıyla reddedilir. Önceden eşleştirme mevcut dosyanın üzerine yazıyordu.
- **Otomatik zimmet:** Eşleşen gelen evrağın aktif zimmeti yoksa (ya da gelen evrak bu istekle yeni oluşturulduysa) evrak, eşleştirmeyi yapan kullanıcıya zimmetlenir. `DocumentAllocations/Create` ile aynı kayıtlar oluşur: `OnKayit` durumunda aktif bir `DocumentAllocation` ve `Zimmet` türünde bir `DocumentTransaction`. Tümü tek `SaveChanges` ile kaydedilir.
- İstekteki `UserId` geçerli bir Guid değilse zimmet oluşturulmaz, eşleştirme yine yapılır.

## Eklenen ve değişen dosyalar

- `DijitalEvrakTakip.Domain/Constants/AppSettingKeys.cs`: Üç yeni anahtar
- `DijitalEvrakTakip.Domain/Dtos/ScannedDocumentImportResultDto.cs`
- `DijitalEvrakTakip.Application/Services/IScannedDocumentImportService.cs`
- `DijitalEvrakTakip.Application/Features/ScannedDocumentFeatures/Commands/ImportScannedDocuments/`
- `DijitalEvrakTakip.Persistance/Services/ScannedDocumentImportService.cs`
- `DijitalEvrakTakip.Persistance/Services/ScannedDocumentService.cs`: Liste filtreleri ve sıralama; eşleştirmede tekrar kullanılan numara kontrolü ve otomatik zimmet
- `DijitalEvrakTakip.Persistance/Migrations/20260927120000_AddScanImportSettings.cs`: Ayar satırları. Model değişikliği olmadığı için snapshot güncellenmedi.
- `DijitalEvrakTakip.WebApi/BackgroundServices/ScannedDocumentImportBackgroundService.cs`
- `DijitalEvrakTakip.WebApi/Program.cs`: Servis kayıtları
- `DijitalEvrakTakip.Presentation/Controllers/ScannedDocumentsController.cs`

## Test

Kod derleniyor ve mevcut birim testi geçiyor. Uçtan uca deneme yapılmadı.

Denemek için API'yi başlatıp bir PDF'i scanned klasörüne kopyalayın. Otuz ile kırk saniye içinde dosya Processed klasörüne taşınmış ve eşleştirme listesinde görünüyor olmalı.

## Dikkat edilecekler

- **IIS:** Uygulama havuzu boşta kaldığında kapanır ve içe aktarma da durur. Havuzun sürekli çalışır şekilde ayarlanması ve önyüklemenin açılması gerekir. Bu mümkün değilse içe aktarma ayrı bir Windows servisine taşınmalı.
- **Klasör yolu güvenliği:** Projede rol bazlı yetkilendirme olmadığı için giriş yapan her kullanıcı klasör yolunu değiştirebilir. Yol belirli bir kök dizinle sınırlanmadı.
- **Aynı disk:** Kaynak ve hedef klasör farklı disklerde ya da ağ paylaşımındaysa taşıma aslında kopyalayıp silmektir. Yarıda kesilen bir işlem aynı dosyanın iki kopyasını bırakabilir.
- **Tarih filtresi:** Kayıt tarihleri UTC tutuluyor ve filtre de UTC'ye göre çalışıyor. Client yerel saat gönderirse gün sınırlarında üç saatlik kayma olur.
