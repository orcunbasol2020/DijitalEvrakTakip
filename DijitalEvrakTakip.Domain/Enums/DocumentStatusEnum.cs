using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Domain.Enums
{
    public enum DocumentStatusEnum
    {
        [Description("Ön Kayıt")]
        OnKayit = 1,

        [Description("Güncelleme")]
        Update = 2,

        // Aktif zimmet Teslim (AllocationStatusEnum.Teslim) veya Teslim Alındı (AllocationStatusEnum.TeslimAlindi) olunca atanır
        [Description("Teslim Edildi")]
        Teslim = 3,

        [Description("Eşleştirme")]
        Match = 4,

        [Description("Ocr")]
        Ocr = 5,

        // Status'a yazılmaz; güncellemede gelirse SubmissionStatus = PublishStatusEnum.Kuyrukta yapılır
        [Description("Yayınla")]
        Publish = 6,

    }
}
