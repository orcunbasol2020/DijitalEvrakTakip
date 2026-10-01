using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.CreateOutgoingDocumentDistribution;

public sealed class CreateOutgoingDocumentDistributionCommandHandler
    : IRequestHandler<CreateOutgoingDocumentDistributionCommand, IList<OutgoingDocumentDistributionDto>>
{
    private readonly IOutgoingDocumentDistributionService _distributionService;
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public CreateOutgoingDocumentDistributionCommandHandler(
        IOutgoingDocumentDistributionService distributionService,
        IOutgoingDocumentService outgoingDocumentService)
    {
        _distributionService = distributionService;
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<IList<OutgoingDocumentDistributionDto>> Handle(
        CreateOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        var document = await _outgoingDocumentService.GetByIdAsync(request.OutgoingDocumentId, cancellationToken);
        if (document is null || document.IsDeleted)
            throw new Exception("Giden evrak bulunamadı");

        var distributions = request.Recipients
            .Select(r => new OutgoingDocumentDistribution
            {
                OutgoingDocumentId = request.OutgoingDocumentId,
                DepartmentId = r.DepartmentId,
                ExternalInstitutionId = r.ExternalInstitutionId,
                ActionRequired = r.ActionRequired,
                SentDate = r.SentDate,
                DeliveryDate = r.DeliveryDate,
                DeliveryMethod = r.DeliveryMethod,
                Notes = r.Notes
            })
            .ToList();

        await _distributionService.CreateRangeAsync(distributions, cancellationToken);

        // Birim / kurum adlarını doldurmak için kayıtları Include ile geri oku
        var saved = await _distributionService.GetByOutgoingDocumentIdAsync(request.OutgoingDocumentId, cancellationToken);
        var createdIds = distributions.Select(x => x.Id).ToHashSet();

        return saved
            .Where(x => createdIds.Contains(x.Id))
            .Select(OutgoingDocumentDistributionMapper.ToDto)
            .ToList();
    }
}
