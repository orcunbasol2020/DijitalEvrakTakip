using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.RemoveOutgoingDocumentDistribution;

public sealed class RemoveOutgoingDocumentDistributionCommandHandler
    : IRequestHandler<RemoveOutgoingDocumentDistributionCommand, MessageResponse>
{
    private readonly IOutgoingDocumentDistributionService _distributionService;

    public RemoveOutgoingDocumentDistributionCommandHandler(IOutgoingDocumentDistributionService distributionService)
    {
        _distributionService = distributionService;
    }

    public async Task<MessageResponse> Handle(
        RemoveOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        await _distributionService.RemoveAsync(request.Id, cancellationToken);

        return new("Dağıtım kaydı kaldırıldı");
    }
}
