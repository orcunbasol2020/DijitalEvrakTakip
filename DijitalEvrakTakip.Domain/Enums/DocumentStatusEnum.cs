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

        [Description("Teslim")]
        Teslim = 3,

        [Description("Eşleştirme")]
        Match = 4,

        [Description("Ocr")]
        Ocr = 5,

        [Description("Yayınla")]
        Publish = 6,

    }
}
