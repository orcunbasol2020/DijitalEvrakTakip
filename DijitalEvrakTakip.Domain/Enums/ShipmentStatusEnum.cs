using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

/// <summary>
/// OutgoingDocumentShipment.Status alanı için kullanılır.
/// </summary>
public enum ShipmentStatusEnum
{
    [Description("Kargoya Verildi")]
    Shipped = 1,

    [Description("Yolda")]
    InTransit = 2,

    [Description("Teslim Edildi")]
    Delivered = 3,

    [Description("İade")]
    Returned = 4
}
