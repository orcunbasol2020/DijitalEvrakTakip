namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Bir kullanıcı üzerinde aktif zimmetli olan gelen ve giden evrakları tek satır tipinde temsil eder.
/// DocumentDirection: DocumentDirectionEnum (1 = Gelen, 2 = Giden).
/// Gelen evrakta FromName = dış kurum, ToName = birim; giden evrakta FromName = birim, ToName = dış kurum.
/// </summary>
public sealed class UserActiveDocumentDto
{
    public Guid AllocationId { get; set; }
    public Guid DocumentId { get; set; }
    public int DocumentDirection { get; set; }
    public string? QrCode { get; set; }
    public string? DocumentNo { get; set; }
    public string? DocumentName { get; set; }
    public DateTime? DocumentDate { get; set; }
    public string? FromName { get; set; }
    public string? ToName { get; set; }
    public int Status { get; set; }
    public int Source { get; set; }
    public DateTime AllocatedDate { get; set; }
}
