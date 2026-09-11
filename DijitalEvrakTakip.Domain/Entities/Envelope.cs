using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class Envelope : Entity
    {
        public string? EnvelopeNo { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? ExternalInstitutionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? UnitName { get; set; }

        public string? Address { get; set; }

        public bool IsClosed { get; set; }

        // EnvelopeStatusEnum: Created / Delivered
        public int Status { get; set; }

        public ICollection<EnvelopeDocument>? EnvelopeDocuments { get; set; }
    }
}