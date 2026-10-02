using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Atlas EBYS'den önceden alınmış evrak numarası (QR kod) havuzu.
    /// Numaralar arka planda Atlas'tan çekilir; kayıt sırasında Atlas'a gidilmez.
    /// </summary>
    public class AtlasDocumentNumber : Entity
    {
        // Atlas'ın verdiği evrak numarası; QR etikete basılan ve IncomingDocument.QrCode'a yazılan değer
        public string QrCode { get; set; } = null!;

        // AtlasDocumentNumberStatusEnum: Boşta / Ayrıldı / Kullanıldı / İptal
        public int Status { get; set; }

        // Atlas tarafındaki kayıt kimliği (Atlas veriyorsa; iptal/kullanım bildirimi için)
        public string? AtlasReferenceId { get; set; }

        // Etiket basmak için numarayı ayıran kullanıcı
        public Guid? ReservedByUserId { get; set; }
        public DateTime? ReservedAt { get; set; }

        // Numarayla açılan gelen evrak
        public Guid? IncomingDocumentId { get; set; }
        public IncomingDocument? IncomingDocument { get; set; }
        public DateTime? UsedAt { get; set; }

        public Guid? CancelledByUserId { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
    }
}
