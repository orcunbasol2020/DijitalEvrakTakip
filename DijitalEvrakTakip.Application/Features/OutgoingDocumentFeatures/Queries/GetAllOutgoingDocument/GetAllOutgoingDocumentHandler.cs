using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;

public sealed class GetAllOutgoingDocumentHandler
    : IRequestHandler<GetAllOutgoingDocumentQuery, IList<OutgoingDocument>>
{
    private readonly IOutgoingDocumentService _outgoingDocumentService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetAllOutgoingDocumentHandler(
        IOutgoingDocumentService outgoingDocumentService,
        IExternalInstitutionService externalInstitutionService)
    {
        _outgoingDocumentService = outgoingDocumentService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IList<OutgoingDocument>> Handle(
        GetAllOutgoingDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _outgoingDocumentService.GetAllAsync(request, cancellationToken);
        await _externalInstitutionService.FillAsync(documents, cancellationToken);
        return documents;
    }
}
