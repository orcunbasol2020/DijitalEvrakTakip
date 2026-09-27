using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

/// <summary>
/// OutgoingDocumentShipment.CargoCompany alanı için kullanılır.
/// </summary>
public enum CargoCompanyEnum
{
    [Description("PTT Kargo")]
    Ptt = 1,

    [Description("Aras Kargo")]
    Aras = 2,

    [Description("Yurtiçi Kargo")]
    Yurtici = 3,

    [Description("MNG Kargo")]
    Mng = 4,

    [Description("Sürat Kargo")]
    Surat = 5,

    [Description("Kurye")]
    Kurye = 6,

    [Description("Diğer")]
    Diger = 99
}
