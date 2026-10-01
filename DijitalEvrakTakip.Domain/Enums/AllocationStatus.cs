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
    Arsiv = 4,

    [Description("Teslim Alındı")]
    TeslimAlindi = 5,

    // Fiziksel evrak kargoya verildi; zimmet zinciri burada kapanır
    [Description("Kargoya Verildi")]
    KargoyaVerildi = 6,

    // Kurum içi Devir talebi alıcı tarafından onaylandı
    [Description("Devir Alındı")]
    DevirAlindi = 7
}
