using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum EnvelopeStatusEnum
{
    [Description("Yeni Kayıt")]
    YeniKayit = 1,

    [Description("Evrak Biriminde")]
    EvrakBiriminde = 2,

    [Description("Teslim Edildi")]
    TeslimEdildi = 3,

    [Description("Zimmet Devri")]
    ZimmetDevri = 4
}
