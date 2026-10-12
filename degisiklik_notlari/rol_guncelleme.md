# Roller: güncelleme ve silme

**Tarih:** 2026-10-12 (son güncelleme: 2026-10-12, backend — `Roles/Delete` eklendi)
**Etkilenen:** Roller sayfası (`/users/role`): aktif / pasif anahtarı, Rolü Güncelle penceresi, Sil butonu; girişte token'a yazılan roller
**Sorumlu taraf:** Backend 1. ve 2. maddeleri bitirdi — sıra frontend doğrulamasında

---

## 👉 Frontend için (2026-10-12)

Backend tarafında Roller sayfasının beklediği her şey hazır; ön yüzde kod değişikliği gerekmiyor, yalnızca doğrulama ve Kurye Takip temizliği kaldı.

**Çalışır hale gelenler:**

- **Aktif / pasif anahtarı ve Rolü Güncelle penceresi** artık kaydediyor (1. madde). Sayfa yenilenince yeni değer gelmeli.
- **Kullanıcıdan rol kaldırma** artık kaydediyor; daha önce kaldırılmış bir rolü **yeniden atama** da çalışıyor (1. madde, aynı hata). Kullanıcı rol atama ekranında da kontrol edilebilir.
- **`POST api/Roles/Delete`** eklendi (2. madde); frontend'in gönderdiği gövdeyle aynı: `{ "id": "..." }`.

**`Roles/Delete` cevapları** (geliştirme veritabanında denendi):

| Durum | HTTP | Gövde |
|---|---|---|
| Silindi | 200 | `{"message":"Rol silindi.","data":null}` |
| Rol bir kullanıcıya atanmış | 500 | `{"Message":"\"<ad>\" rolü N kullanıcıda atanmış; önce rolü kullanıcılardan kaldırın.","StatusCode":500}` |
| Rol yok ya da zaten silinmiş | 500 | `{"Message":"Rol bulunamadı.","StatusCode":500}` |
| Boş `id` | 500 | `{"Errors":["Rol Id Boş Olamaz!"],"StatusCode":403}` |

Hata biçimleri `Update` ile aynı (3. ve 4. maddeler hâlâ açık); Roller sayfası bunları zaten okuyor.

**Bilinmesi gerekenler:**

- Silme kaydı yalnızca işaretler (`isDeleted: true`); `Roles/GetAll` silinmişleri döndürmeye devam eder, frontend'in gizlemesi sürmeli.
- Silinmiş bir rolün adı serbest kalır; aynı adla yeni rol oluşturulabilir.
- Backend sistem rollerini **korumuyor** (6. madde): Sil butonunun sistem rollerinde kapalı olması tek koruma. Bu rollere atanmış kullanıcı olduğu sürece backend de silmeyi reddeder, ama atanmış kimse yoksa siler.
- Silinen / pasif yapılan rol, kullanıcıların elindeki token'da süresi dolana kadar kalır.
- Testte veritabanında iki silinmiş geçici rol kaldı ("Silme Testi (geÃ§ici)" — test betiğinin kodlama hatasıyla bozuk adla — ve "Silme Testi 2"). Silinmiş oldukları için listede görünmezler.

**Yapılacaklar:**

- [ ] Roller sayfasında doğrula: anahtar, Rolü Güncelle, Sil (atanmış rolde hata mesajı dahil) — API düzeyinde doğrulandı (2026-10-12): Kurye Takip pasif → kaydedildi, `updateDate` doldu; aktif → kaydedildi
- [ ] Kullanıcı rol atama ekranında kaldırma / yeniden atamayı doğrula
- [x] "Kurye Takip" silindi (2026-10-12, `Roles/Delete` → "Rol silindi.", `isDeleted: true`; atanmış kullanıcı yoktu)
- [x] Frontend'deki simge ve gizli rol tanımlarından kaldırıldı (`role-service.ts`; gizli rol yalnızca Admin)

---

## ⚠️ Açık işler (backend)

