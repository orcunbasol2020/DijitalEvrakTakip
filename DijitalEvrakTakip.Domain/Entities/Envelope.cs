using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class Envelope : Entity
    {
        public string? EnvelopeNo { get; set; }

        public Guid CreatedByUserId { get; set; }

        public int? ExternalInstitutionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? TargetDepartmentId { get; set; }
        public string? UnitName { get; set; }

        public string? Address { get; set; }

        public bool IsClosed { get; set; }

        // EnvelopeStatusEnum: YeniKayit / EvrakBiriminde / TeslimEdildi / ZimmetDevri / KargoyaVerildi
        public int Status { get; set; }

        public ICollection<EnvelopeDocument>? EnvelopeDocuments { get; set; }
    }
}