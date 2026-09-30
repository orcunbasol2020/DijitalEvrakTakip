using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetPendingAllocationRequestsByUserId;

public sealed class GetPendingAllocationRequestsByUserIdHandler
    : IRequestHandler<GetPendingAllocationRequestsByUserIdQuery, IList<DocumentAllocationRequestDto>>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public GetPendingAllocationRequestsByUserIdHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<IList<DocumentAllocationRequestDto>> Handle(
        GetPendingAllocationRequestsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _allocationRequestService.GetPendingByToUserIdAsync(request.UserId, cancellationToken);
    }
}
