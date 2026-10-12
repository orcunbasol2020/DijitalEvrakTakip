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

## Netleştirilecek

- [x] **Misyonlar:** Birim ağacında misyonlar da var (`Departments/GetAll?includeMissions=true`). Bir birime bağlı misyonların personeli de gelsin mi? Frontend önerisi: hayır, yalnızca merkez birimler. → Kaynakta misyonlar merkez birimlerin `parentId` zincirinde yer almıyor; ek süzgeç gerekmedi.
- [ ] **Sonuç sınırı:** Üst düzey bir birimde (ör. Bakan) sonuç binlerce kişi olabilir. Süzgeçli istekte sınır yok; gerekirse bir üst sınır konmalı.

## Frontend tarafı (backend bittikten sonra)

`src/pages/users/users.ts` → `result`: Birimim açıkken `includeSubDepartments: true` gönderilecek. Kişinin hangi alt birimde olduğu Birimi sütununda zaten görünüyor.

## Durum

- [ ] Backend ekibine iletildi
- [x] Backend düzeltmesi yapıldı (`GetAllUserHandler`, alt birimler `IDepartmentService.GetDescendantIdsAsync` ile bulunur)
- [ ] Frontend parametresi eklendi
- [ ] Kullanıcılar sayfasında doğrulandı
