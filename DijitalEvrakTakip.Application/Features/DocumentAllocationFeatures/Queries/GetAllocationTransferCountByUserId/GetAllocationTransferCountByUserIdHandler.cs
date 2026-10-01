using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;

public sealed class GetAllocationTransferCountByUserIdHandler
    : IRequestHandler<GetAllocationTransferCountByUserIdQuery, UserAllocationTransferCountDto>
{
    private readonly IDocumentAllocationService _documentAllocationService;

    public GetAllocationTransferCountByUserIdHandler(
        IDocumentAllocationService documentAllocationService)
    {
        _documentAllocationService = documentAllocationService;
    }

    public Task<UserAllocationTransferCountDto> Handle(
        GetAllocationTransferCountByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _documentAllocationService.GetTransferCountByUserIdAsync(request.UserId, cancellationToken);
    }
}
