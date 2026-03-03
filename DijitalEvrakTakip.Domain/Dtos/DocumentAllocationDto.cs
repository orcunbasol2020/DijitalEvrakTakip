namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class DocumentAllocationDto
{
    public Guid Id { get; set; }
    public Guid IncomingDocumentId { get; set; }
    public string UserId { get; set; } = default!;
    public string FullName { get; set; } = default!; 

    public int Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public bool IsPreRegistered => Status == 1;
    public bool IsAllocated => Status == 2;
}
