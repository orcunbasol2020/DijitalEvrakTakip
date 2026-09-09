using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

/// <summary>
/// OutgoingDocumentTransaction.Type alanı için kullanılır.
/// 1-4 değerleri OutgoingDocumentStatusEnum ile aynı sırayı takip eder,
/// böylece bir transaction evrağın o andaki durumunu da temsil edebilir.
/// </summary>
public enum OutgoingTransactionTypeEnum
{
    [Description("Taslak")]
    Draft = 1,

    [Description("Gönderildi")]
    Sent = 2,

    [Description("Teslim Edildi")]
    Delivered = 3,

    [Description("İade")]
    Returned = 4,

    [Description("Güncelleme")]
    Update = 5
}
