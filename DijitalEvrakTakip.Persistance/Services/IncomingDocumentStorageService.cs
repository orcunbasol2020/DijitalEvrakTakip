using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentStorageService : IIncomingDocumentStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf"
    };

    private readonly string _rootPath;

    public IncomingDocumentStorageService(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task<string> SaveAsync(
        string qrCode,
        string originalFileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(originalFileName);

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Desteklenmeyen dosya türü. Yalnızca PDF yüklenebilir.");

        var safeQrCode = string.Concat(qrCode.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        var fileName = $"{safeQrCode}_{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = GetFullPath(fileName);

        Directory.CreateDirectory(_rootPath);

        await using (var fileStream = File.Create(fullPath))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return fileName;
    }

    public string GetFullPath(string fileName)
        => Path.Combine(_rootPath, Path.GetFileName(fileName));

    public bool Exists(string fileName)
        => File.Exists(GetFullPath(fileName));

    public void Delete(string fileName)
    {
        var fullPath = GetFullPath(fileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
