using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class EnvelopeDocument : Entity
    {
        public Guid EnvelopeId { get; set; }

        // Okutulan evrak numarası (ham QR değeri)
        public string QrCode { get; set; }

        // OutgoingDocuments tablosundaki karşılık gelen kaydın Id'si (FK değildir)
        public Guid? DocumentId { get; set; }

        public Envelope? Envelope { get; set; }
    }
}