using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferQueue;

public sealed class GetAtlasTransferQueueHandler
    : IRequestHandler<GetAtlasTransferQueueQuery, IReadOnlyList<AtlasTransferQueueItemDto>>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferQueueHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<IReadOnlyList<AtlasTransferQueueItemDto>> Handle(
        GetAtlasTransferQueueQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetQueueAsync(cancellationToken);
    }
}
