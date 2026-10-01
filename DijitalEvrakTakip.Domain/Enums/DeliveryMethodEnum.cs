using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

/// <summary>
/// OutgoingDocumentDistribution.DeliveryMethod alanı için kullanılır.
/// Alıcıya evrağın hangi yolla ulaştırıldığını belirtir.
/// </summary>
public enum DeliveryMethodEnum
{
    [Description("Elden")]
    Elden = 1,

    [Description("Kargo / Posta")]
    Kargo = 2,

    [Description("EBYS")]
    Ebys = 3
}
