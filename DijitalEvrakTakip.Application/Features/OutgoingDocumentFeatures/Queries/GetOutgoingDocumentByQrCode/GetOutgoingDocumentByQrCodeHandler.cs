using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentByQrCode;

public sealed class GetOutgoingDocumentByQrCodeHandler
    : IRequestHandler<GetOutgoingDocumentByQrCodeQuery, OutgoingDocument>
{
    private readonly IOutgoingDocumentRepository _documentRepository;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetOutgoingDocumentByQrCodeHandler(
        IOutgoingDocumentRepository documentRepository,
        IExternalInstitutionService externalInstitutionService)
    {
        _documentRepository = documentRepository;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<OutgoingDocument> Handle(
        GetOutgoingDocumentByQrCodeQuery request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByExpressionAsync(
            x => x.QrCode == request.QrCode, cancellationToken);
        await _externalInstitutionService.FillAsync(new[] { document }, cancellationToken);
        return document;
    }
}
