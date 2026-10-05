using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStats;

public sealed class GetAtlasTransferStatsHandler
    : IRequestHandler<GetAtlasTransferStatsQuery, AtlasTransferStatsDto>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferStatsHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<AtlasTransferStatsDto> Handle(
        GetAtlasTransferStatsQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetStatsAsync(cancellationToken);
    }
}
