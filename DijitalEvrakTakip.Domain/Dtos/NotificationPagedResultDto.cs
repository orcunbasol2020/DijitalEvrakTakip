namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Sayfalanmış bildirim listesi. UnreadCount arama/tür filtresinden bağımsızdır;
/// kullanıcının zil rozetinde gösterilecek okunmamış bildirim sayısıdır.
/// </summary>
public sealed class NotificationPagedResultDto
{
    public IList<NotificationDto> Items { get; set; } = new List<NotificationDto>();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
