namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Bir kullanıcı üzerinde aktif (IsActive = true) olan zimmetli evrakları temsil eder.
/// </summary>
public sealed class UserActiveAllocationDto
{
    public Guid AllocationId { get; set; }
    public Guid IncomingDocumentId { get; set; }
    public string? OrginalNo { get; set; }
    public string? QrCode { get; set; }
    public string? DocumentName { get; set; }
    public string? Subject { get; set; }
    public DateTime? DocumentDate { get; set; }
    public int Status { get; set; }
    public int Source { get; set; }
    public DateTime AllocatedDate { get; set; }
}
