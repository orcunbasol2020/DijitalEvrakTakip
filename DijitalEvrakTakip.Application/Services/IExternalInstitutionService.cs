namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Dış kurumlar yalnızca okunur (vw_ExternalInstitutions: ComAgency); ekleme / güncelleme kaynak sistemde yapılır.
/// </summary>
public interface IExternalInstitutionService
{
    /// <summary>
    /// Silinmemiş kurumlar, ada göre. type verilirse yalnızca o türden; search adda geçen metin.
    /// take verilirse en fazla o kadar kayıt döner.
    /// </summary>
    Task<IList<ExternalInstitution>> GetAllAsync(
        int? type,
        string? search,
        int? take,
        CancellationToken cancellationToken);

    Task<ExternalInstitution?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Verilen kurumların adları, tek sorguda. Kaynakta silinmiş / pasif kurumlar da döner (eski evraklarda
    /// gönderen adı görünmeli); bulunamayan id sözlükte yer almaz.
    /// </summary>
    Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}
