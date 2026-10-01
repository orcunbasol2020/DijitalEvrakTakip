using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class WetSignedDocumentStorageService : IWetSignedDocumentStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff"
    };

    private readonly string _rootPath;

    public WetSignedDocumentStorageService(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task<string> SaveAsync(
        Guid outgoingDocumentId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Desteklenmeyen dosya türü.");

        var relativePath = Path.Combine(
            outgoingDocumentId.ToString(),
            $"{Guid.NewGuid()}{extension}");

        var fullPath = Path.Combine(_rootPath, relativePath);
        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using (var fileStream = File.Create(fullPath))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return relativePath.Replace('\\', '/');
    }

    public async Task<(byte[] Content, string ContentType)?> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(_rootPath, relativePath);

        if (!File.Exists(fullPath))
            return null;

        var content = await File.ReadAllBytesAsync(fullPath, cancellationToken);
        var contentType = GetContentType(Path.GetExtension(fullPath));

        return (content, contentType);
    }

    public void Delete(string relativePath)
    {
        var fullPath = Path.Combine(_rootPath, relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".tif" or ".tiff" => "image/tiff",
        _ => "application/octet-stream"
    };
}
