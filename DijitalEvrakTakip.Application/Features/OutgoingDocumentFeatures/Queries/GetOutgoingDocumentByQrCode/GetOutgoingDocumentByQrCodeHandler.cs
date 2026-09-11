using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentByQrCode;

public sealed class GetOutgoingDocumentByQrCodeHandler
    : IRequestHandler<GetOutgoingDocumentByQrCodeQuery, OutgoingDocument>
{
    private readonly IOutgoingDocumentRepository _documentRepository;

    public GetOutgoingDocumentByQrCodeHandler(IOutgoingDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<OutgoingDocument> Handle(
        GetOutgoingDocumentByQrCodeQuery request,
        CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByExpressionAsync(
            x => x.QrCode == request.QrCode, cancellationToken);
    }
}
