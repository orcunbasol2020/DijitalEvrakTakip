using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class EnvelopeDocument : Entity
    {
        public Guid EnvelopeId { get; set; }

        public string QrCode { get; set; }

        public Envelope? Envelope { get; set; }
    }
}