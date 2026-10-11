using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;

public sealed class GetAllIncomingDocumentHandler : IRequestHandler<GetAllIncomingDocumentQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetAllIncomingDocumentHandler(
        IIncomingDocumentService incomingDocumentService,
        IExternalInstitutionService externalInstitutionService)
    {
        _incomingDocumentService = incomingDocumentService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken)
    {
        // Servis üzerinden IncomingDocument tablosunu filtreli çekiyoruz
        var documents = await _incomingDocumentService.GetAllAsync(request, cancellationToken);
        await _externalInstitutionService.FillAsync(documents, cancellationToken);
        return documents;
    }

}
