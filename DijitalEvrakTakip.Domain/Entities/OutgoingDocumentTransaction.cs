using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocumentTransaction : Entity
    {
        public Guid? OutgoingDocumentId { get; set; }

        public int? Type { get; set; }

        // Kargoya verildi işlemi için ilgili kargo kaydı
        public Guid? ShipmentId { get; set; }

        public string? UserId { get; set; }

        // Navigation
        public OutgoingDocument? OutgoingDocument { get; set; }
    }
}
