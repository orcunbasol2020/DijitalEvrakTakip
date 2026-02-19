using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocument : Entity
    {
        public string? QrCode { get; set; }

        public string? OriginalDocumentNumber { get; set; }

        public string? SecurityDegree { get; set; }

        public int? Type { get; set; }

        public int? LanguageId { get; set; }

        public string? Subject { get; set; }

        public string? Content_Ocr { get; set; }

        public string? DepartmentId { get; set; }

        public Guid? ExternalInstitutonId { get; set; }

        public bool? ElectronicCopy { get; set; }

        public bool? EbysTransfer { get; set; }

        public int? PageCount { get; set; }

        public string? Notes { get; set; }

        public DateTime? DocumentDate { get; set; }

        public string? CreatedUserId { get; set; }
    }
}
