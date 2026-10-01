using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

// IncomingDocument.SubmissionStatus alanında tutulur. Evrakın akış durumu (Status) ile ilgisi yoktur.
// Yayınla işlemi evrakı yalnızca aktarım sırasına alır; Atlas aktarımı başarılı olunca Release = true yapılır.
public enum PublishStatusEnum
{
    [Description("Yayınlanmadı")]
    Yayinlanmadi = 1,

    [Description("Aktarım Sırasında")]
    Kuyrukta = 2,

    [Description("Aktarılıyor")]
    Aktariliyor = 3,

    [Description("Yayınlandı")]
    Basarili = 4,

    [Description("Aktarım Hatalı")]
    Hatali = 5
}
