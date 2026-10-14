using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetFailedAtlasTransfers;

public sealed class GetFailedAtlasTransfersHandler
    : IRequestHandler<GetFailedAtlasTransfersQuery, IReadOnlyList<AtlasTransferFailedDocumentDto>>
{
    private readonly IAtlasTransferService _transferService;

    public GetFailedAtlasTransfersHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<IReadOnlyList<AtlasTransferFailedDocumentDto>> Handle(
        GetFailedAtlasTransfersQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetFailedAsync(cancellationToken);
    }
}
