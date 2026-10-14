using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode;

public sealed class GetIncomingDocumentByQrCodeHandler
: IRequestHandler<GetIncomingDocumentByqrCodeQuery, IncomingDocument>
{
    private readonly IIncomingDocumentRepository _documentRepository;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetIncomingDocumentByQrCodeHandler(
        IIncomingDocumentRepository documentRepository,
        IExternalInstitutionService externalInstitutionService)
    {
        _documentRepository = documentRepository;
        _externalInstitutionService = externalInstitutionService;
    }

   public async Task<IncomingDocument> Handle(GetIncomingDocumentByqrCodeQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByExpressionAsync(
     x => x.QrCode == request.qrCode, cancellationToken);
        await _externalInstitutionService.FillAsync(new[] { document }, cancellationToken);
        return document;
    }
}
