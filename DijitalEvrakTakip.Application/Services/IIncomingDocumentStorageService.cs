namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Gelen evrak dosyalarının (taranan veya elle yüklenen PDF) tutulduğu klasöre erişim sağlar.
/// </summary>
public interface IIncomingDocumentStorageService
{
    /// <summary>
    /// Dosyayı gelen evrak klasörüne kaydeder ve kaydedilen dosya adını (klasörsüz) döner.
    /// </summary>
    Task<string> SaveAsync(
        string qrCode,
        string originalFileName,
        Stream content,
        CancellationToken cancellationToken);

    string GetFullPath(string fileName);

    bool Exists(string fileName);

    void Delete(string fileName);
}
