using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeById;

public sealed class GetEnvelopeByIdHandler
    : IRequestHandler<GetEnvelopeByIdQuery, EnvelopeDto?>
{
    private readonly IEnvelopeService _envelopeService;

    public GetEnvelopeByIdHandler(IEnvelopeService envelopeService)
    {
        _envelopeService = envelopeService;
    }

    public async Task<EnvelopeDto?> Handle(
        GetEnvelopeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var envelope = await _envelopeService.GetByIdAsync(request.Id, cancellationToken);

        if (envelope == null || envelope.IsDeleted)
            return null;

        return new EnvelopeDto(
            envelope.Id,
            envelope.EnvelopeNo,
            envelope.IsClosed,
            envelope.Status,
            envelope.CreatedByUserId,
            envelope.ExternalInstitutionId,
            envelope.DepartmentId,
            envelope.UnitName,
            envelope.Address,
            envelope.CreatedDate
        );
    }
}