using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferAttempts;

public sealed class GetAtlasTransferAttemptsHandler
    : IRequestHandler<GetAtlasTransferAttemptsQuery, IReadOnlyList<AtlasTransferAttemptDto>>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferAttemptsHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<IReadOnlyList<AtlasTransferAttemptDto>> Handle(
        GetAtlasTransferAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetRecentAttemptsAsync(request.Limit, cancellationToken);
    }
}
