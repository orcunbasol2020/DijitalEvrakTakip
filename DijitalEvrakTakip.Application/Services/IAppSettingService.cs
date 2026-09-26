using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IAppSettingService
{
    /// <summary>Tüm ayarları döner. Sonuç kısa süreli önbelleklenir.</summary>
    Task<IList<AppSetting>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Tek bir ayarın değerini döner; anahtar yoksa null.</summary>
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken);

    /// <summary>Anahtara ait değeri günceller ve önbelleği temizler.</summary>
    Task UpdateValueAsync(string key, string? value, Guid? updatedByUserId, CancellationToken cancellationToken);
}
