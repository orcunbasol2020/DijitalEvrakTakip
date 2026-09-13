using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class OutgoingDocumentAllocation : Entity
    {
        // Zimmetlenen evrağın Id'si (FK değildir)
        public Guid OutgoingDocumentId { get; set; }

        // Zimmetlenen kişinin Id'si (User veya ExternalUser tablosuna ait olabilir, FK değildir)
        public Guid UserId { get; set; }

        // Zimmetlenen kişinin iç mi dış kurum personeli mi olduğu (AllocationUserTypeEnum: Internal / External)
        public int UserType { get; set; }

        // Status
        public int Status { get; set; }

        // Zimmetin hangi sistemde yapıldığı (AllocationSourceEnum: EvrakTakip / AtlasEbys)
        public int Source { get; set; }

        // Aynı evrakta tek aktif zimmet için
        public bool IsActive { get; set; } = true;
        public Guid? CreatedUserId { get; set; }

        // Zimmet sonrası yüklenen ıslak imzalı belgenin diske göreli yolu
        public string? WetSignedDocumentPath { get; set; }

        // Yüklenen dosyanın orijinal adı (indirirken kullanılır)
        public string? WetSignedDocumentFileName { get; set; }

        public DateTime? WetSignedDocumentUploadDate { get; set; }

        // Belgeyi yükleyen kişinin Id'si (User veya ExternalUser tablosuna ait olabilir, FK değildir)
        public Guid? WetSignedDocumentUploadedUserId { get; set; }
    }
}
