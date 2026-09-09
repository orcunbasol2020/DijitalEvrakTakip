using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class IncomingDocument : Entity
    {
        public string? OrginalNo { get; set; }

        public string QrCode { get; set; } = null!;

        public string? DocumentName { get; set; }

        public string? Subject { get; set; }

        public string? Notes { get; set; }

        public string? Content_Ocr { get; set; }

        public int? SecurityDegree { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? DocumentDirection { get; set; }
        public int? LanguageId { get; set; }
        public int? Status { get; set; }
        public int? SubmissionStatus { get; set; }
        public int? OcrStatus { get; set; }

        public bool? ElectronicCopy { get; set; }
        public bool? Release { get; set; }
        public int? PageCount { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public Guid? ExternalInstitutionId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string UserId { get; set; } = null!;
        public string? CurrentAssignmentUser { get; set; }
        public Guid? CurrentAssignmentUserId { get; set; }

        public ICollection<DocumentAllocation> DocumentAllocations { get; set; }
            = new List<DocumentAllocation>();

        public ICollection<DocumentAssignment> DocumentAssignments { get; set; }
            = new List<DocumentAssignment>();
    }
}
