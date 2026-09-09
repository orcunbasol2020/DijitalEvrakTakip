using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetEnvelopeDocumentsByEnvelopeId;

public sealed class GetEnvelopeDocumentsByEnvelopeIdHandler
    : IRequestHandler<GetEnvelopeDocumentsByEnvelopeIdQuery, IList<EnvelopeDocumentDto>>
{
    private readonly IEnvelopeDocumentService _envelopeDocumentService;

    public GetEnvelopeDocumentsByEnvelopeIdHandler(IEnvelopeDocumentService envelopeDocumentService)
    {
        _envelopeDocumentService = envelopeDocumentService;
    }

    public async Task<IList<EnvelopeDocumentDto>> Handle(
        GetEnvelopeDocumentsByEnvelopeIdQuery request,
        CancellationToken cancellationToken)
    {
        var envelopeDocuments = await _envelopeDocumentService.GetAllAsync(cancellationToken);

        return envelopeDocuments
            .Where(x => !x.IsDeleted && x.EnvelopeId == request.EnvelopeId)
            .Select(x => new EnvelopeDocumentDto(
                x.Id,
                x.EnvelopeId,
                x.QrCode,
                x.IsDeleted,
                x.CreatedDate,
                x.UpdateDate
            ))
            .ToList();
    }
}