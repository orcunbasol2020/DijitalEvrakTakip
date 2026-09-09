using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeByNo;

public sealed class GetEnvelopeByNoHandler
    : IRequestHandler<GetEnvelopeByNoQuery, EnvelopeDto?>
{
    private readonly IEnvelopeService _envelopeService;

    public GetEnvelopeByNoHandler(IEnvelopeService envelopeService)
    {
        _envelopeService = envelopeService;
    }

    public async Task<EnvelopeDto?> Handle(
        GetEnvelopeByNoQuery request,
        CancellationToken cancellationToken)
    {
        var envelopes = await _envelopeService.GetAllAsync(cancellationToken);

        var envelope = envelopes
            .FirstOrDefault(x => !x.IsDeleted && x.EnvelopeNo == request.EnvelopeNo);

        if (envelope == null) return null;

        return new EnvelopeDto(
            envelope.Id,
            envelope.EnvelopeNo,
            envelope.IsClosed,
            envelope.CreatedByUserId,
            envelope.ExternalInstitutionId,
            envelope.DepartmentId,
            envelope.UnitName,
            envelope.Address,
            envelope.CreatedDate
        );
    }
}