# Kullanıcı cevaplarına unvan (titleName) eklendi

**Tarih:** 2026-10-13
**Backend dalı:** feature/atlas-transfer-queue
**Durum:** Geliştirme veritabanına (192.168.1.3) uygulandı; kaynak unvan tablosu (1.809 unvan) yüklü.
**Sorumlu taraf:** Frontend — yalnızca gösterim, mevcut akışı bozan değişiklik yok

## Ne değişti

Kullanıcı dönen uçlara yeni alan: **`titleName: string | null`**, kişinin unvanı (ör. "İkinci Katip (KİM)", "Sözleşmeli Bilişim Personeli").

| Uç | Cevap |
|---|---|
| `GET api/Users/GetAll` | her kullanıcıda `titleName` |
| `GET api/Users/GetById?id=` | `titleName` |
| `GET api/Users/GetByParentDepartment` | her kullanıcıda `titleName` |
| `GET api/Users/GetDepartmentUsersLoginStatus` | her kullanıcıda `titleName` |
| `POST api/Users/Login`, `POST api/Users/WindowsLogin` | giriş yapan kullanıcının `titleName`'i |

```json
{
  "id": "…",
  "name": "…",
  "surname": "…",
  "departmentShortName": "BDHD",
  "titleName": "Sözleşmeli Bilişim Personeli",
  "isActive": true
}
```

- Alan sadece eklendi; mevcut alanların adı ve tipi değişmedi.
- Token'a unvan eklenmedi. Giriş sonrası gerekiyorsa Login / WindowsLogin cevabından saklanmalı.
- Unvana göre süzme ya da arama yok; yalnızca gösterim için.

## Boş unvan (null)

Unvanlar kaynak sistemin unvan tablosundan (ComTitle, 2026-10-13'te yüklendi) gelir. 5.803 aktif kullanıcının **5.784'ünde** unvan dolu. Kalan 19 kişide `titleName` **null** gelir: kaynakta unvanı hiç girilmemiş olanlar. Kaynakta silinmiş bir unvana bağlı kişilerde de unvan gelir.

- `null` gelince hiçbir şey gösterilmesin (boş bırakılsın); "Unvan tanımsız" gibi bir metin yazılmasın.
- Unvanlar kaynaktaki yazımıyla gelir; büyük harf kullanımı tutarsız olabilir (ör. "DAİMİ TEMSİLCİ YARDIMCISI" ile "Daire Başkanı"). Metin olduğu gibi gösterilsin, büyük/küçük harf dönüştürülmesin.
- Bazı unvanlar uzundur (153 karaktere kadar, ör. büyükelçi + koordinatörlük); listelerde taşmaya karşı kısaltma (ellipsis) ve tam metin için tooltip önerilir.

## Önerilen gösterim

- **Kullanıcı listesi (Kullanıcılar sayfası):** ad soyadın altında ya da yanında küçük/ikincil metin olarak unvan; birim kısa adıyla aynı satırda olabilir: `Sözleşmeli Bilişim Personeli · BDHD`.
- **Kişi seçiciler (havale, zimmet, dağıtım alıcısı):** aynı adı taşıyan kişileri ayırt etmeye yarar; seçenek satırında ad soyadın yanında unvan.
- **Birim personeli / oturum durumu ekranı:** ayrı bir "Unvan" kolonu.
- **Üst bar / profil menüsü:** DISNET'teki gibi ad soyadın altında `BDHD, Sözleşmeli Bilişim Personeli`.

## Tip tanımı

```ts
export interface User {
  // …mevcut alanlar
  titleName: string | null;
}
```

`UserLoginResponse` (Login / WindowsLogin) ve birim personeli oturum durumu tipine de aynı alan eklenmeli.
