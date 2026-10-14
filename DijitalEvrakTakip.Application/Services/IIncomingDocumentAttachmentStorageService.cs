namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Gelen evrak eklerinin tutulduğu klasöre erişim sağlar. Ekler, tarama/OCR akışının izlediği
/// evrak klasöründen ayrı tutulur; her evrakın ekleri kendi alt klasöründedir.
/// </summary>
public interface IIncomingDocumentAttachmentStorageService
{
    /// <summary>
    /// PDF içeriğini evrakın alt klasörüne kaydeder ve ek klasörüne göre yolunu ({evrakId}/{dosya}.pdf) döner.
    /// </summary>
    Task<string> SaveAsync(Guid incomingDocumentId, Stream content, CancellationToken cancellationToken);

    string GetFullPath(string storedFileName);

    bool Exists(string storedFileName);

    void Delete(string storedFileName);
}
