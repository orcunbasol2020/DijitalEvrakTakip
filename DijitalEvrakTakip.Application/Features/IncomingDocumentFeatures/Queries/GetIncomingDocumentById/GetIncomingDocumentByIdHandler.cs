using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentById;

public sealed class GetIncomingDocumentByIdHandler
    : IRequestHandler<GetIncomingDocumentByIdQuery, IncomingDocument>
{
    private readonly IIncomingDocumentRepository _documentRepository;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetIncomingDocumentByIdHandler(
        IIncomingDocumentRepository documentRepository,
        IExternalInstitutionService externalInstitutionService)
    {
        _documentRepository = documentRepository;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IncomingDocument> Handle(GetIncomingDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByExpressionAsync(
            x => x.Id == request.Id, cancellationToken);
        await _externalInstitutionService.FillAsync(new[] { document }, cancellationToken);
        return document;
    }
}
