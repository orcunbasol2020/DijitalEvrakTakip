namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class OutgoingDocumentAllocationDto
{
    public Guid Id { get; set; }
    public Guid OutgoingDocumentId { get; set; }
    public string UserId { get; set; } = default!;
    public int UserType { get; set; }
    public string FullName { get; set; } = default!;
    public string? CreatedFullName { get; set; }

    public int Status { get; set; }
    public int Source { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public string? WetSignedDocumentFileName { get; set; }
    public DateTime? WetSignedDocumentUploadDate { get; set; }
    public string? WetSignedDocumentUploadedByFullName { get; set; }

    public bool IsPreRegistered => Status == 1;
    public bool IsAllocated => Status == 2;
    public bool HasWetSignedDocument => !string.IsNullOrEmpty(WetSignedDocumentFileName);
}
