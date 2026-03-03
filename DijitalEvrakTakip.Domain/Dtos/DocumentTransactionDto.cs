namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class DocumentTransactionDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }

    public int? TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = "-";
    public string UserFullName { get; set; } = "-";

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }

    public string CreatedUserFullName { get; set; } = "-";
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
