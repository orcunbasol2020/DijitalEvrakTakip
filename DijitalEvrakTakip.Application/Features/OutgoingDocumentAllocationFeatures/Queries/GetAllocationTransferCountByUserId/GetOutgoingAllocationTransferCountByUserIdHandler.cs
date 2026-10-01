using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;

public sealed class GetOutgoingAllocationTransferCountByUserIdHandler
    : IRequestHandler<GetOutgoingAllocationTransferCountByUserIdQuery, UserAllocationTransferCountDto>
{
    private readonly IOutgoingDocumentAllocationService _outgoingDocumentAllocationService;

    public GetOutgoingAllocationTransferCountByUserIdHandler(
        IOutgoingDocumentAllocationService outgoingDocumentAllocationService)
    {
        _outgoingDocumentAllocationService = outgoingDocumentAllocationService;
    }

    public Task<UserAllocationTransferCountDto> Handle(
        GetOutgoingAllocationTransferCountByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _outgoingDocumentAllocationService.GetTransferCountByUserIdAsync(request.UserId, cancellationToken);
    }
}
