using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveByDocumentId;

public sealed class GetActiveOutgoingDocumentAllocationByDocumentIdHandler
    : IRequestHandler<GetActiveOutgoingDocumentAllocationByDocumentIdQuery, OutgoingDocumentAllocationDto>
{
    private readonly IOutgoingDocumentAllocationService _outgoingDocumentAllocationService;

    public GetActiveOutgoingDocumentAllocationByDocumentIdHandler(
        IOutgoingDocumentAllocationService outgoingDocumentAllocationService)
    {
        _outgoingDocumentAllocationService = outgoingDocumentAllocationService;
    }

    public async Task<OutgoingDocumentAllocationDto> Handle(
        GetActiveOutgoingDocumentAllocationByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _outgoingDocumentAllocationService
            .GetActiveDtoByDocumentIdAsync(request.OutgoingDocumentId, cancellationToken);
    }
}
