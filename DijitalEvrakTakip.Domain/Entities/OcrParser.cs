using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OcrParser : Entity
    {
        public Guid? ScannedDocumentListId { get; set; }

        public string? FilePath { get; set; }

        public int? Status { get; set; }

        public int? LanguageId { get; set; }

        public int? TryCount { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
