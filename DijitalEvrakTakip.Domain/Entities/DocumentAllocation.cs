using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class DocumentAllocation : Entity
    {
        // Foreign Keys
        public Guid IncomingDocumentId { get; set; }
        public Guid UserId { get; set; } 

        // Status
        public int Status { get; set; }

        // Aynı evrakta tek aktif zimmet için
        public bool IsActive { get; set; } = true;
        public Guid? CreatedUserId { get; set; }

        // Navigation
        public IncomingDocument IncomingDocument { get; set; } = null!;
        public User User { get; set; } = null!; 
    }
}