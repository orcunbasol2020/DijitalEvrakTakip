using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAllAppSetting;

public sealed class GetAllAppSettingHandler
    : IRequestHandler<GetAllAppSettingQuery, IList<AppSettingDto>>
{
    private readonly IAppSettingService _appSettingService;

    public GetAllAppSettingHandler(IAppSettingService appSettingService)
    {
        _appSettingService = appSettingService;
    }

    public async Task<IList<AppSettingDto>> Handle(
        GetAllAppSettingQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _appSettingService.GetAllAsync(cancellationToken);

        return settings
            .Select(x => new AppSettingDto(
                x.Id,
                x.Key,
                x.Value,
                x.Description,
                x.UpdatedByUserId,
                x.UpdateDate
            ))
            .ToList();
    }
}