Öncelik sırasıyla. 1. ve 2. maddeler kapandı; kalanlar karar / iyileştirme bekliyor.

### 1. ✅ DÜZELTİLDİ — `Roles/Update` 200 dönüyor ama hiçbir şey kaydetmiyordu

**Belirti:** Kullanıcı rolü pasif yapıyor, "Rol başarıyla güncellendi" geliyor; sayfaya tekrar gelince rol yine aktif. Ad değişikliği de kaydedilmiyor.

**Test:** "Kurye Takip" → `isActive: false` → HTTP 200, ardından `GetAll`'da hâlâ `isActive: true`, `updateDate: null`.

**Sebep:** `RoleService.UpdateAsync` rolü `_roleRepository.Where(...)` ile okuyor. Bu projedeki `GenericRepository`'de `Where()` / `GetAll()` takipsiz (`AsNoTracking`) sorgu döndürür; yapılan atamalar EF'e ulaşmaz ve `SaveChangesAsync` hiçbir şey yazmaz. Diğer servisler kayıttan önce `Update` çağırıyor (ör. `AppSettingService.cs:71`, `DocumentAllocationRequestService.cs:221`).

**Düzeltme** (`DijitalEvrakTakip.Persistance/Services/RoleService.cs` → `UpdateAsync`):

