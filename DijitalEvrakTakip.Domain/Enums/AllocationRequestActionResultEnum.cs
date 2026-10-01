namespace DijitalEvrakTakip.Domain.Enums;

// Zimmet talebi onay / red / iptal işleminin sonucu; önyüz mesaj metnine değil bu koda bakar
public enum AllocationRequestActionResultEnum
{
    Basarili = 1,

    // Talep beklerken evrağın zimmeti değişti; talep Geçersiz yapıldı
    Gecersiz = 2,

    // Talep aynı anda başka bir işlemle (onay / red / iptal) sonuçlandırıldı
    Cakisma = 3,

    Bulunamadi = 4,

    // Onay / red için alıcı, iptal için devreden veya işlemi yapan kullanıcı değil
    Yetkisiz = 5,

    // Talep zaten sonuçlanmış
    OnayBeklemiyor = 6,

    // UserId Guid değil
    GecersizKullanici = 7
}
