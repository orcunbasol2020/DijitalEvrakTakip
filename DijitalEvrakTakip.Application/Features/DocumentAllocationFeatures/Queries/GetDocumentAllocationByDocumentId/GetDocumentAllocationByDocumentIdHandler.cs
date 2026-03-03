using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetByDocumentId;

public sealed class GetDocumentAllocationByDocumentIdHandler
    : IRequestHandler<GetDocumentAllocationByDocumentIdQuery, IList<DocumentAllocationDto>>
{
    private readonly IDocumentAllocationService _documentAllocationService;

    public GetDocumentAllocationByDocumentIdHandler(
        IDocumentAllocationService documentAllocationService)
    {
        _documentAllocationService = documentAllocationService;
    }

    public async Task<IList<DocumentAllocationDto>> Handle(
        GetDocumentAllocationByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var allocations = await _documentAllocationService
            .GetByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        return allocations;
    }
}