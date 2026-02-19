using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class ScannedDocument : Entity
    {
        public string? DocumentName { get; set; }

        public int? OcrStatusId { get; set; }
    }
}
