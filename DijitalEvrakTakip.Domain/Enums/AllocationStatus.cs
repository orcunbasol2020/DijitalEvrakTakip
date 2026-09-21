using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum AllocationStatusEnum
{
    [Description("Ön Kayıt")]
    OnKayit = 1,

    [Description("Devir")]
    Devir = 2,

    [Description("Teslim")]
    Teslim = 3,

    [Description("Arşiv")]
    Arsiv = 4
}
