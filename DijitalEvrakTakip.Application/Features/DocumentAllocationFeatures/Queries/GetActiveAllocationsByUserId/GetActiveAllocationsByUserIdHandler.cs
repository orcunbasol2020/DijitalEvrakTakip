using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;

public sealed class GetActiveAllocationsByUserIdHandler
    : IRequestHandler<GetActiveAllocationsByUserIdQuery, IList<UserActiveAllocationDto>>
{
    private readonly IDocumentAllocationService _documentAllocationService;

    public GetActiveAllocationsByUserIdHandler(
        IDocumentAllocationService documentAllocationService)
    {
        _documentAllocationService = documentAllocationService;
    }

    public Task<IList<UserActiveAllocationDto>> Handle(
        GetActiveAllocationsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _documentAllocationService.GetActiveByUserIdAsync(request.UserId, cancellationToken);
    }
}
