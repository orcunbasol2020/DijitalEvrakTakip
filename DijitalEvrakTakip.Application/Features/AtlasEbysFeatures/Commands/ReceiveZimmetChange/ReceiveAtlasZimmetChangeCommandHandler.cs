using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Commands.ReceiveZimmetChange;

public sealed class ReceiveAtlasZimmetChangeCommandHandler
    : IRequestHandler<ReceiveAtlasZimmetChangeCommand, MessageResponse>
{
    private readonly IAtlasEbysService _atlasEbysService;

    public ReceiveAtlasZimmetChangeCommandHandler(IAtlasEbysService atlasEbysService)
    {
        _atlasEbysService = atlasEbysService;
    }

    public Task<MessageResponse> Handle(
        ReceiveAtlasZimmetChangeCommand request,
        CancellationToken cancellationToken)
    {
        return _atlasEbysService.ReceiveZimmetChangeAsync(request, cancellationToken);
    }
}
