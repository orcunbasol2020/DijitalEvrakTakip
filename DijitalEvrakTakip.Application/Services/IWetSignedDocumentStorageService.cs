namespace DijitalEvrakTakip.Application.Services;

public interface IWetSignedDocumentStorageService
{
    Task<string> SaveAsync(
        Guid outgoingDocumentId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken);

    Task<(byte[] Content, string ContentType)?> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken);

    void Delete(string relativePath);
}
