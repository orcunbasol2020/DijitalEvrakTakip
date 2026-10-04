using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.RefillAtlasDocumentNumberPool;

public sealed class RefillAtlasDocumentNumberPoolCommandHandler
    : IRequestHandler<RefillAtlasDocumentNumberPoolCommand, AtlasDocumentNumberRefillResultDto>
{
    private readonly IAtlasDocumentNumberPoolService _poolService;

    public RefillAtlasDocumentNumberPoolCommandHandler(IAtlasDocumentNumberPoolService poolService)
    {
        _poolService = poolService;
    }

    public Task<AtlasDocumentNumberRefillResultDto> Handle(
        RefillAtlasDocumentNumberPoolCommand request,
        CancellationToken cancellationToken)
    {
        return _poolService.RefillAsync(ignoreEnabledFlag: true, cancellationToken);
    }
}
