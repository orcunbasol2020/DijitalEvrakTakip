using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AllocationUserTypeEnum
{
    [Description("İç Kullanıcı")]
    Internal = 1,

    [Description("Dış Kurum Kullanıcısı")]
    External = 2
}
