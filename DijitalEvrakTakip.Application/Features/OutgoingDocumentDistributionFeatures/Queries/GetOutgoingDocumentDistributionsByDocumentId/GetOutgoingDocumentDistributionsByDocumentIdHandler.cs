using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Queries.GetOutgoingDocumentDistributionsByDocumentId;

public sealed class GetOutgoingDocumentDistributionsByDocumentIdHandler
    : IRequestHandler<GetOutgoingDocumentDistributionsByDocumentIdQuery, IList<OutgoingDocumentDistributionDto>>
{
    private readonly IOutgoingDocumentDistributionService _distributionService;

    public GetOutgoingDocumentDistributionsByDocumentIdHandler(IOutgoingDocumentDistributionService distributionService)
    {
        _distributionService = distributionService;
    }

    public async Task<IList<OutgoingDocumentDistributionDto>> Handle(
        GetOutgoingDocumentDistributionsByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var distributions = await _distributionService
            .GetByOutgoingDocumentIdAsync(request.OutgoingDocumentId, cancellationToken);

        return distributions.Select(OutgoingDocumentDistributionMapper.ToDto).ToList();
    }
}
