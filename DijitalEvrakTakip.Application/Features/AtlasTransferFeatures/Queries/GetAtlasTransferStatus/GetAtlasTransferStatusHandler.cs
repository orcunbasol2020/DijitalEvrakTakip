using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStatus;

public sealed class GetAtlasTransferStatusHandler
    : IRequestHandler<GetAtlasTransferStatusQuery, AtlasTransferStatusDto>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferStatusHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<AtlasTransferStatusDto> Handle(
        GetAtlasTransferStatusQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetStatusAsync(cancellationToken);
    }
}
