using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum TransactionTypeEnum
{
    [Description("Ön Kayıt")]
    PreRegister = 1,

    [Description("Genel Kayıt")]
    GeneralRegister = 2,

    [Description("Güncelleme")]
    Update = 3,

    [Description("Teslim")]
    Delivery = 4,

    [Description("Detay Güncelleme")]
    UpdateDetail = 5,

    [Description("Zimmet")]
    Zimmet = 6,

    [Description("Teslim")]
    TeslimZimmet = 7,

    [Description("OCR")]
    Ocr = 8,

    [Description("Birim Arşivinde")]
    Archive = 9
}
