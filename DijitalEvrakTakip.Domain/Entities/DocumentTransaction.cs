using DijitalEvrakTakip.Domain.Abstractions;
using DijitalEvrakTakip.Domain.Entities;

public class DocumentTransaction : Entity
{
    public Guid DocumentId { get; set; }

    public int? TransactionType { get; set; }

    public string? UserId { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedUserId { get; set; }

    // Navigation
    public IncomingDocument IncomingDocument { get; set; }

}
