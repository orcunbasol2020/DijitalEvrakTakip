using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocument : Entity
    {
        // -----------------------------
        // Basic Info
        // -----------------------------

        public string? OrginalNo { get; set; }

        public string QrCode { get; set; } = null!;   // Ön kayıt için zorunlu

        public string? DocumentName { get; set; }

        public string? Subject { get; set; }

        public string? Notes { get; set; }

        public string? Content_Ocr { get; set; }

        // -----------------------------
        // Enums / Status Fields
        // -----------------------------

        public int? SecurityDegree { get; set; }

        public int? DocumentTypeId { get; set; }

        public int? LanguageId { get; set; }

        public int? Status { get; set; }

        public int? SubmissionStatus { get; set; }

        public int? OcrStatus { get; set; }

        // -----------------------------
        // Document Details
        // -----------------------------

        public bool? ElectronicCopy { get; set; }

        public bool? Release { get; set; }

        public int? PageCount { get; set; }

        public DateTime? DocumentDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        // -----------------------------
        // Foreign Keys (NULLABLE - Ön kayıt için)
        // -----------------------------

        public Guid? ExternalInstitutionId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // -----------------------------
        // User (Identity)
        // -----------------------------

        public string UserId { get; set; } = null!;

        // -----------------------------
        // Audit
        // -----------------------------

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
