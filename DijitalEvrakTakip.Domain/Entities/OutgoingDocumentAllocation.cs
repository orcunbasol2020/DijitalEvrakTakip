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
    }
}
