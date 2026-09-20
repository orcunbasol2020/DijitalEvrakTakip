using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocument : Entity
    {
        public string? QrCode { get; set; }

        public string? OriginalDocumentNumber { get; set; }

        // SecurityDegreeEnum: Tasnif Dışı / Özel / Hizmete Özel / Kişiye Özel / Gizli / Çok Gizli
        public int? SecurityDegree { get; set; }

        // UrgencyDegreeEnum: Normal / Acele / Çok Acele / Yıldırım / Günlüdür / İvedi Süreli
        public int? UrgencyDegree { get; set; }

        public int? Type { get; set; }

        public Guid? LanguageId { get; set; }

        public string? Subject { get; set; }

        public string? Content_Ocr { get; set; }

        public int? Status { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Guid? ExternalInstitutonId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        public bool? ElectronicCopy { get; set; }

        public bool? EbysTransfer { get; set; }

        // Bilgi/Gereği: true = Gereği, false = Bilgi
        public bool? ActionRequired { get; set; }

        public int? PageCount { get; set; }

        public string? Notes { get; set; }

        public DateTime? DocumentDate { get; set; }

        public string? CreatedUserId { get; set; }

        // Evrağın hangi sistemde oluşturulduğu (AllocationSourceEnum: EvrakTakip / AtlasEbys)
        public int Source { get; set; }
    }
}
