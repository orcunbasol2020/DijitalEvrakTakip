# Roller: güncelleme uç noktası yok (aktif / pasif ve ad değişikliği çalışmıyor)

**Tarih:** 2026-10-12
**Etkilenen:** Roller sayfası (`/users/role`): satırdaki aktif / pasif anahtarı ve Rolü Güncelle penceresi; girişte token'a yazılan roller
**Sorumlu taraf:** Backend — frontend hazır

## Sorun

Frontend `POST api/Roles/Update` çağırıyor; bu uç nokta backend'de yok:

```
POST api/Roles/Update  →  404
```

`RolesController` yalnızca `Create` ve `GetAll` içeriyor; `RoleFeatures/Commands` altında yalnızca `CreateRole` var, `IRoleService`'te güncelleme metodu yok. Anahtar ekranda değişip istek 404 alınca geri dönüyor ("Durum güncellenemedi"); pencereden ad / durum güncellemesi de hiç kaydedilmiyor.

İnceleme sırasında bulunan ek hata: girişte token'a yazılan rolleri okuyan `UserRoleService.GetRolesByUserIdAsync` hiçbir süzgeç uygulamıyordu. Pasif roller, silinmiş roller ve **kullanıcıdan alınmış (silinmiş) rol atamaları** da token'a giriyor, yani yetki vermeye devam ediyordu. Kullanıcılar sayfasındaki rol listesi (`Users/GetAll`) ise bunları göstermediği için ekranda görünen rollerle token'daki roller farklı olabiliyordu.

## Yapılan düzeltme

### 1. Girişteki rol süzgeci

`DijitalEvrakTakip.Persistance/Services/UserRoleService.cs` → `GetRolesByUserIdAsync`: yalnızca geçerli roller döner (`Users/GetAll`'daki süzgeçle aynı):

```csharp
.Where(x => x.UserId == userId && !x.IsDeleted && !x.Role.IsDeleted && x.Role.IsActive)
```

Bu metodu hem giriş (`UserLoginFlow`, Login ve WindowsLogin) hem de `UserRole/GetRolesByUserId` sorgusu kullanır; ikisi de düzeldi.

### 2. `POST api/Roles/Update`

`RoleFeatures/Commands/UpdateRole` altında, `CreateRole` ile aynı kalıpta:

```csharp
public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    bool IsActive
) : IRequest<MessageResponse>;
```

- **Validator:** `Id` boş olamaz ("Rol Id Boş Olamaz!"), `Name` boş olamaz ("Rol Adı Boş Olamaz!").
- **`IRoleService.UpdateAsync`:** rol yoksa ya da silinmişse "Rol bulunamadı." hatası; varsa `Name` (baştaki / sondaki boşluklar kırpılarak), `IsActive` ve `UpdateDate` güncellenir.
- **Ad çakışması:** başka bir (silinmemiş) rol aynı ada sahipse `"<ad>" adında başka bir rol var.` hatası döner. Karşılaştırma Türkçe, büyük / küçük harfe duyarsız ("yönetici" = "YÖNETİCİ"); güncellenen rolün kendisi sayılmaz. Create'te bu kural hiç yoktu (ad için benzersiz index de yok); **aynı kural Create'e de eklendi.**
- **Cevap:** `MessageResponse` ("Rol başarıyla güncellendi.").
- **Hatalar:** Diğer servislerdeki gibi `Exception` fırlatılır; `ErrorMiddleware` bunu HTTP 500 ve gövdede `message` olarak döner. Frontend hata mesajını bu alandan göstermeli.

Frontend iki yerden de **tam gövde** gönderir (kısmi güncelleme gerekmez):

```json
{ "id": "5d33b593-...", "name": "Yönetici", "isActive": false }
```

## Dikkat

- **Rol adı değişikliği yetkileri etkiler:** Frontend menü ve sayfa yetkilerini rol **adıyla** eşleştiriyor (`navigation.ts`, `app.routes.ts`: "Yönetici", "Gelen Evrak" ...). Bu rollerin adı değiştirilirse ilgili kullanıcılar ekranlara erişimini kaybeder. Backend sistem rollerinin adının değiştirilmesini **engellemiyor**; uyarı ya da engel kararı frontend'den bekleniyor. Backend'de hiçbir uç noktada `[Authorize]` olmadığı için kesin bir koruma ancak backend'de (ör. korumalı rol adları listesi) sağlanabilir.
- **Değişiklik bir sonraki girişte geçerli olur:** Roller girişte token'a yazılır. Rol pasif yapıldığında ya da adı değiştiğinde, kullanıcının elindeki token süresi dolana / yeniden giriş yapana kadar eski rolleri taşır. Bu, rol süzgeci düzeltmesi için de geçerlidir.
- **Mevcut veride aynı adlı roller:** Daha önce çakışma kontrolü olmadığı için veritabanında aynı adlı iki rol varsa, ikisinden birini (adını değiştirmeden) güncellemek bile çakışma hatası verir. Veride bir kez kontrol edilmeli.

## Durum

- [x] Frontend hazır (2026-10-12): anahtar `{ id, name, isActive }` gönderir; pencere zaten tam gövde gönderiyordu
- [x] Backend ekibine iletildi
- [x] Girişteki rol süzgeci düzeltildi (2026-10-12)
- [x] Backend uç noktası eklendi (2026-10-12)
- [ ] API doğrulandı
- [ ] Veride aynı adlı rol olmadığı kontrol edildi
- [ ] Roller sayfasında doğrulandı
