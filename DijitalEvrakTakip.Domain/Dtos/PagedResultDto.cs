namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Sayfalanmış liste sonucu. Sayfalama istenmediğinde tüm kayıtlar tek sayfada döner.
/// </summary>
public sealed class PagedResultDto<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
