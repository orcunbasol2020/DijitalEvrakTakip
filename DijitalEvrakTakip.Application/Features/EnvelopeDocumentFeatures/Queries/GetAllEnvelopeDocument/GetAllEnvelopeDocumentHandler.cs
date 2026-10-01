using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetAllEnvelopeDocument;

public sealed class GetAllEnvelopeDocumentHandler
    : IRequestHandler<GetAllEnvelopeDocumentQuery, IList<EnvelopeDocumentDto>>
{
    private readonly IEnvelopeDocumentService _envelopeDocumentService;

    public GetAllEnvelopeDocumentHandler(IEnvelopeDocumentService envelopeDocumentService)
    {
        _envelopeDocumentService = envelopeDocumentService;
    }

    public async Task<IList<EnvelopeDocumentDto>> Handle(
        GetAllEnvelopeDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var envelopeDocuments = await _envelopeDocumentService.GetAllAsync(cancellationToken);

        return envelopeDocuments
            .Where(x => !x.IsDeleted)
            .Select(x => new EnvelopeDocumentDto(
                x.Id,
                x.EnvelopeId,
                x.QrCode,
                x.DocumentId,
                x.IsDeleted,
                x.CreatedDate,
                x.UpdateDate
            ))
            .ToList();
    }
}