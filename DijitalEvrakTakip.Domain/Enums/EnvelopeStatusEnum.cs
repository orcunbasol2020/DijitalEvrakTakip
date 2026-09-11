using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum EnvelopeStatusEnum
{
    [Description("Oluşturuldu")]
    Created = 1,

    [Description("Teslim Edildi")]
    Delivered = 2
}
