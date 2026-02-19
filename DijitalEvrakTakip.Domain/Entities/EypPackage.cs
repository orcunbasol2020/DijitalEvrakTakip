using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class EypPackage : Entity
    {
        public Guid? DocumentId { get; set; }

        public int Status { get; set; }

        public string? Path { get; set; }

        public string? Error { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }
    }
}
