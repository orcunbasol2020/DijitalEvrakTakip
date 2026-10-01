using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum SecurityDegreeEnum
{
    [Description("Tasnif Dışı")]
    Unclassified = 1,

    [Description("Özel")]
    Special = 2,

    [Description("Hizmete Özel")]
    ServiceUseOnly = 3,

    [Description("Kişiye Özel")]
    PersonalUseOnly = 4,

    [Description("Gizli")]
    Confidential = 5,

    [Description("Çok Gizli")]
    TopSecret = 6
}
