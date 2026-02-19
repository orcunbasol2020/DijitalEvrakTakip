using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocumentAttachment : Entity
    {
        public Guid DocumentId { get; set; }

        public string? DocumentName { get; set; }

        public int PageCount { get; set; }

        public bool Ocr { get; set; }

        public int? OcrStatus { get; set; }

        public string? UserId { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }
    }
}
