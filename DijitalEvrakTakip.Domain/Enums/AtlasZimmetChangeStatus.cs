using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AtlasZimmetChangeStatusEnum
{
    [Description("Beklemede")]
    Pending = 1,

    [Description("İşlendi")]
    Processed = 2,

    [Description("Evrak Eşleşmedi")]
    Unmatched = 3,

    [Description("Hata")]
    Failed = 4
}
