using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetAllocationRequestsByDocumentId;

public sealed class GetAllocationRequestsByDocumentIdHandler
    : IRequestHandler<GetAllocationRequestsByDocumentIdQuery, IList<DocumentAllocationRequestDto>>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public GetAllocationRequestsByDocumentIdHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<IList<DocumentAllocationRequestDto>> Handle(
        GetAllocationRequestsByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        return _allocationRequestService.GetByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);
    }
}
