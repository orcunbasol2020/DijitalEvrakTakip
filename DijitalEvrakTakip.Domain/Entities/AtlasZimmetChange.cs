using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Atlas EBYS sisteminden gelen zimmet değişikliği bildirimlerinin (webhook)
    /// ham ve eşleştirilmiş halini tutan gelen kutusu (inbox) tablosu.
    /// </summary>
    public class AtlasZimmetChange : Entity
    {
        // Atlas tarafındaki benzersiz zimmet hareketi kimliği (tekrar bildirimleri ayıklamak için)
        public string AtlasZimmetId { get; set; } = null!;

        // Atlas'taki evrak referansı (bizim OrginalNo/QrCode alanlarımızla eşleştirilir)
        public string AtlasDocumentNo { get; set; } = null!;
        public string? QrCode { get; set; }

        public string? FromUserSicilNo { get; set; }
        public string? FromUserName { get; set; }
        public string? FromUnitName { get; set; }

        public string? ToUserSicilNo { get; set; }
        public string? ToUserName { get; set; }
        public string? ToUnitName { get; set; }

        public string? ZimmetTuru { get; set; }
        public DateTime ZimmetTarihi { get; set; }
        public string? Description { get; set; }

        // Atlas'tan gelen ham JSON payload (denetim/hata ayıklama amaçlı)
        public string RawPayload { get; set; } = null!;

        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? ProcessedDate { get; set; }

        // Eşleşen yerel evrak (bulunabilirse)
        public Guid? IncomingDocumentId { get; set; }
        public IncomingDocument? IncomingDocument { get; set; }
    }
}
