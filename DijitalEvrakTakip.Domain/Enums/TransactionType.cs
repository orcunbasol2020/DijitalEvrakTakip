using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Domain.Enums
{
    public enum TransactionTypeEnum
    {
        PreRegister = 1,   // Ön Kayıt
        GeneralRegister = 2, // Genel Kayıt
        Update = 3,        // Güncelleme / Evrak Tamamlandı
        Delivery = 4       // Teslim
    }
}
