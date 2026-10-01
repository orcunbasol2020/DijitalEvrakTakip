namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class NotificationDto
{
    public Guid Id { get; set; }
    public int Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public Guid? IncomingDocumentId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
