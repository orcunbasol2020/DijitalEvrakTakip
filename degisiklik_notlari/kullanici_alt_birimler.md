# Kullanıcı listesi: birim süzgecine alt birimlerdeki personel de gelsin

**Tarih:** 2026-10-12
**Etkilenen:** `api/Users/GetAll?departmentId=` (Kullanıcılar sayfası → "Birimim" butonu)
**Sorumlu taraf:** Backend — ardından frontend'de tek parametre eklenecek

## İstek

Kullanıcılar sayfasındaki "Birimim" butonu giriş yapan kullanıcının birimiyle `departmentId` gönderiyor. Bu listede yalnızca o birime kayıtlı kişiler değil, `parentId` zinciriyle o birime bağlı tüm alt birimlerdeki kişiler de gelmeli.

## Mevcut durum

- `departmentId=11082872` (Bakan) → 0 kişi
- `departmentId=1100212` (BÖZK, `parentId` = 11082872) → 59 kişi
- `departmentId=11082872&departmentId=1100212` → 0 kişi
- `departmentIds=11082872&departmentIds=1100212` → 200 kişi (parametre tanınmıyor, süzgeçsiz sınır)

`GetAll` yalnızca tam o birime kayıtlı kişileri dönüyor; birden fazla birim ya da alt birim seçeneği yok.

## Önerilen değişiklik

`Users/GetAll`'a yeni parametre: **`includeSubDepartments`** (bool, varsayılan `false`).

- `true` ise `departmentId` ve onun `parentId` zinciriyle bağlı **tüm düzeylerdeki** alt birimleri bulunur; bu birimlerin hepsindeki kullanıcılar döner.
- `false` ya da yoksa bugünkü davranış aynen kalır (kişi seçiciler vb. etkilenmez).
- Diğer süzgeçlerle (`search`, `roleId`) birlikte çalışmalı.
- Alt birimler sorguda bulunmalı (ör. recursive CTE ya da birim ağacını belleğe alıp kimlik listesi çıkarmak); birim başına ayrı sorgu değil.

## Yapılan değişiklik

`GetAllUserQuery`'ye `IncludeSubDepartments` (varsayılan `false`) eklendi. `GetAllUserHandler`, parametre `true` ve `departmentId` dolu olduğunda birimin kendisini ve tüm alt birimlerini `IDepartmentService.GetDescendantIdsAsync(..., includeSelf: true)` ile bulur; kullanıcılar bu birimlerden herhangi birine kayıtlıysa döner. Birim ağacı tek sorguda çekilir.

```
GET api/Users/GetAll?departmentId=1100212&includeSubDepartments=true
GET api/Users/GetAll?departmentId=1100212&includeSubDepartments=true&search=ahmet&roleId=...
```

- `departmentId` verilmezse parametrenin etkisi yoktur.
- `search`, `roleId`, `onlyWithRoles` aynen birlikte çalışır.
- Cevap biçimi değişmedi (`UserDto`, roller dahil); sıralama ad, soyad.
- Pasif kullanıcılar da gelir (`GetAll`'ın bugünkü davranışı); `GetByParentDepartment`'tan farklı olarak yalnızca aktifler süzülmez.

## Netleştirilecek

- [x] **Misyonlar:** Birim ağacında misyonlar da var (`Departments/GetAll?includeMissions=true`). Bir birime bağlı misyonların personeli de gelsin mi? Frontend önerisi: hayır, yalnızca merkez birimler. → Kaynakta misyonlar merkez birimlerin `parentId` zincirinde yer almıyor; ek süzgeç gerekmedi.
- [ ] **Sonuç sınırı:** Üst düzey bir birimde (ör. Bakan) sonuç binlerce kişi olabilir. Süzgeçli istekte sınır yok; gerekirse bir üst sınır ya da sayfalama konmalı.
- [ ] **Kimlere açılacağı (frontend kararı):** Alt birim listesi yalnızca bazı kullanıcılara açılacak. Hangi kullanıcıların (rol, ayar ...) göreceğine frontend karar verecek.

## Frontend tarafı

`src/pages/users/users.ts` → `result`: Birimim açıkken ve kullanıcı bu özelliğe yetkiliyse `includeSubDepartments: true` gönderilecek; diğer kullanıcılarda gönderilmeyecek (bugünkü davranış). Kişinin hangi alt birimde olduğu Birimi sütununda zaten görünüyor.

- Kişi seçiciler ve diğer `GetAll` çağrıları değişmez; parametre gönderilmedikçe eski davranış sürer.
- Kısıt yalnızca arayüzde: backend bu parametreyi kullanıcıya göre kısıtlamaz. Oturum açmış herkes `departmentId` ile herhangi bir birimin personelini zaten alabildiği için yeni bir veri açılmış olmaz. İleride gerçek bir yetki kısıtı gerekirse backend'de ele alınmalı.
- Üst düzey bir birimde (ör. Bakan) liste çok uzun olabilir; sınır kararı verilene kadar bunu hesaba katın.

## Durum

- [x] Backend ekibine iletildi
- [x] Backend düzeltmesi yapıldı (`GetAllUserHandler`, alt birimler `IDepartmentService.GetDescendantIdsAsync` ile bulunur)
- [ ] Frontend parametresi eklendi
- [ ] Kullanıcılar sayfasında doğrulandı
