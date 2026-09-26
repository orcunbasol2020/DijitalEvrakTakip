using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class AppSettingService : IAppSettingService
{
    private const string CacheKey = "AppSettings:All";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public AppSettingService(
        IAppSettingRepository appSettingRepository,
        IUnitOfWork unitOfWork,
        IMemoryCache cache)
    {
        _appSettingRepository = appSettingRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<IList<AppSetting>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out IList<AppSetting>? cached) && cached is not null)
            return cached;

        var settings = await _appSettingRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Key)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKey, (IList<AppSetting>)settings, CacheDuration);

        return settings;
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken)
    {
        var settings = await GetAllAsync(cancellationToken);

        return settings
            .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public async Task UpdateValueAsync(
        string key,
        string? value,
        Guid? updatedByUserId,
        CancellationToken cancellationToken)
    {
        var entity = await _appSettingRepository
            .GetByExpressionAsync(x => x.Key == key && !x.IsDeleted, cancellationToken);

        if (entity is null)
            throw new Exception($"'{key}' anahtarlı ayar bulunamadı.");

        entity.Value = value;
        entity.UpdatedByUserId = updatedByUserId;
        entity.UpdateDate = DateTime.UtcNow;

        _appSettingRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cache.Remove(CacheKey);
    }
}
