using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AllocationSourceEnum
{
    [Description("Evrak Takip")]
    EvrakTakip = 1,

    [Description("Atlas EBYS")]
    AtlasEbys = 2
}
