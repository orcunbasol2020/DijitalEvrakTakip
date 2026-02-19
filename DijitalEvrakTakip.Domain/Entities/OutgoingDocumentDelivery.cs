using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocumentDelivery : Entity
    {
        public Guid? OutgoingDocumentId { get; set; }

        public Guid? ExternalInstitutionId { get; set; }

        public string? RecipientName { get; set; }

        public string? RecipientSurname { get; set; }

        public string? DelivererUserId { get; set; }

        public string? UserId { get; set; }

        public string? Notes { get; set; }
    }
}
