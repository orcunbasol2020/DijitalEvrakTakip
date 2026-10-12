# Roller: güncelleme ve silme

**Tarih:** 2026-10-12 (son güncelleme: 2026-10-12, frontend)
**Etkilenen:** Roller sayfası (`/users/role`): aktif / pasif anahtarı, Rolü Güncelle penceresi, Sil butonu; girişte token'a yazılan roller
**Sorumlu taraf:** Backend — frontend hazır

---

## ⚠️ Açık işler (backend)

Öncelik sırasıyla. Kod durumu `da75701 Add Roles/Update and reject duplicate role names` commit'ine göre.

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

### 2. EKSİK — `POST api/Roles/Delete` uç noktası yok

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

Backend'de hiçbir uç noktada `[Authorize]` yok. `Roles/Create`, `Roles/Update` (ve eklenecek `Roles/Delete`) token olmadan çağrılabiliyor: Roller sayfası yalnızca Yönetici'ye açık olsa da bu koruma yalnızca arayüzde. Bu uç noktaların en azından Yönetici rolü istemesi önerilir.

### 6. RİSK — Sistem rollerinin adı değiştirilebiliyor / silinebilecek

Frontend menü ve sayfa yetkilerini rol **adıyla** eşleştiriyor (`navigation.ts`, `app.routes.ts`). Şu roller adıyla kullanılıyor: **Yönetici, Gelen Evrak, Giden Evrak, Ön Kayıt, Birim Yöneticisi, Birim Evrak Sorumlusu**. Bunların adı değişirse ya da silinirse o roldeki kullanıcılar ekranlara erişimini kaybeder.

- Frontend: bu rollerde Sil butonu kapalı. Ad alanı henüz kilitli değil (karar bekleniyor).
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
- [ ] Sistem rollerinde ad alanını kilitleme (karar bekleniyor)
- [ ] Kurye Takip silinince frontend'deki simge ve gizli rol tanımlarından kaldırılacak

## Durum

- [x] Girişteki rol süzgeci düzeltildi
- [x] `Roles/Update` eklendi
- [x] **1.** `Roles/Update` kaydetmiyordu — `Update` satırı eklendi; rol kaldırma / yeniden atamadaki aynı hata da düzeltildi
- [ ] **2.** `Roles/Delete` eklendi
- [ ] **3.** Doğrulama hatası 400 dönüyor
- [ ] **4.** Hata gövdesi tek biçimde (isteğe bağlı)
- [ ] **5.** Rol uç noktalarında yetki kontrolü
- [ ] **6.** Sistem rolleri backend'de korunuyor
- [ ] Kurye Takip silindi
- [ ] Roller sayfasında doğrulandı (aktif / pasif, güncelleme, silme)
