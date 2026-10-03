using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum NotificationTypeEnum
{
    [Description("Zimmet Onay Talebi")]
    ZimmetOnayTalebi = 1,

    [Description("Zimmet Onay Hatırlatma")]
    ZimmetOnayHatirlatma = 2,

    [Description("Zimmet Onaylandı")]
    ZimmetOnaylandi = 3,

    [Description("Zimmet Reddedildi")]
    ZimmetReddedildi = 4,

    [Description("Zimmet Talebi İptal Edildi")]
    ZimmetTalebiIptal = 5,

    // Alıcı belirli sayıda hatırlatmaya rağmen onay vermediğinde devredene gider
    [Description("Zimmet Onayı Gecikiyor")]
    ZimmetOnayGecikme = 6,

    [Description("Zimmet Talebi Geçersiz")]
    ZimmetTalebiGecersiz = 7,

    // Alıcı zimmeti şerh koyarak kabul etti; devredene ve işlemi yapana gider
    [Description("Zimmet Şerhli Kabul Edildi")]
    ZimmetSerhliKabul = 8
}
