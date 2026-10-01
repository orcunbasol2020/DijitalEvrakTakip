using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetSentAllocationRequestsByUserId;

public sealed class GetSentAllocationRequestsByUserIdHandler
    : IRequestHandler<GetSentAllocationRequestsByUserIdQuery, IList<DocumentAllocationRequestDto>>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public GetSentAllocationRequestsByUserIdHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<IList<DocumentAllocationRequestDto>> Handle(
        GetSentAllocationRequestsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _allocationRequestService.GetSentByUserIdAsync(request.UserId, request.OnlyPending, cancellationToken);
    }
}
