using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode;

public sealed class GetIncomingDocumentByQrCodeHandler
: IRequestHandler<GetIncomingDocumentByqrCodeQuery, IncomingDocument>
{
    private readonly IIncomingDocumentRepository _documentRepository;

    public GetIncomingDocumentByQrCodeHandler(IIncomingDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

   public async Task<IncomingDocument> Handle(GetIncomingDocumentByqrCodeQuery request, CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByExpressionAsync(
     x => x.QrCode == request.qrCode, cancellationToken);
    }
}
