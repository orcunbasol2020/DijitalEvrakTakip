using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocumentEntry : Entity
    {
        public Guid? DocumentId { get; set; }

        public Guid? ExternalInstitutionPersonnelId { get; set; }

        public string? CreateUserId { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }

        public ExternalInstitutionPersonnel? ExternalInstitutionPersonnel { get; set; }
    }
}
