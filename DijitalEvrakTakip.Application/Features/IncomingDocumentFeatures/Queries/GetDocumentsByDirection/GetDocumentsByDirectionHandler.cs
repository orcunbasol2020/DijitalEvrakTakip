using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocuments;

public sealed class GetDocumentsByDirectionHandler
    : IRequestHandler<GetDocumentsByDirectionQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetDocumentsByDirectionHandler(
        IIncomingDocumentService incomingDocumentService,
        IExternalInstitutionService externalInstitutionService)
    {
        _incomingDocumentService = incomingDocumentService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetDocumentsByDirectionQuery request, CancellationToken cancellationToken)
    {
        var documents = await _incomingDocumentService.GetAllByDirectionAsync(request.DocumentDirection, cancellationToken);
        await _externalInstitutionService.FillAsync(documents, cancellationToken);
        return documents;
    }
}
