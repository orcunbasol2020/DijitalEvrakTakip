namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Bir kullanıcının zimmet devri yaptığı (CreatedUserId olarak oluşturduğu) evrak sayısını temsil eder.
/// </summary>
public sealed class UserAllocationTransferCountDto
{
    public Guid UserId { get; set; }
    public int TransferCount { get; set; }
}
