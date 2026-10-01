using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAppInfo;

/// <summary>
/// Versiyon/build bilgisini assembly'den, destek bilgilerini AppSettings tablosundan
/// alıp tek bir DTO'da birleştirir.
/// </summary>
public sealed class GetAppInfoHandler
    : IRequestHandler<GetAppInfoQuery, AppInfoDto>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IAppVersionProvider _appVersionProvider;

    public GetAppInfoHandler(
        IAppSettingService appSettingService,
        IAppVersionProvider appVersionProvider)
    {
        _appSettingService = appSettingService;
        _appVersionProvider = appVersionProvider;
    }

    public async Task<AppInfoDto> Handle(
        GetAppInfoQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _appSettingService.GetAllAsync(cancellationToken);

        var byKey = settings.ToDictionary(
            x => x.Key,
            x => x.Value,
            StringComparer.OrdinalIgnoreCase);

        return new AppInfoDto(
            _appVersionProvider.Version,
            _appVersionProvider.BuildDate,
            byKey.GetValueOrDefault(AppSettingKeys.ApplicationName),
            byKey.GetValueOrDefault(AppSettingKeys.SupportEmail),
            byKey.GetValueOrDefault(AppSettingKeys.SupportPhone),
            byKey.GetValueOrDefault(AppSettingKeys.AnnouncementMessage)
        );
    }
}
