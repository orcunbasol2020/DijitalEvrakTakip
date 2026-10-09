using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentDistributions;

public sealed class GetIncomingDocumentDistributionsHandler
    : IRequestHandler<GetIncomingDocumentDistributionsQuery, IList<IncomingDocumentDistributionDto>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public GetIncomingDocumentDistributionsHandler(IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<IList<IncomingDocumentDistributionDto>> Handle(
        GetIncomingDocumentDistributionsQuery request,
        CancellationToken cancellationToken)
    {
        var distributions = await _incomingDocumentService
            .GetDistributionsAsync(request.IncomingDocumentId, cancellationToken);

        return distributions
            .Select(x => new IncomingDocumentDistributionDto(
                x.Id,
                x.IncomingDocumentId,
                x.DepartmentId,
                x.Department?.Name,
                x.ActionRequired,
                x.Notes,
                x.CreatedDate,
                x.UpdateDate))
            .ToList();
    }
}
