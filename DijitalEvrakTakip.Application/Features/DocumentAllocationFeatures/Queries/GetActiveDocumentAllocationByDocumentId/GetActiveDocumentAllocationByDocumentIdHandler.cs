using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveByDocumentId;

public sealed class GetActiveDocumentAllocationByDocumentIdHandler
    : IRequestHandler<GetActiveDocumentAllocationByDocumentIdQuery, DocumentAllocationDto>
{
    private readonly IDocumentAllocationService _documentAllocationService;

    public GetActiveDocumentAllocationByDocumentIdHandler(
        IDocumentAllocationService documentAllocationService)
    {
        _documentAllocationService = documentAllocationService;
    }

    public async Task<DocumentAllocationDto> Handle(
        GetActiveDocumentAllocationByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _documentAllocationService
            .GetActiveDtoByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);
    }
}