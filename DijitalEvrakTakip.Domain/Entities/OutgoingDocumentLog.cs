using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocumentLog : Entity
    {
        public Guid? OutgoingDocumentId { get; set; }

        public string? ChangeArea { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        // Navigation
        public OutgoingDocument? OutgoingDocument { get; set; }
    }
}
