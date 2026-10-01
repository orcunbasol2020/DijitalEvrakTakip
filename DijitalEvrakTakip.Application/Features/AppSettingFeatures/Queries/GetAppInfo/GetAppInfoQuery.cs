using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAppInfo;

public sealed record GetAppInfoQuery()
    : IRequest<AppInfoDto>;
