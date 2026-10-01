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
    Archive = 9,

    [Description("Yayınla")]
    Yayinla = 10,

    [Description("Dosya Yükleme")]
    FileUpload = 11,

    [Description("Teslim Alındı")]
    TeslimAlindi = 12,

    [Description("Zimmet Onay Talebi")]
    ZimmetTalebi = 13,

    [Description("Devir Alındı")]
    DevirAlindi = 14,

    [Description("Zimmet Talebi Reddedildi")]
    ZimmetTalebiReddedildi = 15,

    [Description("Zimmet Talebi İptal Edildi")]
    ZimmetTalebiIptal = 16
}
