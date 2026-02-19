using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocumentTransaction : Entity
    {
        public Guid? OutgoingDocumentId { get; set; }

        public int? Type { get; set; }

        public string? CargoPostNumber { get; set; }

        public string? UserId { get; set; }

        // Navigation
        public OutgoingDocument? OutgoingDocument { get; set; }
    }
}
