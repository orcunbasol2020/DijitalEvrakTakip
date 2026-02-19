using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class DocumentAllocation : Entity
    {
        public Guid DocumentId { get; set; }

        public string? UserId { get; set; }

        public int? Status { get; set; }

        public bool? IsActive { get; set; }

        // Navigation
        public IncomingDocument IncomingDocument { get; set; }
    }
}
