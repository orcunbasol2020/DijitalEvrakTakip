namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Zimmet talebi onay / red / iptal sonucu. Result: AllocationRequestActionResultEnum (1 = Başarılı).
/// </summary>
public sealed class AllocationRequestActionResultDto
{
    public Guid RequestId { get; set; }
    public int Result { get; set; }
    public string Message { get; set; } = string.Empty;
}
