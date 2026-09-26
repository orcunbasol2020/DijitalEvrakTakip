using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAllAppSetting;

public sealed record GetAllAppSettingQuery()
    : IRequest<IList<AppSettingDto>>;
