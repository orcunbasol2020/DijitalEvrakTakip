using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Kurum içi kullanıcıya uygulama içinde gösterilen bildirim.
/// </summary>
public class Notification : Entity
{
    // Bildirimi alacak kurum içi kullanıcı (FK değildir)
    public Guid UserId { get; set; }

    // NotificationTypeEnum
    public int Type { get; set; }

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;

    // İlgili kayıt (ör. DocumentAllocationRequest Id'si); toplu bildirimlerde boş
    public Guid? RelatedEntityId { get; set; }

    // İlgili gelen evrak; toplu bildirimlerde boş
    public Guid? IncomingDocumentId { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
}
