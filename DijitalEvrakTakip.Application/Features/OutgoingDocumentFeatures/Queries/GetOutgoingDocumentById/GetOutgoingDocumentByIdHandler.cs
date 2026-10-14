using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentById;

public sealed class GetOutgoingDocumentByIdHandler
    : IRequestHandler<GetOutgoingDocumentByIdQuery, OutgoingDocument>
{
    private readonly IOutgoingDocumentRepository _documentRepository;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetOutgoingDocumentByIdHandler(
        IOutgoingDocumentRepository documentRepository,
        IExternalInstitutionService externalInstitutionService)
    {
        _documentRepository = documentRepository;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<OutgoingDocument> Handle(
        GetOutgoingDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByExpressionAsync(
            x => x.Id == request.Id, cancellationToken);
        await _externalInstitutionService.FillAsync(new[] { document }, cancellationToken);
        return document;
    }
}
