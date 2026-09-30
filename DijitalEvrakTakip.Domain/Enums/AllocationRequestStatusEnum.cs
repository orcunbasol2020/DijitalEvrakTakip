using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AllocationRequestStatusEnum
{
    [Description("Onay Bekliyor")]
    Beklemede = 1,

    [Description("Onaylandı")]
    Onaylandi = 2,

    [Description("Reddedildi")]
    Reddedildi = 3,

    // Devreden veya işlemi yapan kullanıcı talebi geri çekti
    [Description("İptal Edildi")]
    IptalEdildi = 4,

    // Talep beklerken evrağın aktif zimmeti başka bir işlemle değişti
    [Description("Geçersiz")]
    Gecersiz = 5
}
