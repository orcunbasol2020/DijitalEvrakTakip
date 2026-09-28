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

        // SecurityDegreeEnum: Tasnif Dışı / Özel / Hizmete Özel / Kişiye Özel / Gizli / Çok Gizli
        public int? SecurityDegree { get; set; }
        // UrgencyDegreeEnum: Normal / Acele / Çok Acele / Yıldırım / Günlüdür / İvedi Süreli
        public int? UrgencyDegree { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? DocumentDirection { get; set; }
        public int? LanguageId { get; set; }
        // DocumentStatusEnum: evrakın akıştaki yeri (Ön Kayıt / Güncelleme / Teslim ...)
        public int? Status { get; set; }
        // PublishStatusEnum: Atlas'a yayın (aktarım) durumu
        public int? SubmissionStatus { get; set; }
        public int? OcrStatus { get; set; }

        public bool? ElectronicCopy { get; set; }
        public bool? Release { get; set; }
        // Bilgi/Gereği: true = Gereği, false = Bilgi
        public bool? ActionRequired { get; set; }
        public int? PageCount { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public Guid? ExternalInstitutionId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string UserId { get; set; } = null!;

        // Evrağı ilk kaydeden (ön kayıt / oluşturma) kullanıcı
        public string? CreatedUserId { get; set; }
        public string? CurrentAssignmentUser { get; set; }
        public Guid? CurrentAssignmentUserId { get; set; }

        public ICollection<DocumentAssignment> DocumentAssignments { get; set; }
            = new List<DocumentAssignment>();
    }
}
