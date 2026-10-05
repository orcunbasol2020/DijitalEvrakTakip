using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class EypPackageStorageService : IEypPackageStorageService
{
    private readonly string _rootPath;

    public EypPackageStorageService(string rootPath)
    {
        _rootPath = rootPath;
    }

    // Kayıtlı ad: <evrak no>_<paket id>.eyp; evrak numarasındaki "/" gibi geçersiz karakterler atılır
    public async Task<string> SaveAsync(string qrCode, Guid packageId, byte[] content, CancellationToken cancellationToken)
    {
        var safeQrCode = string.Concat(qrCode.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        var fileName = $"{safeQrCode}_{packageId:N}.eyp".TrimStart('_');

        Directory.CreateDirectory(_rootPath);
        await File.WriteAllBytesAsync(GetFullPath(fileName), content, cancellationToken);

        return fileName;
    }

    public Task<byte[]> ReadAsync(string fileName, CancellationToken cancellationToken)
        => File.ReadAllBytesAsync(GetFullPath(fileName), cancellationToken);

    public bool Exists(string fileName)
        => File.Exists(GetFullPath(fileName));

    private string GetFullPath(string fileName)
        => Path.Combine(_rootPath, Path.GetFileName(fileName));
}
