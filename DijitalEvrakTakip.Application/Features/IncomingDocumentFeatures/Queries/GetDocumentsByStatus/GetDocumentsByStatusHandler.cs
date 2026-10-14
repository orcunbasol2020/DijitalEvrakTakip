using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetDocumentsByStatus;

public sealed class GetDocumentsByStatusHandler
    : IRequestHandler<GetDocumentsByStatusQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetDocumentsByStatusHandler(
        IIncomingDocumentService incomingDocumentService,
        IExternalInstitutionService externalInstitutionService)
    {
        _incomingDocumentService = incomingDocumentService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetDocumentsByStatusQuery request, CancellationToken cancellationToken)
    {
        var documents = await _incomingDocumentService.GetAllByStatusAsync(request, cancellationToken);
        await _externalInstitutionService.FillAsync(documents, cancellationToken);
        return documents;
    }
}
