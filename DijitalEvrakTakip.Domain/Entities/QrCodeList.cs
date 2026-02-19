using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class QrCodeList : Entity
    {
        public string? QrCode { get; set; }

        public Guid? DocumentId { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }
    }
}
