using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Queries.GetAtlasDocumentNumberPoolStock;

public sealed class GetAtlasDocumentNumberPoolStockHandler
    : IRequestHandler<GetAtlasDocumentNumberPoolStockQuery, AtlasDocumentNumberPoolStockDto>
{
    private readonly IAtlasDocumentNumberPoolService _poolService;

    public GetAtlasDocumentNumberPoolStockHandler(IAtlasDocumentNumberPoolService poolService)
    {
        _poolService = poolService;
    }

    public Task<AtlasDocumentNumberPoolStockDto> Handle(
        GetAtlasDocumentNumberPoolStockQuery request,
        CancellationToken cancellationToken)
    {
        return _poolService.GetStockAsync(cancellationToken);
    }
}
