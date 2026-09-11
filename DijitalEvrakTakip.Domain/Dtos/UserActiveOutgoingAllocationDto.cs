namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Bir kullanıcı üzerinde aktif (IsActive = true) olan zimmetli giden evrakları temsil eder.
/// </summary>
public sealed class UserActiveOutgoingAllocationDto
{
    public Guid AllocationId { get; set; }
    public Guid OutgoingDocumentId { get; set; }
    public string? QrCode { get; set; }
    public string? OriginalDocumentNumber { get; set; }
    public string? Subject { get; set; }
    public DateTime? DocumentDate { get; set; }
    public int Status { get; set; }
    public int Source { get; set; }
    public DateTime AllocatedDate { get; set; }
}
