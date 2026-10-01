using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum OutgoingDocumentStatusEnum
{
    [Description("Taslak")]
    Draft = 1,

    [Description("Gönderildi")]
    Sent = 2,

    [Description("Teslim Edildi")]
    Delivered = 3,

    [Description("İade")]
    Returned = 4
}
