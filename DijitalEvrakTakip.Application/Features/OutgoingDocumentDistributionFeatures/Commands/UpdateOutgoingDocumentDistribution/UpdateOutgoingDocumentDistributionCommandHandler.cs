using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.UpdateOutgoingDocumentDistribution;

public sealed class UpdateOutgoingDocumentDistributionCommandHandler
    : IRequestHandler<UpdateOutgoingDocumentDistributionCommand, OutgoingDocumentDistributionDto>
{
    private readonly IOutgoingDocumentDistributionService _distributionService;

    public UpdateOutgoingDocumentDistributionCommandHandler(IOutgoingDocumentDistributionService distributionService)
    {
        _distributionService = distributionService;
    }

    public async Task<OutgoingDocumentDistributionDto> Handle(
        UpdateOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _distributionService.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            throw new Exception("Dağıtım kaydı bulunamadı");

        if (request.ActionRequired.HasValue) entity.ActionRequired = request.ActionRequired;
        if (request.SentDate.HasValue) entity.SentDate = request.SentDate;
        if (request.DeliveryDate.HasValue) entity.DeliveryDate = request.DeliveryDate;
        if (request.DeliveryMethod.HasValue) entity.DeliveryMethod = request.DeliveryMethod;
        if (request.Notes != null) entity.Notes = request.Notes;

        await _distributionService.UpdateAsync(entity, cancellationToken);

        return OutgoingDocumentDistributionMapper.ToDto(entity);
    }
}
