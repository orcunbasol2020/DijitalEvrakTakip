using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class DocumentLog : Entity
    {
        public Guid? DocumentId { get; set; }

        public string? Area { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? UserId { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }
    }
}
