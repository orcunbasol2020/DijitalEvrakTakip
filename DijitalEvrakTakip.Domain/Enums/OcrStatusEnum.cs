using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Domain.Enums
{
    public enum OcrStatusEnum
    {
        [Description("Beklemede")]
        Wait = 0,

        [Description("Tamamlandı")]
        Done = 1,

        [Description("Hata")]
        Error = 2,

    }
}
