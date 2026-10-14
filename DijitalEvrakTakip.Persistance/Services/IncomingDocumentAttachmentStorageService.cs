using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentAttachmentStorageService : IIncomingDocumentAttachmentStorageService
{
    private readonly string _rootPath;

    public IncomingDocumentAttachmentStorageService(string rootPath)
    {
        _rootPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(rootPath));
    }

    public async Task<string> SaveAsync(Guid incomingDocumentId, Stream content, CancellationToken cancellationToken)
    {
        var storedFileName = Path.Combine(incomingDocumentId.ToString("N"), $"{Guid.NewGuid():N}.pdf");
        var fullPath = GetFullPath(storedFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var fileStream = File.Create(fullPath))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return storedFileName;
    }

    public string GetFullPath(string storedFileName)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, storedFileName));

        // Kayıtlı yol ek klasörünün dışına çıkamaz
        if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Geçersiz ek dosyası yolu.");

        return fullPath;
    }

    public bool Exists(string storedFileName)
        => File.Exists(GetFullPath(storedFileName));

    public void Delete(string storedFileName)
    {
        var fullPath = GetFullPath(storedFileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
