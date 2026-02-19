using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocumentShelf : Entity
    {
        public Guid DocumentId { get; set; }

        public string? DepartmentId { get; set; }

        public string ShelfNo { get; set; }

        public bool IsActive { get; set; }

        // Navigation
        public IncomingDocument IncomingDocument { get; set; }
    }
}