```csharp
role.Name = request.Name.Trim();
role.IsActive = request.IsActive;
role.UpdateDate = DateTime.UtcNow;
_roleRepository.Update(role);          // eksik satır
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

(Ya da okumayı `GetAllWithTracking().Where(...)` ile yapmak — `DocumentAllocationRequestService.cs:649` gibi.)

**Tarama:** Aynı kalıp (takipsiz okuyup `Update` çağırmadan kaydetme) tüm servislerde tarandı. Başka iki yerde de vardı, ikisi de daha eskiden beri:

- `UserRoleService.RemoveRoleFromUser`: kullanıcıdan rol alma kaydedilmiyordu; rol token'a girmeye devam ediyordu.
- `UserRoleService.AssignRolesAsync`: daha önce alınmış bir rolü yeniden atama kaydedilmiyordu (ilk kez atama çalışıyordu).

Üçüne de `Update(...)` eklendi. Benzer görünen ~30 yer zaten doğruydu (`Update` çağırıyor ya da izlenen sorguyla okuyor).

**Doğrulama (2026-10-12, geliştirme veritabanı):**

| Deneme | Sonuç |
|---|---|
| "Kurye Takip" → `isActive: false` | kaydedildi, `updateDate` doldu ✅ |
| "Kurye Takip" → ad "Kurye Takip Test" | kaydedildi ✅ (sonra eski ad / durum geri yüklendi) |
| Rolsüz bir kullanıcıya "Kurye Takip" ata → kaldır → yeniden ata → kaldır | `GetRolesByUserId` her adımda doğru ✅ |

Test sonunda "Kurye Takip" yine aktif ve hiçbir kullanıcıda atalı değil (`Users/GetAll?roleId=...` → 0); test kullanıcısında yalnızca silinmiş işaretli bir atama kaydı kaldı.

### 2. ✅ EKLENDİ — `POST api/Roles/Delete` uç noktası yoktu

**Neden gerekli:** "Kurye Takip" rolü kullanılmıyor ve silinecek. Rolü taşıyan kullanıcı yok (`Users/GetAll?roleId=22ef19a3-99b2-4041-9a59-deec206e4362` → 0), backend ve frontend kodunda yetki olarak geçmiyor, seed ile eklenmemiş. Uç nokta olmadığı için frontend'den silinemiyor (`POST api/Roles/Delete` → 404).

**İstenen** (`CreateRole` / `UpdateRole` kalıbında):

```csharp
public sealed record DeleteRoleCommand(Guid Id) : IRequest<MessageResponse>;
```

- **Silme işaretlemedir:** `IsDeleted = true`, `UpdateDate` set edilir; fiziksel silme yok. **1. maddedeki `Update` satırı burada da gerekli.**
- **Kullanımdaki rol silinemez:** Silinmemiş bir `UserRole` kaydı varsa hata: `"<ad>" rolü N kullanıcıda atanmış; önce rolü kullanıcılardan kaldırın.`
- Rol yoksa ya da zaten silinmişse: "Rol bulunamadı."
- **Validator:** `Id` boş olamaz.
- **Cevap:** `MessageResponse` ("Rol silindi.").
- **Gövde:** `{ "id": "..." }` (`Languages/Delete` ile aynı).

**Yapılan:** İstendiği gibi eklendi (`RoleFeatures/Commands/DeleteRole`, `RoleService.DeleteAsync`). Atama sayısı silinmemiş `UserRole` kayıtlarından sayılır.

**Doğrulama (2026-10-12, geliştirme veritabanı, geçici test rolleriyle — "Kurye Takip" silinmedi):**

| Deneme | Sonuç |
|---|---|
| Geçici rol oluştur, kullanıcıya ata, sil | `"Silme Testi 2" rolü 1 kullanıcıda atanmış; önce rolü kullanıcılardan kaldırın.` ✅, rol silinmedi |
| Atamayı kaldır, sil | "Rol silindi.", `isDeleted: true` ✅ |
| Aynı rolü tekrar sil | "Rol bulunamadı." ✅ |
| Olmayan `id` | "Rol bulunamadı." ✅ |
| Boş `id` | "Rol Id Boş Olamaz!" ✅ (HTTP kodu yanlış, bkz. 3. madde) |
| Başka bir rolü silinmiş rolün adıyla güncelle | izin verildi ✅ (sonra geri alındı) |

### 3. HATA — Doğrulama hatası yanlış HTTP koduyla dönüyor

Boş `id` ile `Roles/Update`:

```
HTTP 500   {"Errors":["Rol Id Boş Olamaz!"],"StatusCode":403}
```

- HTTP durum kodu **500**, gövdedeki `StatusCode` **403**: ikisi birbirini tutmuyor ve ikisi de yanlış. Doğrulama hatası **400** olmalı. 500 olduğu için frontend'in genel hata yakalayıcısı bunu "Sunucu Hatası — Sistem geçici olarak hizmet veremiyor" diye gösteriyor (Roller sayfası bu yüzden genel uyarıyı kapatıp mesajı kendisi gösteriyor).
- `ErrorMiddleware`'de `ValidationException` → 400 eşlemesi yapılmalı. Bu tüm uç noktaları etkiler.

### 4. TUTARSIZLIK — Hata gövdesinin alan adları

| Durum | Gövde |
|---|---|
| Başarılı | `{"message": "...", "data": null}` (küçük harf) |
| İş kuralı hatası (`Exception`) | `{"Message": "...", "StatusCode": 500}` (büyük harf) |
| Doğrulama hatası | `{"Errors": ["..."], "StatusCode": 403}` |

Frontend üçünü de okuyor (`incomingErrorMessage`), ama tek biçime (küçük harf, camelCase) çekilmesi önerilir. Önceki notta hata alanı `message` diye yazılmıştı; gerçekte `Message`.

### 5. RİSK — Rol uç noktalarında yetki kontrolü yok

Backend'de hiçbir uç noktada `[Authorize]` yok. `Roles/Create`, `Roles/Update` ve `Roles/Delete` token olmadan çağrılabiliyor: Roller sayfası yalnızca Yönetici'ye açık olsa da bu koruma yalnızca arayüzde. Bu uç noktaların en azından Yönetici rolü istemesi önerilir.

### 6. RİSK — Sistem rollerinin adı değiştirilebiliyor / silinebilecek

Frontend menü ve sayfa yetkilerini rol **adıyla** eşleştiriyor (`navigation.ts`, `app.routes.ts`). Şu roller adıyla kullanılıyor: **Yönetici, Gelen Evrak, Giden Evrak, Ön Kayıt, Birim Yöneticisi, Birim Evrak Sorumlusu**. Bunların adı değişirse ya da silinirse o roldeki kullanıcılar ekranlara erişimini kaybeder.

- Frontend: bu rollerde Sil butonu kapalı; Rolü Güncelle penceresinde ad alanı kilitli (2026-10-12).
- Backend: koruma yok. 5. madde nedeniyle kesin koruma ancak backend'de olur: korumalı ad listesi, `Update`'te ad değişikliğini ve `Delete`'i reddetsin.

---

## Bilinmesi gerekenler

- **Değişiklik bir sonraki girişte geçerli olur:** Roller girişte token'a yazılır. Rol pasif yapıldığında, adı değiştiğinde ya da silindiğinde kullanıcının elindeki token süresi dolana / yeniden giriş yapana kadar eski rolleri taşır.
- **Silinmiş roller `GetAll`'da gelir:** `Roles/GetAll` silinmişleri de döner; frontend Roller sayfasında, atama ekranında ve rol süzgecinde bunları gizliyor. Değişiklik gerekmez.

---

## Tamamlananlar

### Girişteki rol süzgeci ✅ (`97d561a`)

`UserRoleService.GetRolesByUserIdAsync` hiçbir süzgeç uygulamıyordu: pasif roller, silinmiş roller ve kullanıcıdan alınmış rol atamaları da token'a girip yetki vermeye devam ediyordu. Artık:

```csharp
.Where(x => x.UserId == userId && !x.IsDeleted && !x.Role.IsDeleted && x.Role.IsActive)
```

Hem giriş (`UserLoginFlow`, Login ve WindowsLogin) hem `UserRole/GetRolesByUserId` düzeldi.

### `POST api/Roles/Update` eklendi ✅ (`da75701`; kaydetme hatası düzeltildi, bkz. 1. madde)

```csharp
public sealed record UpdateRoleCommand(Guid Id, string Name, bool IsActive) : IRequest<MessageResponse>;
```

| Deneme | Sonuç |
|---|---|
| Boş `id` | "Rol Id Boş Olamaz!" ✅ (HTTP kodu yanlış, bkz. 3. madde) |
| Olmayan `id` | "Rol bulunamadı." ✅ |
| "Kurye Takip" → ad "kurye" | `"kurye" adında başka bir rol var.` ✅ (Türkçe, harf duyarsız) |
| "Kurye Takip" → `isActive: false` | İlk sürümde kaydedilmiyordu ❌ → düzeltmeden sonra kaydediliyor ✅ |

### Ad çakışması kuralı ✅ (`da75701`)

Create ve Update'te: başka bir (silinmemiş) rol aynı ada sahipse hata. Veride aynı adlı rol yok (11 rol kontrol edildi).

---

## Frontend durumu

- [x] Anahtar ve pencere tam gövde gönderir: `{ id, name, isActive }`
- [x] Backend'in hata mesajı gösterilir (`Message` / `Errors`), genel "Sunucu Hatası" uyarısı çıkmaz
- [x] Sil butonu: onaylı, `POST api/Roles/Delete { id }`; sistem rollerinde kapalı
- [x] Silinmiş roller listelenmez
- [x] Sistem rollerinde ad alanı kilitli (2026-10-12)
- [x] Kurye Takip frontend'deki simge ve gizli rol tanımlarından kaldırıldı

## Durum

- [x] Girişteki rol süzgeci düzeltildi
- [x] `Roles/Update` eklendi
- [x] **1.** `Roles/Update` kaydetmiyordu — `Update` satırı eklendi; rol kaldırma / yeniden atamadaki aynı hata da düzeltildi
- [x] **2.** `Roles/Delete` eklendi
- [ ] **3.** Doğrulama hatası 400 dönüyor
- [ ] **4.** Hata gövdesi tek biçimde (isteğe bağlı)
- [ ] **5.** Rol uç noktalarında yetki kontrolü
- [ ] **6.** Sistem rolleri backend'de korunuyor
- [x] Kurye Takip silindi (2026-10-12)
- [ ] Roller sayfasında doğrulandı (aktif / pasif, güncelleme, silme)
