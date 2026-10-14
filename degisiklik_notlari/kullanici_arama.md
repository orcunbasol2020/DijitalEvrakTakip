# Kullanıcı araması: ad ve soyad birlikte yazılınca sonuç dönmüyor

**Tarih:** 2026-10-12
**Etkilenen:** `api/Users/GetAll?search=` (Kullanıcılar sayfası ve kişi seçiciler)
**Sorumlu taraf:** Backend — frontend değişikliği gerekmiyor

## Sorun

- `search=ahmet` → 221 kayıt
- `search=çelikkaya` → 1 kayıt
- `search=ahmet çelikkaya` → **0 kayıt**

Arama metni bölünmeden tek parça olarak her alanda (Name / Surname / UserName) aranıyor gibi görünüyor.

## Neden

`UserService.Search` arama metnini bölmeden tek parça olarak Name / Surname / UserName alanlarında arıyordu. "ahmet çelikkaya" bu alanların hiçbirinde tek başına geçmediği için sonuç boş dönüyordu.

## Yapılan düzeltme

`DijitalEvrakTakip.Persistance/Services/UserService.cs` → `Search`: metin boşluklardan bölünüyor; her kelimenin bu üç alandan en az birinde geçmesi şart koşuluyor. Türkçe büyük / küçük harf eşleşmesi (`EF.Functions.Collate(..., SqlCollations.Turkish)`) korundu.

```csharp
foreach (string term in search.Split(' ', StringSplitOptions.RemoveEmptyEntries))
{
    query = query.Where(u =>
        EF.Functions.Collate(u.Name, SqlCollations.Turkish).Contains(term)
        || EF.Functions.Collate(u.Surname, SqlCollations.Turkish).Contains(term)
        || EF.Functions.Collate(u.UserName, SqlCollations.Turkish).Contains(term));
}
```

Böylece "ahmet çelik" ve "çelikkaya ahmet" gibi aramalar da çalışır.

## Notlar

- Düzeltme frontend'de yapılmamalı: istemcide süzmek için tüm kullanıcıların gelmesi gerekir, oysa hiçbir süzgeç verilmezse sunucu en fazla 200 kayıt döner (`GetAllUserQuery.MaxUnfilteredCount`). Arama metni verildiğinde bu sınır uygulanmaz ("ahmet" 221 sonuç döner).
- Collation `Turkish_CI_AS`: büyük / küçük harfe duyarsız, aksana duyarlı. "celikkaya" → "Çelikkaya" eşleşmez (Türkçede c ve ç ayrı harf). İstenirse ayrıca ele alınmalı.

## Durum

- [x] Backend düzeltmesi yapıldı (2026-10-12)
- [x] API doğrulandı (2026-10-12): `ahmet çelikkaya` → 1, `çelikkaya ahmet` → 1, `ahmet çelik` → 3 kayıt
- [ ] Kullanıcılar sayfasında doğrulandı
