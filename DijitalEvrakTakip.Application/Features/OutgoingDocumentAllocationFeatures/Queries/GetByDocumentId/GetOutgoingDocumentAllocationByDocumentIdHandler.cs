using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetByDocumentId;

public sealed class GetOutgoingDocumentAllocationByDocumentIdHandler
    : IRequestHandler<GetOutgoingDocumentAllocationByDocumentIdQuery, IList<OutgoingDocumentAllocationDto>>
{
    private readonly IOutgoingDocumentAllocationService _outgoingDocumentAllocationService;

    public GetOutgoingDocumentAllocationByDocumentIdHandler(
        IOutgoingDocumentAllocationService outgoingDocumentAllocationService)
    {
        _outgoingDocumentAllocationService = outgoingDocumentAllocationService;
    }

    public async Task<IList<OutgoingDocumentAllocationDto>> Handle(
        GetOutgoingDocumentAllocationByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _outgoingDocumentAllocationService
            .GetByDocumentIdAsync(request.OutgoingDocumentId, cancellationToken);
    }
}
