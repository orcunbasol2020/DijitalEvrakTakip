using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums
{
    public enum DocumentDirectionEnum
    {
        [Description("Gelen")]
        Incoming = 1,

        [Description("Giden")]
        Outgoing = 2,

    }
}
