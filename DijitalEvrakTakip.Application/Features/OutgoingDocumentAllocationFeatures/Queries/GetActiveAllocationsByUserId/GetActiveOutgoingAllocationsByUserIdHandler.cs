using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;

public sealed class GetActiveOutgoingAllocationsByUserIdHandler
    : IRequestHandler<GetActiveOutgoingAllocationsByUserIdQuery, IList<UserActiveOutgoingAllocationDto>>
{
    private readonly IOutgoingDocumentAllocationService _outgoingDocumentAllocationService;

    public GetActiveOutgoingAllocationsByUserIdHandler(
        IOutgoingDocumentAllocationService outgoingDocumentAllocationService)
    {
        _outgoingDocumentAllocationService = outgoingDocumentAllocationService;
    }

    public Task<IList<UserActiveOutgoingAllocationDto>> Handle(
        GetActiveOutgoingAllocationsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _outgoingDocumentAllocationService.GetActiveByUserIdAsync(request.UserId, cancellationToken);
    }
}
