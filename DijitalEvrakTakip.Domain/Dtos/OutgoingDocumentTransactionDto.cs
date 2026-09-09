namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class OutgoingDocumentTransactionDto
{
    public Guid Id { get; set; }
    public Guid? OutgoingDocumentId { get; set; }

    public int? Type { get; set; }
    public string TypeName { get; set; } = "-";

    public string? CargoPostNumber { get; set; }
    public string? UserId { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
