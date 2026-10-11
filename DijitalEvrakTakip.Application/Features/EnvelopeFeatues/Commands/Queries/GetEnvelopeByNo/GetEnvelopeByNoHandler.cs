using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeByNo;

public sealed class GetEnvelopeByNoHandler
    : IRequestHandler<GetEnvelopeByNoQuery, EnvelopeDto?>
{
    private readonly IEnvelopeService _envelopeService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetEnvelopeByNoHandler(
        IEnvelopeService envelopeService,
        IExternalInstitutionService externalInstitutionService)
    {
        _envelopeService = envelopeService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<EnvelopeDto?> Handle(
        GetEnvelopeByNoQuery request,
        CancellationToken cancellationToken)
    {
        var envelopes = await _envelopeService.GetAllAsync(createdByUserId: null, departmentId: null, cancellationToken);

        var envelope = envelopes
            .FirstOrDefault(x => !x.IsDeleted && x.EnvelopeNo == request.EnvelopeNo);

        if (envelope == null) return null;

        string? institutionName = envelope.ExternalInstitutionId.HasValue
            ? (await _externalInstitutionService.GetNamesAsync(new[] { envelope.ExternalInstitutionId.Value }, cancellationToken))
                .GetValueOrDefault(envelope.ExternalInstitutionId.Value)
            : null;

        return new EnvelopeDto(
            envelope.Id,
            envelope.EnvelopeNo,
            envelope.IsClosed,
            envelope.Status,
            envelope.CreatedByUserId,
            envelope.ExternalInstitutionId,
            institutionName,
            envelope.DepartmentId,
            envelope.TargetDepartmentId,
            envelope.UnitName,
            envelope.Address,
            envelope.CreatedDate
        );
    }
}