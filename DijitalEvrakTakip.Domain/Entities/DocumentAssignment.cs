using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class DocumentAssignment : Entity
    {
        public Guid DocumentId { get; set; }

        public string? UserId { get; set; }

        public bool? Lock { get; set; }

        public bool? IsActive { get; set; }

        // Navigation
        public IncomingDocument IncomingDocument { get; set; }


    }
}
