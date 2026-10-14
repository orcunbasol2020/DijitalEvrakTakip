using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferDocument;

public sealed class GetAtlasTransferDocumentHandler
    : IRequestHandler<GetAtlasTransferDocumentQuery, AtlasTransferDocumentDto>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferDocumentHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<AtlasTransferDocumentDto> Handle(
        GetAtlasTransferDocumentQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetDocumentAsync(request.DocumentId, cancellationToken);
    }
}
