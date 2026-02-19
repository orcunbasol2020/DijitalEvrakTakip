using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocumentStatus : Entity
    {
        public Guid IncomingDocumentId { get; set; }

        public int StatusId { get; set; }

        public bool IsActive { get; set; }

        public string UserId { get; set; }

        // Navigation
        public IncomingDocument IncomingDocument { get; set; }
    }
}
