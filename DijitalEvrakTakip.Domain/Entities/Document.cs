using DijitalEvrakTakip.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Domain.Entities
{
    public sealed class Document : Entity
    {
        public string BelgeId { get; set; }
        public string BelgeOrjinalNo { get; set; }
        public int GizlilikDerecesi { get; set; }
        public int BelgeTuru { get; set; }
        public int BelgeDilId { get; set; }
        public string Konusu { get; set; }
        public string Nereden { get; set; }
        public string Nereye { get; set; }
        public int Durum { get; set; } // tarandı, ocr vb.
        public int TeslimDurum { get; set; }
        public bool ElektronikKopya { get; set; }
        public bool Yayinla { get; set; }
        public int SayfaSayisi { get; set; }
        public string Notlar { get; set; }
        public DateTime BelgeTarihi { get; set; }
        public DateTime YayinTarihi { get; set; }
        public string KullaniciId { get; set; }
        public string AtananPersonel { get; set; }
        public string qr { get; set; }
        public string TarananDosyaAdi { get; set; }

    }
}
