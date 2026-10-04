using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AtlasDocumentNumberStatusEnum
{
    [Description("Boşta")]
    Available = 1,

    [Description("Ayrıldı")]
    Reserved = 2,

    [Description("Kullanıldı")]
    Used = 3,

    [Description("İptal")]
    Cancelled = 4
}
